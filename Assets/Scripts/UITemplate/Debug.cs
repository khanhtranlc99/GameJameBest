using UnityEngine.UI;

namespace UITemplate
{
	public class Debug
	{
		private static Text logText;

		public static void Log(string message)
		{
			InitLog();
		}

		private static void LogMessage(string message)
		{
			if (logText != null)
			{
				logText.text = logText.text + "\n" + message;
			}
		}

		public static void LogFormat(string format, params object[] args)
		{
			InitLog();
		}

		private static void InitLog()
		{
		}
	}
}
