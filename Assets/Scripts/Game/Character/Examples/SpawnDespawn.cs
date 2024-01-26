using UnityEngine;

namespace Game.Character.Examples
{
	public class SpawnDespawn : MonoBehaviour
	{
		public GameObject CharacterControllerPrefab;

		public GameObject CharacterControllerCurrent;

		private CameraManager cameraManager;

		public Vector3 position;

		private void Start()
		{
			cameraManager = CameraManager.Instance;
		}

		public void Spawn()
		{
			CharacterControllerCurrent = (UnityEngine.Object.Instantiate(CharacterControllerPrefab, position, Quaternion.identity) as GameObject);
			if (CharacterControllerCurrent != null)
			{
				cameraManager.SetCameraTarget(CharacterControllerCurrent.transform);
				cameraManager.SetMode(cameraManager.ActivateModeOnStart);
			}
		}

		public void Despawn()
		{
			position = CharacterControllerCurrent.transform.position;
			UnityEngine.Object.Destroy(CharacterControllerCurrent.gameObject);
			cameraManager.SetMode(null);
		}
	}
}
