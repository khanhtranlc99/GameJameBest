using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace FullSerializer
{
	public class fsJsonParser
	{
		private int _start;

		private string _input;

		private readonly StringBuilder _cachedStringBuilder = new StringBuilder(256);

		private fsJsonParser(string input)
		{
			_input = input;
			_start = 0;
		}

		private FsResult MakeFailure(string message)
		{
			int num = Math.Max(0, _start - 20);
			int length = Math.Min(50, _input.Length - num);
			string warning = "Error while parsing: " + message + "; context = <" + _input.Substring(num, length) + ">";
			return FsResult.Fail(warning);
		}

		private bool TryMoveNext()
		{
			if (_start < _input.Length)
			{
				_start++;
				return true;
			}
			return false;
		}

		private bool HasValue()
		{
			return HasValue(0);
		}

		private bool HasValue(int offset)
		{
			return _start + offset >= 0 && _start + offset < _input.Length;
		}

		private char Character()
		{
			return Character(0);
		}

		private char Character(int offset)
		{
			return _input[_start + offset];
		}

		private void SkipSpace()
		{
			while (HasValue())
			{
				char c = Character();
				if (char.IsWhiteSpace(c))
				{
					TryMoveNext();
					continue;
				}
				if (!HasValue(1) || Character(0) != '/')
				{
					break;
				}
				if (Character(1) == '/')
				{
					while (HasValue() && !Environment.NewLine.Contains(string.Empty + Character()))
					{
						TryMoveNext();
					}
				}
				else
				{
					if (Character(1) != '*')
					{
						continue;
					}
					TryMoveNext();
					TryMoveNext();
					while (HasValue(1))
					{
						if (Character(0) == '*' && Character(1) == '/')
						{
							TryMoveNext();
							TryMoveNext();
							TryMoveNext();
							break;
						}
						TryMoveNext();
					}
				}
			}
		}

		private bool IsHex(char c)
		{
			return (c >= '0' && c <= '9') || (c >= 'a' && c <= 'f') || (c >= 'A' && c <= 'F');
		}

		private uint ParseSingleChar(char c1, uint multipliyer)
		{
			uint result = 0u;
			if (c1 >= '0' && c1 <= '9')
			{
				result = (uint)((c1 - 48) * (int)multipliyer);
			}
			else if (c1 >= 'A' && c1 <= 'F')
			{
				result = (uint)((c1 - 65 + 10) * (int)multipliyer);
			}
			else if (c1 >= 'a' && c1 <= 'f')
			{
				result = (uint)((c1 - 97 + 10) * (int)multipliyer);
			}
			return result;
		}

		private uint ParseUnicode(char c1, char c2, char c3, char c4)
		{
			uint num = ParseSingleChar(c1, 4096u);
			uint num2 = ParseSingleChar(c2, 256u);
			uint num3 = ParseSingleChar(c3, 16u);
			uint num4 = ParseSingleChar(c4, 1u);
			return num + num2 + num3 + num4;
		}

		private FsResult TryUnescapeChar(out char escaped)
		{
			TryMoveNext();
			if (!HasValue())
			{
				escaped = ' ';
				return MakeFailure("Unexpected end of input after \\");
			}
			switch (Character())
			{
			case '\\':
				TryMoveNext();
				escaped = '\\';
				return FsResult.Success;
			case '/':
				TryMoveNext();
				escaped = '/';
				return FsResult.Success;
			case '"':
				TryMoveNext();
				escaped = '"';
				return FsResult.Success;
			case 'a':
				TryMoveNext();
				escaped = '\a';
				return FsResult.Success;
			case 'b':
				TryMoveNext();
				escaped = '\b';
				return FsResult.Success;
			case 'f':
				TryMoveNext();
				escaped = '\f';
				return FsResult.Success;
			case 'n':
				TryMoveNext();
				escaped = '\n';
				return FsResult.Success;
			case 'r':
				TryMoveNext();
				escaped = '\r';
				return FsResult.Success;
			case 't':
				TryMoveNext();
				escaped = '\t';
				return FsResult.Success;
			case '0':
				TryMoveNext();
				escaped = '\0';
				return FsResult.Success;
			case 'u':
				TryMoveNext();
				if (IsHex(Character(0)) && IsHex(Character(1)) && IsHex(Character(2)) && IsHex(Character(3)))
				{
					uint num = ParseUnicode(Character(0), Character(1), Character(2), Character(3));
					TryMoveNext();
					TryMoveNext();
					TryMoveNext();
					TryMoveNext();
					escaped = (char)num;
					return FsResult.Success;
				}
				escaped = '\0';
				return MakeFailure($"invalid escape sequence '\\u{Character(0)}{Character(1)}{Character(2)}{Character(3)}'\n");
			default:
				escaped = '\0';
				return MakeFailure($"Invalid escape sequence \\{Character()}");
			}
		}

		private FsResult TryParseExact(string content)
		{
			for (int i = 0; i < content.Length; i++)
			{
				if (Character() != content[i])
				{
					return MakeFailure("Expected " + content[i]);
				}
				if (!TryMoveNext())
				{
					return MakeFailure("Unexpected end of content when parsing " + content);
				}
			}
			return FsResult.Success;
		}

		private FsResult TryParseTrue(out fsData data)
		{
			FsResult result = TryParseExact("true");
			if (result.Succeeded)
			{
				data = new fsData(boolean: true);
				return FsResult.Success;
			}
			data = null;
			return result;
		}

		private FsResult TryParseFalse(out fsData data)
		{
			FsResult result = TryParseExact("false");
			if (result.Succeeded)
			{
				data = new fsData(boolean: false);
				return FsResult.Success;
			}
			data = null;
			return result;
		}

		private FsResult TryParseNull(out fsData data)
		{
			FsResult result = TryParseExact("null");
			if (result.Succeeded)
			{
				data = new fsData();
				return FsResult.Success;
			}
			data = null;
			return result;
		}

		private bool IsSeparator(char c)
		{
			return char.IsWhiteSpace(c) || c == ',' || c == '}' || c == ']';
		}

		private FsResult TryParseNumber(out fsData data)
		{
			int start = _start;
			while (TryMoveNext() && HasValue() && !IsSeparator(Character()))
			{
			}
			string text = _input.Substring(start, _start - start);
			if (text.Contains(".") || text.Contains("e") || text.Contains("E") || text == "Infinity" || text == "-Infinity" || text == "NaN")
			{
				double result;
				if (!double.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out result))
				{
					data = null;
					return MakeFailure("Bad double format with " + text);
				}
				data = new fsData(result);
				return FsResult.Success;
			}

			long result2;
			if (!long.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out result2))
			{
				data = null;
				return MakeFailure("Bad Int64 format with " + text);
			}
			data = new fsData(result2);
			return FsResult.Success;
		}

		private FsResult TryParseString(out string str)
		{
			_cachedStringBuilder.Length = 0;
			if (Character() != '"' || !TryMoveNext())
			{
				str = string.Empty;
				return MakeFailure("Expected initial \" when parsing a string");
			}
			while (HasValue() && Character() != '"')
			{
				char c = Character();
				if (c == '\\')
				{
					char escaped;
					FsResult result = TryUnescapeChar(out escaped);
					if (result.Failed)
					{
						str = string.Empty;
						return result;
					}
					_cachedStringBuilder.Append(escaped);
				}
				else
				{
					_cachedStringBuilder.Append(c);
					if (!TryMoveNext())
					{
						str = string.Empty;
						return MakeFailure("Unexpected end of input when reading a string");
					}
				}
			}
			if (!HasValue() || Character() != '"' || !TryMoveNext())
			{
				str = string.Empty;
				return MakeFailure("No closing \" when parsing a string");
			}
			str = _cachedStringBuilder.ToString();
			return FsResult.Success;
		}

		private FsResult TryParseArray(out fsData arr)
		{
			if (Character() != '[')
			{
				arr = null;
				return MakeFailure("Expected initial [ when parsing an array");
			}
			if (!TryMoveNext())
			{
				arr = null;
				return MakeFailure("Unexpected end of input when parsing an array");
			}
			SkipSpace();
			List<fsData> list = new List<fsData>();
			while (HasValue() && Character() != ']')
			{
				fsData data;
				FsResult result = RunParse(out data);
				if (result.Failed)
				{
					arr = null;
					return result;
				}
				list.Add(data);
				SkipSpace();
				if (HasValue() && Character() == ',')
				{
					if (!TryMoveNext())
					{
						break;
					}
					SkipSpace();
				}
			}
			if (!HasValue() || Character() != ']' || !TryMoveNext())
			{
				arr = null;
				return MakeFailure("No closing ] for array");
			}
			arr = new fsData(list);
			return FsResult.Success;
		}

		private FsResult TryParseObject(out fsData obj)
		{
			if (Character() != '{')
			{
				obj = null;
				return MakeFailure("Expected initial { when parsing an object");
			}
			if (!TryMoveNext())
			{
				obj = null;
				return MakeFailure("Unexpected end of input when parsing an object");
			}
			SkipSpace();
			Dictionary<string, fsData> dictionary = new Dictionary<string, fsData>((!fsGlobalConfig.IsCaseSensitive) ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal);
			while (HasValue() && Character() != '}')
			{
				SkipSpace();
				string str;
				FsResult result = TryParseString(out str);
				if (result.Failed)
				{
					obj = null;
					return result;
				}
				SkipSpace();
				if (!HasValue() || Character() != ':' || !TryMoveNext())
				{
					obj = null;
					return MakeFailure("Expected : after key \"" + str + "\"");
				}
				SkipSpace();
				fsData data;
				result = RunParse(out data);
				if (result.Failed)
				{
					obj = null;
					return result;
				}
				dictionary.Add(str, data);
				SkipSpace();
				if (HasValue() && Character() == ',')
				{
					if (!TryMoveNext())
					{
						break;
					}
					SkipSpace();
				}
			}
			if (!HasValue() || Character() != '}' || !TryMoveNext())
			{
				obj = null;
				return MakeFailure("No closing } for object");
			}
			obj = new fsData(dictionary);
			return FsResult.Success;
		}

		private FsResult RunParse(out fsData data)
		{
			SkipSpace();
			if (!HasValue())
			{
				data = null;
				return MakeFailure("Unexpected end of input");
			}
			switch (Character())
			{
			case '+':
			case '-':
			case '.':
			case '0':
			case '1':
			case '2':
			case '3':
			case '4':
			case '5':
			case '6':
			case '7':
			case '8':
			case '9':
			case 'I':
			case 'N':
				return TryParseNumber(out data);
			case '"':
			{
				string str;
				FsResult result = TryParseString(out str);
				if (result.Failed)
				{
					data = null;
					return result;
				}
				data = new fsData(str);
				return FsResult.Success;
			}
			case '[':
				return TryParseArray(out data);
			case '{':
				return TryParseObject(out data);
			case 't':
				return TryParseTrue(out data);
			case 'f':
				return TryParseFalse(out data);
			case 'n':
				return TryParseNull(out data);
			default:
				data = null;
				return MakeFailure("unable to parse; invalid token \"" + Character() + "\"");
			}
		}

		public static FsResult Parse(string input, out fsData data)
		{
			if (string.IsNullOrEmpty(input))
			{
				data = null;
				return FsResult.Fail("No input");
			}
			fsJsonParser fsJsonParser = new fsJsonParser(input);
			return fsJsonParser.RunParse(out data);
		}

		public static fsData Parse(string input)
		{
			fsData data;
			Parse(input, out data).AssertSuccess();
			return data;
		}
	}
}
