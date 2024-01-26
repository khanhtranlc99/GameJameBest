using UnityEngine;

public class MApplication
{
	private static MApplication _instance;

	public static MApplication instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new MApplication();
			}
			return _instance;
		}
	}

	public static int CurrentLevel
	{
		get;
		set;
	}

	public static Constants.MenuState MenuState
	{
		get;
		set;
	}

	private MApplication()
	{
	}

	static MApplication()
	{
		CurrentLevel = 1;
		Screen.sleepTimeout = -1;
	}
}
