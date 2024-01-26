using UnityEngine;

public class OneFrameFixedUpdate
{
	private int _prevFrame;

	private int _fixedCounts = 1;

	public int FixedCounts => _fixedCounts;

	public bool IsSameFrameCountFrame()
	{
		if (_prevFrame == Time.renderedFrameCount)
		{
			_fixedCounts++;
			return true;
		}
		_fixedCounts = 1;
		_prevFrame = Time.renderedFrameCount;
		return false;
	}

	public bool IsSameFrame()
	{
		if (_prevFrame == Time.renderedFrameCount)
		{
			return true;
		}
		_fixedCounts = 1;
		_prevFrame = Time.renderedFrameCount;
		return false;
	}
}
