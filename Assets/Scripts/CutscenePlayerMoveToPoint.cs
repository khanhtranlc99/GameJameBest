using Game.Character;
using Game.Character.CharacterController;
using Game.Character.Input;
using Game.GlobalComponent;
using System.Collections;
using UnityEngine;

public class CutscenePlayerMoveToPoint : Cutscene
{
	public float MoveSpeed = 1f;

	public Transform StartPosition;

	public Transform EndPosition;

	public float DistanceToPoint = 0.5f;

	private Transform playerTransform;

	private Player player;

	private UnityEngine.AI.NavMeshAgent agent;

	private InputFilter velocityFilter;

	public override void StartScene()
	{
		IsPlaying = true;
		velocityFilter = new InputFilter(10, 1f);
		playerTransform = PlayerInteractionsManager.Instance.Player.transform;
		player = PlayerInteractionsManager.Instance.Player;
		StartCoroutine(MoveToPoint());
	}

	private IEnumerator MoveToPoint()
	{
		while (PlayerInteractionsManager.Instance.inVehicle || PlayerManager.Instance.IsGettingInOrOut)
		{
			yield return new WaitForSeconds(0.25f);
		}
		player.enabled = false;
		agent = player.agent;
		agent.enabled = true;
		while (Vector3.Distance(EndPosition.position, playerTransform.position) > DistanceToPoint && IsPlaying)
		{
			if (agent.enabled)
			{
				agent.SetDestination(EndPosition.position);
				MetroManager.Instance.CurrentMetro.RemoveObstructive();
			}
			yield return new WaitForSeconds(0.25f);
		}
		EndScene(isCheck: true);
	}

	private Vector3 SmoothVelocityVector(Vector3 v)
	{
		velocityFilter.AddSample(new Vector2(v.x, v.z));
		Vector2 value = velocityFilter.GetValue();
		return new Vector3(value.x, 0f, value.y).normalized;
	}

	private void Update()
	{
		if (IsPlaying && !(player == null))
		{
			player.GetComponent<AnimationController>().Move(new Game.Character.CharacterController.Input
			{
				camMove = SmoothVelocityVector((agent.steeringTarget - playerTransform.position).normalized) * MoveSpeed,
				AttackState = new AttackState(),
				lookPos = EndPosition.position + Vector3.up,
				crouch = false,
				inputMove = Vector3.zero,
				jump = false,
				smoothAimRotation = false,
				aimTurn = false
			});
		}
	}

	public override void EndScene(bool isCheck)
	{
		base.EndScene(isCheck);
		agent.enabled = false;
		player.enabled = true;
		StopAllCoroutines();
	}
}
