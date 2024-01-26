public class LevelInfo
{
	private int _starsCount;

	private bool _isAvailable;

	public int StarsCount
	{
		get
		{
			return _starsCount;
		}
		set
		{
			_starsCount = value;
		}
	}

	public bool IsAvailable
	{
		get
		{
			return _isAvailable;
		}
		set
		{
			_isAvailable = value;
		}
	}
}
