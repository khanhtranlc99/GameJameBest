using Game.Character;
using Game.GlobalComponent;
using System;
using System.Collections.Generic;
using UnityEngine;

public class SearchProcess<T> : ISeachProcess where T : Component
{
	private const int C_ARRAY_SIZE = 200;

	public T[] m_Founded;

	private List<MarkContainer> containersMarks;

	public int countMarks = 10;

	public string markType = "Kill";

	public Func<T, bool> Condition;

	private DistanceComparer<T> comparer;

	public SearchProcess()
	{
		Condition = DefaultCondition;
	}

	public SearchProcess(Func<T, bool> condition)
	{
		Condition = condition;
	}

	public bool DefaultCondition(T obj)
	{
		return true;
	}

	public void Initialize()
	{
		comparer = new DistanceComparer<T>();
		m_Founded = new T[200];
		containersMarks = new List<MarkContainer>();
		for (int i = 0; i < countMarks; i++)
		{
			containersMarks.Add(UIMarkManager.Instance.AddDinamicMark(null, markType));
		}
	}

	public void Processing()
	{
		int itemInUse = PoolManager.Instance.GetItemInUse(m_Founded);
		if (itemInUse > 0)
		{
			comparer.playerPosition = PlayerInteractionsManager.Instance.GetPlayerPosition();
			Array.Sort(m_Founded, 0, itemInUse, comparer);
		}
		int num = 0;
		for (int i = 0; i < itemInUse; i++)
		{
			if (num >= countMarks)
			{
				break;
			}
			if (Condition(m_Founded[i]))
			{
				containersMarks[num].Target = m_Founded[i].transform;
				num++;
			}
		}
		for (int j = num; j < countMarks; j++)
		{
			containersMarks[num].Target = null;
		}
	}

	public void Release()
	{
		if (UIMarkManager.InstanceExist)
		{
			for (int i = 0; i < countMarks; i++)
			{
				UIMarkManager.Instance.RemoveDinamicMarks(containersMarks[i]);
			}
		}
	}
}
