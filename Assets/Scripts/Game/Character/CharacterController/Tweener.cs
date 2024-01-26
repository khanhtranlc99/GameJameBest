using Game.Character.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Character.CharacterController
{
	internal class Tweener : MonoBehaviour
	{
		private abstract class Tween
		{
			public Transform Transform;

			public float Time;

			public float Timeout;

			public OnFinish callback;

			public abstract void Update();
		}

		private class TweenPos : Tween
		{
			public Vector3 TargetPos;

			public Vector3 StartPos;

			public override void Update()
			{
				Timeout += UnityEngine.Time.deltaTime;
				float t = Timeout / Time;
				Transform.position = Vector3.Lerp(StartPos, TargetPos, t);
			}
		}

		private class TweenRot : Tween
		{
			public Quaternion TargetRot;

			public Quaternion StartRot;

			public override void Update()
			{
				Timeout += UnityEngine.Time.deltaTime;
				float t = Timeout / Time;
				Transform.rotation = Quaternion.Slerp(StartRot, TargetRot, t);
			}
		}

		public delegate void OnFinish();

		private static Tweener instance;

		private List<Tween> tweens;

		private List<Tween> finishedTweens;

		public static Tweener Instance
		{
			get
			{
				if (!instance)
				{
					instance = CameraInstance.CreateInstance<Tweener>("Tweener");
				}
				return instance;
			}
		}

		public void MoveTo(Transform trans, Vector3 targetPos, float time, OnFinish onFinish = null)
		{
			tweens.Add(new TweenPos
			{
				Transform = trans,
				StartPos = trans.position,
				TargetPos = targetPos,
				Time = time,
				Timeout = 0f,
				callback = onFinish
			});
		}

		public void RotateTo(Transform trans, Quaternion rot, float time, OnFinish onFinish = null)
		{
			tweens.Add(new TweenRot
			{
				Transform = trans,
				StartRot = trans.rotation,
				TargetRot = rot,
				Time = time,
				Timeout = 0f,
				callback = onFinish
			});
		}

		private void Awake()
		{
			instance = this;
			tweens = new List<Tween>();
			finishedTweens = new List<Tween>();
		}

		private void FixedUpdate()
		{
			foreach (Tween tween in tweens)
			{
				tween.Update();
				if (tween.Timeout >= tween.Time)
				{
					if (tween.callback != null)
					{
						tween.callback();
					}
					finishedTweens.Add(tween);
				}
			}
			foreach (Tween finishedTween in finishedTweens)
			{
				tweens.Remove(finishedTween);
			}
			finishedTweens.Clear();
		}
	}
}
