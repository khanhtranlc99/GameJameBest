using Game.Character.CharacterController;
using UnityEngine;

public class PlayerKiller : MonoBehaviour
{
	public GameObject KillButton;

	public KeyCode KillKey = KeyCode.KeypadMinus;

	private void Awake()
	{
		if (Debug.isDebugBuild)
		{
			KillButton.SetActive(value: true);
		}
		else
		{
			KillButton.SetActive(value: false);
		}
	}

	private void Update()
	{
		if (UnityEngine.Input.GetKeyDown(KillKey))
		{
			KillPlayer();
		}
	}

	public void KillPlayer()
	{
		PlayerManager.Instance.PlayerAsTarget.Die();
	}
}
