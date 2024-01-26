using Game.Character;
using Game.Character.CameraEffects;
using Game.Character.CharacterController;
using Game.Character.Input;
using Game.Character.Stats;
using Game.Enemy;
using Game.GlobalComponent;
using Game.Vehicle;
using System.Collections;
using UnityEngine;

public class Transformer : MonoBehaviour
{
	private const float relativeSpeedForHitSmallDynamic = 7f;

	private const float relativeSpeedForStrongHit = 30f;

	private const float relativeSpeedForHitBigDynamic = 2f;

	public bool DebugLog;

	[Space(10f)]
	public TransformerForm currentForm;

	public GameObject robotModel;

	public GameObject transformationModel;

	public bool transformating;

	public float timeToTPose = 0.3f;

	public float timeToTransformation = 3.54f;

	public GameObject carPrefab;

	public Vector3 carOffset;

	public BoxCollider transformationCollider;

	public bool isPlayer;

	public float animationDirection = 1f;

	public LayerMask SpaceCheckMask;

	private Animator mainAnimator;

	private Animator transformAnimator;

	private AnimationController mainController;

	private BaseNPC baseNpc;

	private Vector3 trColSize;

	private Vector3 trColCenter;

	private HitEntity owner;

	private DontGoThroughThings dontGoThroughThings;

	private GameObject car;

	private float transformationStartTime;

	private Rigidbody rb;

	private bool inited;

	private float hitTimer = 1f;

	private int bigDynamicLayer;

	private float transformationTryTime;

	private float transformationTryCD = 1f;

	private void Start()
	{
		bigDynamicLayer = 1 << LayerMask.NameToLayer("Terrain");
		Init();
	}

	public void Init(TransformerForm form = TransformerForm.Robot)
	{
		if (!inited)
		{
			if (DebugLog)
			{
				UnityEngine.Debug.Log("Заинициэйтился");
			}
			robotModel.SetActive(value: true);
			currentForm = form;
			mainAnimator = GetComponent<Animator>();
			transformAnimator = transformationModel.GetComponent<Animator>();
			transformationModel.SetActive(value: false);
			mainController = GetComponent<AnimationController>();
			baseNpc = GetComponent<BaseNPC>();
			trColSize = transformationCollider.size;
			trColCenter = transformationCollider.center;
			transformationCollider.size = Vector3.zero;
			transformationCollider.gameObject.SetActive(value: false);
			owner = GetComponent<HitEntity>();
			dontGoThroughThings = GetComponent<DontGoThroughThings>();
			rb = GetComponent<Rigidbody>();
			inited = true;
		}
	}

	public void DeInit()
	{
		StopAllCoroutines();
		transformating = false;
		transformationCollider.gameObject.SetActive(value: false);
		transformationModel.SetActive(value: false);
		inited = false;
	}

	private void Update()
	{
		if (isPlayer && InputManager.Instance.GetInput(InputType.Action, defaultValue: false) && !transformating)
		{
			Transform();
		}
		if ((bool)mainAnimator)
		{
			mainAnimator.applyRootMotion = !transformating;
		}
	}

	private void FixedUpdate()
	{
		if (transformating && currentForm == TransformerForm.Robot)
		{
			transformationCollider.size = Vector3.Lerp(Vector3.zero, trColSize, (Time.time - transformationStartTime) / timeToTransformation);
			transformationCollider.center = Vector3.Lerp(Vector3.zero, trColCenter, (Time.time - transformationStartTime) / timeToTransformation);
		}
	}

	public void Transform()
	{
		Transform(null, null);
	}

	public void Transform(GameObject existingCar, HitEntity target)
	{
		if (transformationTryTime + transformationTryCD > Time.time || transformating)
		{
			return;
		}
		transformationTryTime = Time.time;
		switch (currentForm)
		{
		case TransformerForm.Robot:
			if (EnoughSpaceForCar())
			{
				StartCoroutine(TransformToCarCorutine());
			}
			break;
		case TransformerForm.Car:
			if (EnoughSpaceForRobot())
			{
				StartCoroutine(TransformToRobotCorutine(existingCar, target));
			}
			break;
		}
		if (transformating)
		{
			PointSoundManager.Instance.PlaySoundAtPoint(base.transform.position, TypeOfSound.TransformationMain);
		}
		else
		{
			PointSoundManager.Instance.PlaySoundAtPoint(base.transform.position, TypeOfSound.Fail);
		}
	}

	public IEnumerator TransformToCarCorutine()
	{
		transformating = true;
		if (isPlayer)
		{
			mainController.useIKLook = false;
			CameraManager.Instance.GetCurrentCameraMode().SetCameraConfigMode("Default");
			CameraEffect effect = EffectManager.Instance.Create(Type.SprintShake);
			effect.Stop();
		}
		yield return new WaitForSeconds(timeToTPose);
		mainAnimator.enabled = false;
		transformationStartTime = Time.time;
		transformationCollider.gameObject.SetActive(value: true);
		transformationCollider.size = Vector3.zero;
		rb.isKinematic = true;
		rb.velocity = Vector3.zero;
		robotModel.SetActive(value: false);
		transformationModel.SetActive(value: true);
		transformAnimator.SetFloat("SpeedMultipler", 0f - animationDirection);
		transformAnimator.SetBool("Transformating", value: true);
		yield return new WaitForSeconds(timeToTransformation);
		transformAnimator.SetBool("Transformating", value: false);
		transformationModel.SetActive(value: false);
		currentForm = TransformerForm.Car;
		transformationCollider.gameObject.SetActive(value: false);
		if (isPlayer)
		{
			mainController.ExitAnimEnd();
		}
		car = PoolManager.Instance.GetFromPool(carPrefab);
		car.SetActive(value: false);
		car.transform.position = base.transform.position + base.transform.right * carOffset.x + base.transform.up * carOffset.y + base.transform.forward * carOffset.z;
		car.transform.rotation = base.transform.rotation;
		VehicleStatus carVehicleStatus = car.GetComponent<DrivableVehicle>().GetVehicleStatus();
		carVehicleStatus.Health = owner.Health;
		carVehicleStatus.Defence.Set(owner.Defence);
		carVehicleStatus.Faction = owner.Faction;
		car.SetActive(value: true);
		car.GetComponent<Rigidbody>().velocity = rb.velocity;
		base.transform.parent = car.transform;
		if (isPlayer)
		{
			PlayerInteractionsManager.Instance.InstantEnterVehicle(car.GetComponent<DrivableVehicle>());
		}
		transformating = false;
		if (isPlayer)
		{
			mainController.ExitAnimStart();
		}
		else
		{
			baseNpc.CurrentController.enabled = false;
		}
	}

	public IEnumerator TransformToRobotCorutine(GameObject existingCar = null, HitEntity target = null)
	{
		transformating = true;
		if (existingCar != null)
		{
			Init();
			car = existingCar;
		}
		rb.isKinematic = true;
		if (isPlayer)
		{
			mainController.useIKLook = false;
		}
		else
		{
			baseNpc.CurrentController.enabled = false;
		}
		robotModel.SetActive(value: false);
		transformationModel.SetActive(value: true);
		transformAnimator.SetFloat("SpeedMultipler", animationDirection);
		transformAnimator.SetBool("Transformating", value: true);
		if (isPlayer)
		{
			mainController.ExitAnimStart();
			if ((bool)dontGoThroughThings)
			{
				dontGoThroughThings.SetPrevPostion(base.transform.position);
			}
			PlayerInteractionsManager.Instance.InstantExitVehicle();
		}
		else
		{
			base.transform.parent = null;
		}
		base.transform.position = car.transform.position - (base.transform.right * carOffset.x + base.transform.up * carOffset.y + base.transform.forward * carOffset.z);
		base.transform.rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(car.transform.forward, Vector3.up), Vector3.up);
		car.GetComponentInChildren<VehicleStatus>().Health = new CharacterStat();
		PoolManager.Instance.ReturnToPool(car);
		if (DebugLog)
		{
			UnityEngine.Debug.Log("Начал проигрывать трансформацию");
		}
		yield return new WaitForSeconds(timeToTransformation);
		if (DebugLog)
		{
			UnityEngine.Debug.Log("Закончил проигрывать трансформацию");
		}
		transformAnimator.SetBool("Transformating", value: false);
		robotModel.SetActive(value: true);
		transformationModel.SetActive(value: false);
		mainAnimator.enabled = true;
		currentForm = TransformerForm.Robot;
		rb.isKinematic = false;
		if (isPlayer)
		{
			mainController.useIKLook = true;
			mainController.ExitAnimEnd();
		}
		else
		{
			baseNpc.CurrentController.enabled = true;
			if (target != null)
			{
				BaseControllerNPC _;
				baseNpc.ChangeController(BaseNPC.NPCControllerType.Smart, out _);
				((SmartHumanoidController)baseNpc.CurrentController).AddTarget(target);
				((SmartHumanoidController)baseNpc.CurrentController).InitBackToDummyLogic();
			}
		}
		transformating = false;
	}

	public void ResetToRobotForm()
	{
		if (isPlayer)
		{
			mainController.ExitAnimStart();
			if ((bool)dontGoThroughThings)
			{
				dontGoThroughThings.SetPrevPostion(base.transform.position);
			}
			PlayerInteractionsManager.Instance.InstantExitVehicle();
		}
		else
		{
			base.transform.parent = null;
		}
		base.transform.position = car.transform.position - (base.transform.right * carOffset.x + base.transform.up * carOffset.y + base.transform.forward * carOffset.z);
		base.transform.rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(car.transform.forward, Vector3.up), Vector3.up);
		car.GetComponentInChildren<VehicleStatus>().Health = new CharacterStat();
		PoolManager.Instance.ReturnToPool(car);
		mainAnimator.enabled = true;
		currentForm = TransformerForm.Robot;
		if (isPlayer)
		{
			mainController.ExitAnimEnd();
		}
		else
		{
			baseNpc.CurrentController.enabled = true;
		}
	}

	public virtual void OnCollisionEnter(Collision c)
	{
		Effects(c);
	}

	public void Effects(Collision c)
	{
		if (c.contacts.Length < 1 || c.gameObject.layer == DrivableVehicle.terrainLayerNumber)
		{
			return;
		}
		float num = Mathf.Abs(Vector3.Dot(c.relativeVelocity, c.contacts[0].normal));
		if (c.gameObject.layer == DrivableVehicle.chatacterLayerNumber && num > 7f)
		{
			PointSoundManager.Instance.PlaySoundAtPoint(c.contacts[0].point, TypeOfSound.CarHitHuman);
		}
		else
		{
			if (c.gameObject.layer != DrivableVehicle.smallDynamicLayerNumber || !(num > 7f))
			{
				return;
			}
			Rigidbody component = c.gameObject.GetComponent<Rigidbody>();
			if ((bool)component)
			{
				float mass = component.mass;
				if (mass < 5f && hitTimer < 0f)
				{
					hitTimer = 1f;
					PointSoundManager.Instance.PlaySoundAtPoint(c.contacts[0].point, TypeOfSound.CarHitHuman);
				}
			}
			else
			{
				PointSoundManager.Instance.PlaySoundAtPoint(c.contacts[0].point, (!(num < 30f)) ? TypeOfSound.StrongCarHit : TypeOfSound.CarHit);
			}
		}
	}

	private bool EnoughSpaceForCar()
	{
		if (currentForm == TransformerForm.Car)
		{
			return true;
		}
		LayerMask spaceCheckMask = SpaceCheckMask;
		spaceCheckMask = ((int)spaceCheckMask & ~bigDynamicLayer);
		Vector3 center = transformationCollider.transform.position + trColCenter;
		Vector3 localScale = transformationCollider.transform.localScale;
		float x = localScale.x * trColSize.x;
		Vector3 localScale2 = transformationCollider.transform.localScale;
		float y = localScale2.y * trColSize.y;
		Vector3 localScale3 = transformationCollider.transform.localScale;
		Vector3 halfExtents = new Vector3(x, y, localScale3.z * trColSize.z) / 2f;
		return !Physics.CheckBox(center, halfExtents, transformationCollider.transform.rotation, spaceCheckMask);
	}

	private bool EnoughSpaceForRobot()
	{
		if (currentForm == TransformerForm.Robot)
		{
			return true;
		}
		Transform parent = base.transform.parent;
		if (!parent)
		{
			return true;
		}
		Vector3 a = parent.position - (base.transform.right * carOffset.x + base.transform.up * carOffset.y + base.transform.forward * carOffset.z);
		return !Physics.Raycast(a + Vector3.up * 0.2f, Vector3.up, 4f, SpaceCheckMask);
	}
}
