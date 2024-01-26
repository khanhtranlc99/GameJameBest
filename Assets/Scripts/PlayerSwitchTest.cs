using Game.Character.CharacterController;
using UnityEngine;

public class PlayerSwitchTest : MonoBehaviour
{
	public GameObject NewPlayerPref;

	[InspectorButton("StartTest")]
	public bool testStart;

	[InspectorButton("StopTest")]
	public bool testStop;

	public void StartTest()
	{
		PlayerManager.Instance.SwitchPlayer(NewPlayerPref);
	}

	public void StopTest()
	{
		PlayerManager.Instance.ResetPlayer();
	}
}
