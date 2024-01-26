using System.Collections.Generic;
using UnityEngine;

namespace Game.Character.Config
{
	public class ThirdPersonConfigBIG : Config
	{
		public override void LoadDefault()
		{
			Dictionary<string, Param> dictionary = new Dictionary<string, Param>();
			dictionary.Add("FOV", new RangeParam
			{
				value = 60f,
				min = 20f,
				max = 100f
			});
			dictionary.Add("Distance", new RangeParam
			{
				value = 2f,
				min = 0f,
				max = 10f
			});
			dictionary.Add("RotationSpeedX", new RangeParam
			{
				value = 8f,
				min = 0f,
				max = 10f
			});
			dictionary.Add("RotationSpeedY", new RangeParam
			{
				value = 5f,
				min = 0f,
				max = 10f
			});
			dictionary.Add("RotationYMax", new RangeParam
			{
				value = 80f,
				min = 0f,
				max = 80f
			});
			dictionary.Add("RotationYMin", new RangeParam
			{
				value = -80f,
				min = -80f,
				max = 0f
			});
			dictionary.Add("TargetOffset", new Vector3Param
			{
				value = Vector3.zero
			});
			dictionary.Add("FollowCoef", new RangeParam
			{
				value = 1f,
				min = 0f,
				max = 10f
			});
			dictionary.Add("Spring", new RangeParam
			{
				value = 0f,
				min = 0f,
				max = 1f
			});
			dictionary.Add("DefaultYRotation", new RangeParam
			{
				value = 0f,
				min = -80f,
				max = 80f
			});
			dictionary.Add("AutoYRotation", new RangeParam
			{
				value = 0f,
				min = 0f,
				max = 1f
			});
			dictionary.Add("DeadZone", new Vector2Param
			{
				value = Vector2.zero
			});
			dictionary.Add("Orthographic", new BoolParam
			{
				value = false
			});
			Dictionary<string, Param> value = dictionary;
			dictionary = new Dictionary<string, Param>();
			dictionary.Add("FOV", new RangeParam
			{
				value = 60f,
				min = 20f,
				max = 100f
			});
			dictionary.Add("Distance", new RangeParam
			{
				value = 2f,
				min = 0f,
				max = 10f
			});
			dictionary.Add("RotationSpeedX", new RangeParam
			{
				value = 8f,
				min = 0f,
				max = 10f
			});
			dictionary.Add("RotationSpeedY", new RangeParam
			{
				value = 5f,
				min = 0f,
				max = 10f
			});
			dictionary.Add("RotationYMax", new RangeParam
			{
				value = 80f,
				min = 0f,
				max = 80f
			});
			dictionary.Add("RotationYMin", new RangeParam
			{
				value = -80f,
				min = -80f,
				max = 0f
			});
			dictionary.Add("TargetOffset", new Vector3Param
			{
				value = Vector3.zero
			});
			dictionary.Add("FollowCoef", new RangeParam
			{
				value = 1f,
				min = 0f,
				max = 10f
			});
			dictionary.Add("Spring", new RangeParam
			{
				value = 0f,
				min = 0f,
				max = 1f
			});
			dictionary.Add("DefaultYRotation", new RangeParam
			{
				value = 0f,
				min = -80f,
				max = 80f
			});
			dictionary.Add("AutoYRotation", new RangeParam
			{
				value = 0f,
				min = 0f,
				max = 1f
			});
			dictionary.Add("DeadZone", new Vector2Param
			{
				value = Vector2.zero
			});
			dictionary.Add("Orthographic", new BoolParam
			{
				value = false
			});
			Dictionary<string, Param> value2 = dictionary;
			dictionary = new Dictionary<string, Param>();
			dictionary.Add("FOV", new RangeParam
			{
				value = 40f,
				min = 20f,
				max = 100f
			});
			dictionary.Add("Distance", new RangeParam
			{
				value = 2f,
				min = 0f,
				max = 10f
			});
			dictionary.Add("RotationSpeedX", new RangeParam
			{
				value = 8f,
				min = 0f,
				max = 10f
			});
			dictionary.Add("RotationSpeedY", new RangeParam
			{
				value = 5f,
				min = 0f,
				max = 10f
			});
			dictionary.Add("RotationYMax", new RangeParam
			{
				value = 80f,
				min = 0f,
				max = 80f
			});
			dictionary.Add("RotationYMin", new RangeParam
			{
				value = -80f,
				min = -80f,
				max = 0f
			});
			dictionary.Add("TargetOffset", new Vector3Param
			{
				value = Vector3.zero
			});
			dictionary.Add("FollowCoef", new RangeParam
			{
				value = 0f,
				min = 0f,
				max = 10f
			});
			dictionary.Add("Spring", new RangeParam
			{
				value = 0f,
				min = 0f,
				max = 1f
			});
			dictionary.Add("DefaultYRotation", new RangeParam
			{
				value = 0f,
				min = -80f,
				max = 80f
			});
			dictionary.Add("AutoYRotation", new RangeParam
			{
				value = 0f,
				min = 0f,
				max = 1f
			});
			dictionary.Add("DeadZone", new Vector2Param
			{
				value = Vector2.zero
			});
			dictionary.Add("Orthographic", new BoolParam
			{
				value = false
			});
			Dictionary<string, Param> value3 = dictionary;
			dictionary = new Dictionary<string, Param>();
			dictionary.Add("FOV", new RangeParam
			{
				value = 40f,
				min = 20f,
				max = 100f
			});
			dictionary.Add("Distance", new RangeParam
			{
				value = 2f,
				min = 0f,
				max = 10f
			});
			dictionary.Add("RotationSpeedX", new RangeParam
			{
				value = 8f,
				min = 0f,
				max = 10f
			});
			dictionary.Add("RotationSpeedY", new RangeParam
			{
				value = 5f,
				min = 0f,
				max = 10f
			});
			dictionary.Add("RotationYMax", new RangeParam
			{
				value = 80f,
				min = 0f,
				max = 80f
			});
			dictionary.Add("RotationYMin", new RangeParam
			{
				value = -80f,
				min = -80f,
				max = 0f
			});
			dictionary.Add("TargetOffset", new Vector3Param
			{
				value = Vector3.zero
			});
			dictionary.Add("FollowCoef", new RangeParam
			{
				value = 0f,
				min = 0f,
				max = 10f
			});
			dictionary.Add("Spring", new RangeParam
			{
				value = 0f,
				min = 0f,
				max = 1f
			});
			dictionary.Add("DefaultYRotation", new RangeParam
			{
				value = 0f,
				min = -80f,
				max = 80f
			});
			dictionary.Add("AutoYRotation", new RangeParam
			{
				value = 0f,
				min = 0f,
				max = 1f
			});
			dictionary.Add("DeadZone", new Vector2Param
			{
				value = Vector2.zero
			});
			dictionary.Add("Orthographic", new BoolParam
			{
				value = false
			});
			Dictionary<string, Param> value4 = dictionary;
			dictionary = new Dictionary<string, Param>();
			dictionary.Add("FOV", new RangeParam
			{
				value = 60f,
				min = 20f,
				max = 100f
			});
			dictionary.Add("Distance", new RangeParam
			{
				value = 2f,
				min = 0f,
				max = 10f
			});
			dictionary.Add("RotationSpeedX", new RangeParam
			{
				value = 8f,
				min = 0f,
				max = 10f
			});
			dictionary.Add("RotationSpeedY", new RangeParam
			{
				value = 5f,
				min = 0f,
				max = 10f
			});
			dictionary.Add("RotationYMax", new RangeParam
			{
				value = 80f,
				min = 0f,
				max = 80f
			});
			dictionary.Add("RotationYMin", new RangeParam
			{
				value = -80f,
				min = -80f,
				max = 0f
			});
			dictionary.Add("TargetOffset", new Vector3Param
			{
				value = Vector3.zero
			});
			dictionary.Add("FollowCoef", new RangeParam
			{
				value = 1f,
				min = 0f,
				max = 10f
			});
			dictionary.Add("Spring", new RangeParam
			{
				value = 0f,
				min = 0f,
				max = 1f
			});
			dictionary.Add("DefaultYRotation", new RangeParam
			{
				value = 0f,
				min = -80f,
				max = 80f
			});
			dictionary.Add("AutoYRotation", new RangeParam
			{
				value = 0f,
				min = 0f,
				max = 1f
			});
			dictionary.Add("DeadZone", new Vector2Param
			{
				value = Vector2.zero
			});
			dictionary.Add("Orthographic", new BoolParam
			{
				value = false
			});
			Dictionary<string, Param> value5 = dictionary;
			dictionary = new Dictionary<string, Param>();
			dictionary.Add("FOV", new RangeParam
			{
				value = 60f,
				min = 20f,
				max = 100f
			});
			dictionary.Add("Distance", new RangeParam
			{
				value = 2f,
				min = 0f,
				max = 10f
			});
			dictionary.Add("RotationSpeedX", new RangeParam
			{
				value = 8f,
				min = 0f,
				max = 10f
			});
			dictionary.Add("RotationSpeedY", new RangeParam
			{
				value = 5f,
				min = 0f,
				max = 10f
			});
			dictionary.Add("RotationYMax", new RangeParam
			{
				value = 80f,
				min = 0f,
				max = 80f
			});
			dictionary.Add("RotationYMin", new RangeParam
			{
				value = -80f,
				min = -80f,
				max = 0f
			});
			dictionary.Add("TargetOffset", new Vector3Param
			{
				value = Vector3.zero
			});
			dictionary.Add("FollowCoef", new RangeParam
			{
				value = 1f,
				min = 0f,
				max = 10f
			});
			dictionary.Add("Spring", new RangeParam
			{
				value = 0f,
				min = 0f,
				max = 1f
			});
			dictionary.Add("DefaultYRotation", new RangeParam
			{
				value = 0f,
				min = -80f,
				max = 80f
			});
			dictionary.Add("AutoYRotation", new RangeParam
			{
				value = 0f,
				min = 0f,
				max = 1f
			});
			dictionary.Add("DeadZone", new Vector2Param
			{
				value = Vector2.zero
			});
			dictionary.Add("Orthographic", new BoolParam
			{
				value = false
			});
			Dictionary<string, Param> value6 = dictionary;
			dictionary = new Dictionary<string, Param>();
			dictionary.Add("FOV", new RangeParam
			{
				value = 60f,
				min = 20f,
				max = 100f
			});
			dictionary.Add("Distance", new RangeParam
			{
				value = 2f,
				min = 0f,
				max = 10f
			});
			dictionary.Add("RotationSpeedX", new RangeParam
			{
				value = 8f,
				min = 0f,
				max = 10f
			});
			dictionary.Add("RotationSpeedY", new RangeParam
			{
				value = 5f,
				min = 0f,
				max = 10f
			});
			dictionary.Add("RotationYMax", new RangeParam
			{
				value = 80f,
				min = 0f,
				max = 80f
			});
			dictionary.Add("RotationYMin", new RangeParam
			{
				value = -80f,
				min = -80f,
				max = 0f
			});
			dictionary.Add("TargetOffset", new Vector3Param
			{
				value = Vector3.zero
			});
			dictionary.Add("FollowCoef", new RangeParam
			{
				value = 0f,
				min = 0f,
				max = 10f
			});
			dictionary.Add("Spring", new RangeParam
			{
				value = 0f,
				min = 0f,
				max = 1f
			});
			dictionary.Add("DefaultYRotation", new RangeParam
			{
				value = 0f,
				min = -80f,
				max = 80f
			});
			dictionary.Add("AutoYRotation", new RangeParam
			{
				value = 0f,
				min = 0f,
				max = 1f
			});
			dictionary.Add("DeadZone", new Vector2Param
			{
				value = Vector2.zero
			});
			dictionary.Add("Orthographic", new BoolParam
			{
				value = false
			});
			Dictionary<string, Param> value7 = dictionary;
			dictionary = new Dictionary<string, Param>();
			dictionary.Add("FOV", new RangeParam
			{
				value = 60f,
				min = 20f,
				max = 100f
			});
			dictionary.Add("Distance", new RangeParam
			{
				value = 2f,
				min = 0f,
				max = 10f
			});
			dictionary.Add("RotationSpeedX", new RangeParam
			{
				value = 8f,
				min = 0f,
				max = 10f
			});
			dictionary.Add("RotationSpeedY", new RangeParam
			{
				value = 5f,
				min = 0f,
				max = 10f
			});
			dictionary.Add("RotationYMax", new RangeParam
			{
				value = 80f,
				min = 0f,
				max = 80f
			});
			dictionary.Add("RotationYMin", new RangeParam
			{
				value = -80f,
				min = -80f,
				max = 0f
			});
			dictionary.Add("TargetOffset", new Vector3Param
			{
				value = Vector3.zero
			});
			dictionary.Add("FollowCoef", new RangeParam
			{
				value = 0f,
				min = 0f,
				max = 10f
			});
			dictionary.Add("Spring", new RangeParam
			{
				value = 0f,
				min = 0f,
				max = 1f
			});
			dictionary.Add("DefaultYRotation", new RangeParam
			{
				value = 0f,
				min = -80f,
				max = 80f
			});
			dictionary.Add("AutoYRotation", new RangeParam
			{
				value = 0f,
				min = 0f,
				max = 1f
			});
			dictionary.Add("DeadZone", new Vector2Param
			{
				value = Vector2.zero
			});
			dictionary.Add("Orthographic", new BoolParam
			{
				value = false
			});
			Dictionary<string, Param> value8 = dictionary;
			dictionary = new Dictionary<string, Param>();
			dictionary.Add("FOV", new RangeParam
			{
				value = 60f,
				min = 20f,
				max = 100f
			});
			dictionary.Add("Distance", new RangeParam
			{
				value = 2f,
				min = 0f,
				max = 10f
			});
			dictionary.Add("RotationSpeedX", new RangeParam
			{
				value = 8f,
				min = 0f,
				max = 10f
			});
			dictionary.Add("RotationSpeedY", new RangeParam
			{
				value = 5f,
				min = 0f,
				max = 10f
			});
			dictionary.Add("RotationYMax", new RangeParam
			{
				value = 80f,
				min = 0f,
				max = 80f
			});
			dictionary.Add("RotationYMin", new RangeParam
			{
				value = -80f,
				min = -80f,
				max = 0f
			});
			dictionary.Add("TargetOffset", new Vector3Param
			{
				value = Vector3.zero
			});
			dictionary.Add("FollowCoef", new RangeParam
			{
				value = 0f,
				min = 0f,
				max = 10f
			});
			dictionary.Add("Spring", new RangeParam
			{
				value = 0f,
				min = 0f,
				max = 1f
			});
			dictionary.Add("DefaultYRotation", new RangeParam
			{
				value = 0f,
				min = -80f,
				max = 80f
			});
			dictionary.Add("AutoYRotation", new RangeParam
			{
				value = 0f,
				min = 0f,
				max = 1f
			});
			dictionary.Add("DeadZone", new Vector2Param
			{
				value = Vector2.zero
			});
			dictionary.Add("Orthographic", new BoolParam
			{
				value = false
			});
			Dictionary<string, Param> value9 = dictionary;
			dictionary = new Dictionary<string, Param>();
			dictionary.Add("FOV", new RangeParam
			{
				value = 60f,
				min = 20f,
				max = 100f
			});
			dictionary.Add("Distance", new RangeParam
			{
				value = 2f,
				min = 0f,
				max = 10f
			});
			dictionary.Add("RotationSpeedX", new RangeParam
			{
				value = 8f,
				min = 0f,
				max = 10f
			});
			dictionary.Add("RotationSpeedY", new RangeParam
			{
				value = 5f,
				min = 0f,
				max = 10f
			});
			dictionary.Add("RotationYMax", new RangeParam
			{
				value = 80f,
				min = 0f,
				max = 80f
			});
			dictionary.Add("RotationYMin", new RangeParam
			{
				value = -80f,
				min = -80f,
				max = 0f
			});
			dictionary.Add("TargetOffset", new Vector3Param
			{
				value = Vector3.zero
			});
			dictionary.Add("FollowCoef", new RangeParam
			{
				value = 0f,
				min = 0f,
				max = 10f
			});
			dictionary.Add("Spring", new RangeParam
			{
				value = 0f,
				min = 0f,
				max = 1f
			});
			dictionary.Add("DefaultYRotation", new RangeParam
			{
				value = 0f,
				min = -80f,
				max = 80f
			});
			dictionary.Add("AutoYRotation", new RangeParam
			{
				value = 0f,
				min = 0f,
				max = 1f
			});
			dictionary.Add("DeadZone", new Vector2Param
			{
				value = Vector2.zero
			});
			dictionary.Add("Orthographic", new BoolParam
			{
				value = false
			});
			Dictionary<string, Param> value10 = dictionary;
			dictionary = new Dictionary<string, Param>();
			dictionary.Add("FOV", new RangeParam
			{
				value = 60f,
				min = 20f,
				max = 100f
			});
			dictionary.Add("Distance", new RangeParam
			{
				value = 2f,
				min = 0f,
				max = 10f
			});
			dictionary.Add("RotationSpeedX", new RangeParam
			{
				value = 8f,
				min = 0f,
				max = 10f
			});
			dictionary.Add("RotationSpeedY", new RangeParam
			{
				value = 5f,
				min = 0f,
				max = 10f
			});
			dictionary.Add("RotationYMax", new RangeParam
			{
				value = 80f,
				min = 0f,
				max = 80f
			});
			dictionary.Add("RotationYMin", new RangeParam
			{
				value = -80f,
				min = -80f,
				max = 0f
			});
			dictionary.Add("TargetOffset", new Vector3Param
			{
				value = Vector3.zero
			});
			dictionary.Add("FollowCoef", new RangeParam
			{
				value = 0f,
				min = 0f,
				max = 10f
			});
			dictionary.Add("Spring", new RangeParam
			{
				value = 0f,
				min = 0f,
				max = 1f
			});
			dictionary.Add("DefaultYRotation", new RangeParam
			{
				value = 0f,
				min = -80f,
				max = 80f
			});
			dictionary.Add("AutoYRotation", new RangeParam
			{
				value = 0f,
				min = 0f,
				max = 1f
			});
			dictionary.Add("DeadZone", new Vector2Param
			{
				value = Vector2.zero
			});
			dictionary.Add("Orthographic", new BoolParam
			{
				value = false
			});
			Dictionary<string, Param> value11 = dictionary;
			dictionary = new Dictionary<string, Param>();
			dictionary.Add("FOV", new RangeParam
			{
				value = 60f,
				min = 20f,
				max = 100f
			});
			dictionary.Add("Distance", new RangeParam
			{
				value = 2f,
				min = 0f,
				max = 10f
			});
			dictionary.Add("RotationSpeedX", new RangeParam
			{
				value = 8f,
				min = 0f,
				max = 10f
			});
			dictionary.Add("RotationSpeedY", new RangeParam
			{
				value = 5f,
				min = 0f,
				max = 10f
			});
			dictionary.Add("RotationYMax", new RangeParam
			{
				value = 50f,
				min = 0f,
				max = 80f
			});
			dictionary.Add("RotationYMin", new RangeParam
			{
				value = -50f,
				min = -80f,
				max = 0f
			});
			dictionary.Add("TargetOffset", new Vector3Param
			{
				value = Vector3.zero
			});
			dictionary.Add("FollowCoef", new RangeParam
			{
				value = 0f,
				min = 0f,
				max = 10f
			});
			dictionary.Add("Spring", new RangeParam
			{
				value = 0f,
				min = 0f,
				max = 1f
			});
			dictionary.Add("DefaultYRotation", new RangeParam
			{
				value = 0f,
				min = -80f,
				max = 80f
			});
			dictionary.Add("AutoYRotation", new RangeParam
			{
				value = 0f,
				min = 0f,
				max = 1f
			});
			dictionary.Add("DeadZone", new Vector2Param
			{
				value = Vector2.zero
			});
			dictionary.Add("Orthographic", new BoolParam
			{
				value = false
			});
			Dictionary<string, Param> value12 = dictionary;
			Params = new Dictionary<string, Dictionary<string, Param>>
			{
				{
					"Default",
					value
				},
				{
					"Crouch",
					value2
				},
				{
					"Aim",
					value3
				},
				{
					"Sprint",
					value5
				},
				{
					"Ragdoll",
					value6
				},
				{
					"MeleeAim",
					value4
				},
				{
					"WallClimb",
					value7
				},
				{
					"BigFoot",
					value8
				},
				{
					"MechSpider",
					value9
				},
				{
					"SuperFly",
					value10
				},
				{
					"SuperFlySprint",
					value11
				},
				{
					"SuperFlyNearWalls",
					value12
				}
			};
			Transitions = new Dictionary<string, float>();
			foreach (KeyValuePair<string, Dictionary<string, Param>> param in Params)
			{
				Transitions.Add(param.Key, 0.25f);
			}
			Deserialize(base.DefaultConfigPath);
			base.LoadDefault();
		}

		public override void Awake()
		{
			base.Awake();
			LoadDefault();
		}
	}
}
