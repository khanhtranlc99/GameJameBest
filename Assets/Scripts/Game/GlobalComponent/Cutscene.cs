using UnityEngine;

namespace Game.GlobalComponent
{
	public abstract class Cutscene : MonoBehaviour
	{
		public string Name;

		public bool IsMainAction;

		public bool IsPlaying;

		protected CutsceneManager mainManager;

		public virtual void Init(CutsceneManager manager)
		{
			mainManager = manager;
		}

		public virtual void StartScene()
		{
			IsPlaying = true;
		}

		public virtual void EndScene(bool isCheck = true)
		{
			IsPlaying = false;
			if (isCheck)
			{
				mainManager.CheckFrame(this);
			}
		}
	}
}
