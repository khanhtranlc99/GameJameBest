using UnityEngine;

[RequireComponent(typeof(UIColorShine))]
public class ShineAnimation : MonoBehaviour
{
	public AnimationCurve animCurve;

	[Tooltip("Animation speed multiplier")]
	public float speed = 1f;

	private float t;

	private UIColorShine m_UIColorShine;

	private void OnEnable()
	{
		t = 0f;
		m_UIColorShine = GetComponent<UIColorShine>();
	}

	private void Update()
	{
		t += speed * Time.unscaledDeltaTime;
		m_UIColorShine.shinePositon = animCurve.Evaluate(t);
	}
}
