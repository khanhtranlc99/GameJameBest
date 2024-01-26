using Game.Character;
using Game.Character.CharacterController;
using Game.Character.Stats;
using Game.GlobalComponent;
using Game.GlobalComponent.Qwest;
using Game.GlobalComponent.Training;
using Game.Shop;
using System;
using System.Collections.Generic;
using UnityEngine;

public class BaseProfile
{
	public class NullValueStoredExeption : Exception
	{
	}

	private const string ArrayLengthInteline = "Length";

	private const string SerializationPrefix = "v1_3_";

	public static float SoundVolume
	{
		get
		{
			return (float)ResolveValue(SystemSettingsList.SoundVolume.ToString(), 1.0);
		}
		set
		{
			StoreValue((double)value, SystemSettingsList.SoundVolume.ToString());
		}
	}

	public static float MusicVolume
	{
		get
		{
			return ResolveValue(SystemSettingsList.MusicVolume.ToString(), 1f);
		}
		set
		{
			StoreValue(value, SystemSettingsList.MusicVolume.ToString());
		}
	}

	public static bool SkipTraining
	{
		get
		{
			return ResolveValue("SkipTraining", defaultValue: false);
		}
		set
		{
			StoreValue(value, "SkipTraining");
		}
	}

	public static void StoreValue<T>(T value, string key)
	{
		string value2 = MiamiSerializier.JSONSerialize(value);
		PlayerPrefs.SetString("v1_3_" + key, value2);
		PlayerPrefs.Save();
	}

	public static T ResolveValue<T>(string key, T defaultValue)
	{
		string key2 = "v1_3_" + key;
		string @string = PlayerPrefs.GetString(key2);
		object obj;
		try
		{
			obj = MiamiSerializier.JSONDeserialize(@string);
			if (obj is IConvertible)
			{
				IConvertible obj2 = obj as IConvertible;
				return To<T>(obj2);
			}
		}
		catch (Exception)
		{
			StoreValue(defaultValue, key);
			return defaultValue;
			//IL_004e:;
		}
		if (obj == null)
		{
			StoreValue(defaultValue, key);
			obj = defaultValue;
		}
		return (T)obj;
	}

	public static T ResolveValueWitoutDefault<T>(string key)
	{
		string text = "v1_3_" + key;
		string @string = PlayerPrefs.GetString(text);
		object obj = null;
		try
		{
			obj = MiamiSerializier.JSONDeserialize(@string);
		}
		catch (Exception)
		{
			UnityEngine.Debug.LogFormat("BaseProfile: Error loading value with key = {0}", text);
		}
		if (obj is IConvertible)
		{
			IConvertible obj2 = obj as IConvertible;
			return To<T>(obj2);
		}
		return (T)obj;
	}

	public static void ClearValue(string key)
	{
		PlayerPrefs.DeleteKey("v1_3_" + key);
	}

	public static void StoreArray<T>(T[] values, string key)
	{
		StoreValue(values.Length, key + "Length");
		for (int i = 0; i < values.Length; i++)
		{
			StoreValue(values[i], key + i);
		}
	}

	public static void StoreLastElementOfArray<T>(T value, string key)
	{
		int num = ResolveValue(key + "Length", 0);
		StoreValue(num + 1, key + "Length");
		StoreValue(value, key + num);
	}

	public static T[] ResolveArray<T>(string key)
	{
		int num = ResolveValue(key + "Length", 0);
		if (num == 0)
		{
			return null;
		}
		List<T> list = new List<T>();
		for (int i = 0; i < num; i++)
		{
			list.Add(ResolveValueWitoutDefault<T>(key + i));
		}
		return list.ToArray();
	}

	public static void ClearArray<T>(string key)
	{
		int num = ResolveValue(key + "Length", 0);
		if (num != 0)
		{
			for (int i = 0; i < num; i++)
			{
				PlayerPrefs.DeleteKey("v1_3_" + key + i);
			}
			StoreValue(0, key + "Length");
		}
	}

	public static T GetValue<T>(string key)
	{
		string @string = PlayerPrefs.GetString("v1_3_" + key);
		if (@string == null)
		{
			throw new NullValueStoredExeption();
		}
		object obj = null;
		try
		{
			obj = MiamiSerializier.JSONDeserialize(@string);
		}
		catch (Exception)
		{
			throw new NullValueStoredExeption();
			//IL_0032:;
		}
		if (obj == null)
		{
			throw new NullValueStoredExeption();
		}
		if (obj is IConvertible)
		{
			IConvertible obj2 = obj as IConvertible;
			return To<T>(obj2);
		}
		return (T)obj;
	}

	public static void ClearBaseProfileWithoutSystemSettings()
	{
		List<string[]> list = new List<string[]>();
		list.Add(Enum.GetNames(typeof(StatsList)));
		list.Add(Enum.GetNames(typeof(WeaponNameList)));
		list.Add(Enum.GetNames(typeof(QwestProfileList)));
		list.Add(new string[1]
		{
			"SkipTraining"
		});
		foreach (string[] item in list)
		{
			string[] array = item;
			foreach (string str in array)
			{
				PlayerPrefs.DeleteKey("v1_3_" + str);
			}
		}
		TrainingManager.ClearCompletedTrainingsInfo();
		PlayerInfoManager.ClearBI(coinsOnly: false);
		PlayerStoreProfile.ClearLoadout();
		FactionsManager.ClearPlayerRelations();
		PlayerInfoManager.ClearPlayerInfo();
	}

	public static T To<T>(IConvertible obj)
	{
		Type typeFromHandle = typeof(T);
		Type underlyingType = Nullable.GetUnderlyingType(typeFromHandle);
		if (underlyingType != null)
		{
			if (obj == null)
			{
				return default(T);
			}
			return (T)Convert.ChangeType(obj, underlyingType);
		}
		return (T)Convert.ChangeType(obj, typeFromHandle);
	}

	public static bool HasKey(string key)
	{
		return PlayerPrefs.HasKey("v1_3_" + key);
	}

	public static void ClearProfile()
	{
		PlayerPrefs.DeleteAll();
	}
}
