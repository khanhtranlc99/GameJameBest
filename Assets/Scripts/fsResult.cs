using System;
using System.Collections.Generic;
using System.Linq;

namespace FullSerializer
{
	public struct FsResult
	{
		private static readonly string[] EmptyStringArray = new string[0];

		private bool _success;

		private List<string> _messages;

		public static FsResult Success = new FsResult
		{
			_success = true
		};

		public bool Failed
		{
			get
            {
				return !_success;
			}
		}

		public bool Succeeded
		{
			get
            {
				return _success;
			}
		}
		

		public bool HasWarnings
		{
			get
            {
				return _messages != null && _messages.Any();
			}
		}
		

		public Exception AsException
		{
			get
			{
				if (!Failed && !RawMessages.Any())
				{
					throw new Exception("Only a failed result can be converted to an exception");
				}
				return new Exception(FormattedMessages);
			}
		}

		public IEnumerable<string> RawMessages
		{
			get
			{
				if (_messages != null)
				{
					return _messages;
				}
				return EmptyStringArray;
			}
		}

		public string FormattedMessages
		{
			get
            {
				return string.Join(",\n", RawMessages.ToArray());
			}
		}
		

		public void AddMessage(string message)
		{
			if (_messages == null)
			{
				_messages = new List<string>();
			}
			_messages.Add(message);
		}

		public void AddMessages(FsResult result)
		{
			if (result._messages != null)
			{
				if (_messages == null)
				{
					_messages = new List<string>();
				}
				_messages.AddRange(result._messages);
			}
		}

		public FsResult Merge(FsResult other)
		{
			_success = (_success && other._success);
			if (other._messages != null)
			{
				if (_messages == null)
				{
					_messages = new List<string>(other._messages);
				}
				else
				{
					_messages.AddRange(other._messages);
				}
			}
			return this;
		}

		public static FsResult Warn(string warning)
		{
			FsResult result = default(FsResult);
			result._success = true;
			result._messages = new List<string>
			{
				warning
			};
			return result;
		}

		public static FsResult Fail(string warning)
		{
			FsResult result = default(FsResult);
			result._success = false;
			result._messages = new List<string>
			{
				warning
			};
			return result;
		}

		public FsResult AssertSuccess()
		{
			if (Failed)
			{
				throw AsException;
			}
			return this;
		}

		public FsResult AssertSuccessWithoutWarnings()
		{
			if (Failed || RawMessages.Any())
			{
				throw AsException;
			}
			return this;
		}

		public static FsResult operator +(FsResult a, FsResult b)
		{
			return a.Merge(b);
		}
	}
}
