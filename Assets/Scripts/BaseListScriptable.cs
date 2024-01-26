using System.Collections.Generic;
using UnityEngine;

public abstract class BaseListScriptable<T> : ScriptableObject
{
	[SerializeField]
	protected List<T> m_Details;

	public T this[int index]
	{
		get
        {
			return m_Details[index];
		}
	}
	

	public int Count
	{
		get
        {
			return m_Details.Count;
		}
	}
	

	protected virtual void Awake()
	{
	}

	protected virtual void OnEnable()
	{
	}

	protected virtual void OnDisable()
	{
	}

	protected virtual void OnDestroy()
	{
	}
}
