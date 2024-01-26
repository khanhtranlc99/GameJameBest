using Game.Weapons;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Shop
{
	public class PlayerStoreProfile : MonoBehaviour
	{
		[Serializable]
		public class WeaponProfile
		{
			public WeaponNameList WeaponName;

			public Weapon PlayerWeaponLink;
		}

		private const string loadoutSavingKey = "Loadout";

		private static PlayerStoreProfile instance;

		private WeaponController weaponController;

		public static Loadout CurrentLoadout;

		public static PlayerStoreProfile Instance
		{
			get
			{
				if (instance == null)
				{
					throw new Exception("PlayerStoreProfile is not initialized");
				}
				return instance;
			}
		}

		private void Awake()
		{
			instance = this;
		}

		public static void SaveLoadout()
		{
			BaseProfile.StoreValue(CurrentLoadout, "Loadout");
		}

		public static void LoadLoadout()
		{
			CurrentLoadout = BaseProfile.ResolveValueWitoutDefault<Loadout>("Loadout");
			if (CurrentLoadout == null)
			{
				Debug.LogWarning("Cant find saved loadout. Creating new one.");
				CurrentLoadout = new Loadout();
				CurrentLoadout.Weapons = new Dictionary<string, int>();
				CurrentLoadout.Skin = new Dictionary<string, int>
				{
					{
						"HeadID",
						0
					},
					{
						"FaceID",
						0
					},
					{
						"BodyID",
						0
					},
					{
						"ArmsID",
						0
					},
					{
						"ForearmsID",
						0
					},
					{
						"HandsID",
						0
					},
					{
						"UpperLegsID",
						0
					},
					{
						"LowerLegsID",
						0
					},
					{
						"FootsID",
						0
					},
					{
						"HatID",
						0
					},
					{
						"GlassID",
						0
					},
					{
						"MaskID",
						0
					},
					{
						"LeftBraceletID",
						0
					},
					{
						"RightBraceletID",
						0
					},
					{
						"LeftHuckleID",
						0
					},
					{
						"RightHuckleID",
						0
					},
					{
						"LeftPalmID",
						0
					},
					{
						"RightPalmID",
						0
					},
					{
						"LeftToeID",
						0
					},
					{
						"RightToeID",
						0
					},
					{
						"ExternalBodyID",
						0
					},
					{
						"ExternalForearmsID",
						0
					},
					{
						"ExternalFootsID",
						0
					}
				};
			}
		}

		public static void ClearLoadout()
		{
			BaseProfile.ClearValue("Loadout");
		}

		public bool GetOldWeapon(string weaponName)
		{
			return BaseProfile.ResolveValue(weaponName, defaultValue: false);
		}
	}
}
