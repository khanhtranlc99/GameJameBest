using Game.GlobalComponent;
using UnityEngine;

namespace Game.Managers
{
	public class GameManager : MonoBehaviour
	{
		public static bool ShowDebugs;

		public bool IsTransformersGame;

		public ControlsType[] TransformationTypes;

		private static GameManager instance;

		public static GameManager Instance
		{
			get
			{
				if (instance == null)
				{
					instance = UnityEngine.Object.FindObjectOfType<GameManager>();
				}
				return instance;
			}
		}

		private void Awake()
		{
			if (instance == null)
			{
				instance = this;
			}
		}
	}
}
