using Game.Character;
using Game.Character.CharacterController;
using Game.Character.Modes;
using Game.GlobalComponent;
using Game.UI;
using System;
using UnityEngine;

namespace Game.Vehicle
{
	public class VehicleController : MonoBehaviour
	{
		private const float MagicConstant = 0.083f;

		[Header("Configurables")]
		public Game.Character.Modes.Type CameraModeType = Game.Character.Modes.Type.ThirdPersonVehicle;

		public string ConfigName = "Default";

		public VehicleType VehicleType;

		[Header("Non-Configurables")]
		public AudioSource EngineAudioSource;

		[Header("Non-Configurables")]
		public AudioSource BrakeAudioSource;

		protected Player player;

		protected bool IsInitialized;

		protected Rigidbody MainRigidbody;

		protected DrivableVehicle DrivableVehicle;

		protected Vector3 primordialCenterOfMass;

		[HideInInspector]
		public VehicleSpecific VehicleSpecific;

		protected bool engineEnabled = true;

		protected virtual VehicleType GetVehicleType()
		{
			return VehicleType;
		}

		public virtual void Init(DrivableVehicle drivableVehicle)
		{
			if (drivableVehicle.VehicleSpecificPrefab != null)
			{
				VehicleSpecific = PoolManager.Instance.GetFromPool(drivableVehicle.VehicleSpecificPrefab);
				GameObject gameObject = VehicleSpecific.gameObject;
				gameObject.transform.parent = base.transform;
				gameObject.transform.localPosition = Vector3.zero;
				gameObject.transform.localRotation = Quaternion.identity;
			}
			MainRigidbody = drivableVehicle.MainRigidbody;
			primordialCenterOfMass = MainRigidbody.centerOfMass;
			DrivableVehicle = drivableVehicle;
			Controls.SetControlsByVehicle(GetVehicleType());
			SetCameraFollowTarget(drivableVehicle);
			player = PlayerInteractionsManager.Instance.Player;
			drivableVehicle.ApplyCenterOfMass(drivableVehicle.VehiclePoints.CenterOfMass);
			if (EngineAudioSource != null && drivableVehicle.SoundsPrefab != null && drivableVehicle.SoundsPrefab.EngineSounds.Length > 0 && drivableVehicle.SoundsPrefab.EngineSounds[0] != null)
			{
				EngineAudioSource.clip = ((CarSpecific)drivableVehicle.VehicleSpecificPrefab).EngineSounds[0];
				EngineAudioSource.Play();
			}
			IsInitialized = true;
		}

		public void SetInitialization(bool value)
		{
			IsInitialized = value;
		}

		public virtual void Animate(DrivableVehicle drivableVehicle)
		{
		}

		public virtual void DeInit()
		{
			DeInit(delegate
			{
			});
		}

		protected virtual void FixedUpdate()
		{
			if ((bool)player)
			{
				player.stats.stamina.DoFixedUpdate();
				player.Health.DoFixedUpdate();
			}
			if (DrivableVehicle.WaterSensor.InWater && DrivableVehicle.DeepInWater)
			{
				Drowning();
				if ((bool)player)
				{
					DangerIndicator.Instance.Activate("You are drowning.");
					RadioManager.Instance.DisableRadio();
				}
			}
			else if(!PlayerInteractionsManager.Instance.sitInVehicle)
			{
				DisableEngine();
			}
			else
			{
				EnableEngine();
				if ((bool)player)
				{
					DangerIndicator.Instance.Deactivate();
				}
			}
		}

		public virtual void DeInit(Action callbackAfterDeInit)
		{
			IsInitialized = false;
			if (EngineAudioSource != null)
			{
				EngineAudioSource.Stop();
				EngineAudioSource.clip = null;
			}
			MainRigidbody = null;
			DrivableVehicle = null;
			if (VehicleSpecific != null)
			{
				PoolManager.Instance.ReturnToPool(VehicleSpecific);
				VehicleSpecific = null;
			}
			callbackAfterDeInit?.Invoke();
		}

		public virtual void StopVehicle(bool inMoment = false)
		{
		}

		public virtual void Particles(Collision collision)
		{
		}

		public virtual void EnableEngine()
		{
			if (!engineEnabled)
			{
				engineEnabled = true;
			}
		}

		public virtual void DisableEngine()
		{
			if (engineEnabled)
			{
				engineEnabled = false;
			}
		}

		public virtual bool EnabledToExit()
		{
			return true;
		}

		public virtual void DropFrom()
		{
		}

		protected virtual void Drowning()
		{
			DisableEngine();
		}

		protected virtual void SetCameraFollowTarget(DrivableVehicle drivableVehicle)
		{
			CameraManager.Instance.SetCameraTarget(drivableVehicle.transform);
			CameraManager.Instance.SetMode(CameraModeType);
			CameraManager.Instance.GetCurrentCameraMode().SetCameraConfigMode(ConfigName);
		}
	}
}
