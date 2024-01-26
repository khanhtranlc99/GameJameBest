using Game.Character;
using Game.Character.CharacterController;
using System;
using RopeHeroViceCity.UI_Features.UI_InGame.Scripts;
using UnityEngine;

namespace Game.GlobalComponent
{
	public class CutsceneManager : MonoBehaviour
	{
		[Serializable]
		public class Frame
		{
			public string Name;

			public Cutscene[] Scenes;

			public float Time = 10f;

			public bool EndMainAction;
		}

		public delegate void Callback();

		public Frame[] Scenes;

		[HideInInspector]
		public bool ForcedStop;

		private Callback callback;

		private Callback fCallback;

		private int sceneIndex;

		private bool init;

		private Animator prevPanel;

		private Player player;

		private float timer;

		public bool Inited => init;

		public int CurrentIndex()
		{
			return sceneIndex;
		}

		public void Init(Callback cb, Callback forcedCallback)
		{
			player = PlayerInteractionsManager.Instance.Player;
			UI_InGame.Instance.SetStateCutScene(true);
			//CutscenePanel.Instance.Open();
			sceneIndex = 0;
			callback = cb;
			fCallback = forcedCallback;
			init = true;
			StartMove();
			Player obj = player;
			obj.DiedEvent = (HitEntity.AliveStateChagedEvent)Delegate.Combine(obj.DiedEvent, new HitEntity.AliveStateChagedEvent(StopAllFrame));
		}

		public void CheckFrame(Cutscene scene)
		{
			if (scene.IsMainAction && Scenes[sceneIndex].EndMainAction)
			{
				StopFrame();
			}
			bool flag = true;
			Cutscene[] scenes = Scenes[sceneIndex].Scenes;
			foreach (Cutscene cutscene in scenes)
			{
				if (cutscene.IsPlaying)
				{
					flag = false;
				}
			}
			if (flag)
			{
				StopFrame();
			}
		}

		public void StartMove()
		{
			Cutscene[] scenes = Scenes[sceneIndex].Scenes;
			foreach (Cutscene cutscene in scenes)
			{
				cutscene.Init(this);
				cutscene.StartScene();
			}
			timer = Scenes[sceneIndex].Time;
		}

		private void StopScene()
		{
			if (init)
			{
				//CutscenePanel.Instance.Close();
				UI_InGame.Instance.SetStateCutScene(false);
				callback();
			}
			init = false;
		}

		private void StopAllFrame()
		{
			if (init)
			{
				UI_InGame.Instance.SetStateCutScene(false);
			}
			init = false;
			fCallback();
			for (int i = 0; i < Scenes.Length; i++)
			{
				Cutscene[] scenes = Scenes[i].Scenes;
				foreach (Cutscene cutscene in scenes)
				{
					if (cutscene.IsPlaying)
					{
						cutscene.EndScene(isCheck: false);
					}
				}
			}
		}

		private void Update()
		{
			if (init)
			{
				timer -= Time.deltaTime;
				if (timer <= 0f)
				{
					StopAllFrame();
				}
			}
		}

		private void StopFrame()
		{
			Cutscene[] scenes = Scenes[sceneIndex].Scenes;
			foreach (Cutscene cutscene in scenes)
			{
				if (cutscene.IsPlaying)
				{
					cutscene.EndScene(isCheck: false);
				}
			}
			sceneIndex++;
			if (sceneIndex >= Scenes.Length)
			{
				sceneIndex--;
				StopScene();
			}
			else
			{
				StartMove();
			}
		}
	}
}
