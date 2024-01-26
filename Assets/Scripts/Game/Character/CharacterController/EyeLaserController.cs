using System;
using System.Collections;
using System.Collections.Generic;
using Game.Character;
using Game.Character.CharacterController;
using Game.Character.CharacterController.Enums;
using Game.GlobalComponent;
using UnityEngine;
using UnityEngine.Serialization;

public class EyeLaserController : MonoBehaviour
{
    public Transform startPointTransform;
    private Player player;
    public float cooldownTime = .02f;
   // private Vector3 movingTargetOffset;
    private float cooldownStartTime;
    [FormerlySerializedAs("maxRopeXZDistance")] public float maxLaserDistance = 100f;
    public LayerMask WallsLayerMask;
    private IComparer<RaycastHit> rayHitComparer;
    public LayerMask DoNotShootThroughThisLayerMask;
    private RaycastHit target;
    public LayerMask TargetLayerMask;
    public LineRenderer lineRender;
    private Rigidbody rbody;
    private bool isEnableLaser;
    public GameObject objFX;
	public GameObject objFXEndPoint;
	private void Awake()
    {
        player = GetComponent<Player>();
        if (player != null)
        {
	        player.PlayerStartUsingEyeLaser += StartUsingLazer;
	        player.PlayerStoptUsingEyeLaser += StopUsingLaser;
        }
        rayHitComparer = new RayHitComparer();
        rbody = GetComponent<Rigidbody>();
    }

    private void OnDestroy()
    {
	    if (player != null)
	    {
		    player.PlayerStartUsingEyeLaser -= StartUsingLazer;
		    player.PlayerStoptUsingEyeLaser -= StopUsingLaser;
	    }
    }

    private void StartUsingLazer()
    {
		
		if (!player.CanUseSkill) return;
	    if (!isEnableLaser)
	    {
		    isEnableLaser = true;
		    lineRender.enabled = true;
		    objFX.SetActive(true);
	    }
	    lineRender.SetPosition(0,startPointTransform.position);
		objFXEndPoint.SetActive(true);
		Ray ray = CameraManager.Instance.UnityCamera.ScreenPointToRay(TargetManager.Instance.RopeAimPosition);
	    float num = Vector3.Dot(ray.direction, Vector3.up);
	    float maxDistance = maxLaserDistance / (float)Math.Cos(Math.Asin(num));
	    RaycastHit[] array = Physics.RaycastAll(ray, maxDistance, (int)WallsLayerMask | (int)TargetLayerMask | (int)DoNotShootThroughThisLayerMask);
	    Array.Sort(array, rayHitComparer);
	    Vector3 direction = ray.direction;
	    float x = direction.x;
	    Vector3 direction2 = ray.direction;
	    rbody.transform.rotation = Quaternion.LookRotation(new Vector3(x, 0f, direction2.z), Vector3.up);
	    RaycastHit[] array2 = array;
	    for (var i = 0; i < array2.Length; i++)
	    {
		    RaycastHit raycastHit = array2[i];
			objFXEndPoint.transform.position = raycastHit.point;
			if (LayerInLayerMask(raycastHit.collider.transform.gameObject.layer, DoNotShootThroughThisLayerMask))
		    {
			    break;
		    }

		    if (raycastHit.collider.gameObject.CompareTag("Player")) continue;
		    target = raycastHit;
		    if (!LayerInLayerMask(target.collider.transform.gameObject.layer, TargetLayerMask)) continue;
		    PointSoundManager.Instance.PlaySoundAtPoint(base.transform.position, TypeOfSound.RopeShoot);
		   // movingTargetOffset = target.point - target.collider.transform.position;
		   lineRender.SetPosition(1,target.point);
		   if ((Time.time - cooldownStartTime > cooldownTime))
		   {
			   cooldownStartTime = Time.time;
			   var hitTarget = target.collider.gameObject.GetComponent<HitEntity>();
			   if (hitTarget != null)
			   {
				   hitTarget.OnHit(DamageType.Bullet,player,15,target.point,startPointTransform.forward);
					
					
			   }
		   }
		    //rope.ShootMovingTarget(target.collider.transform, movingTargetOffset, ropeExpandTime, ropeStraighteningTime);
		    //BeginDragStage();
		    return;
	    }
	    PointSoundManager.Instance.PlaySoundAtPoint(base.transform.position, TypeOfSound.RopeShoot);
	    lineRender.SetPosition(1,startPointTransform.position+ startPointTransform.forward*maxLaserDistance);
	   // rope.ShootFail(ray.direction, maxDistance, ropeExpandTime, ropeStraighteningTime);
    }

    private void StopUsingLaser()
    {
	    if(!isEnableLaser) return;
	    isEnableLaser = false;
	    lineRender.enabled = false;
	    objFX.SetActive(false);
		objFXEndPoint.SetActive(false);
	}
    private bool LayerInLayerMask(int layer, LayerMask mask)
    {
	    return (mask.value & (1 << layer)) == 1 << layer;
    }
}
