using Game.Character.Stats;
using System.Linq;
using UnityEngine;

public class WorldStatBar : CharacterStatDisplay
{
	private const string ofHundredString = "/100";

	public Transform CamTrf;

	public Transform SpriteFrontTrf;

	public TextMesh TextMesh;

	private float percentage;

	public GameObject[] Children;

	private bool isSetActive = true;

	protected override void Start()
	{
		base.Start();
		if (CamTrf == null)
		{
			CamTrf = Camera.main.transform;
		}
		if (TextMesh == null)
		{
			TextMesh = GetComponentInChildren<TextMesh>();
		}
		if (Children == null || Children.Length <= 0)
		{
			Children = (from t in GetComponentsInChildren<Transform>()
				where t.parent.Equals(base.transform)
				select t.gameObject).ToArray();
		}
	}

	protected override void UpdateDisplayValue()
	{
		if (current < max && current > 0f)
		{
			SetStatBarActive(on: true);
			percentage = current / max;
			if (SpriteFrontTrf != null)
			{
				Transform spriteFrontTrf = SpriteFrontTrf;
				float x = percentage;
				Vector3 localScale = SpriteFrontTrf.localScale;
				float y = localScale.y;
				Vector3 localScale2 = SpriteFrontTrf.localScale;
				spriteFrontTrf.localScale = new Vector3(x, y, localScale2.z);
			}
			if (TextMesh != null)
			{
				TextMesh.text = (percentage * 100f).ToString("F0") + "/100";
			}
			base.transform.rotation = CamTrf.rotation;
		}
		else
		{
			SetStatBarActive(on: false);
		}
	}

	private void SetStatBarActive(bool on)
	{
		if (isSetActive != on)
		{
			GameObject[] children = Children;
			foreach (GameObject gameObject in children)
			{
				gameObject.gameObject.SetActive(on);
			}
			isSetActive = on;
		}
	}

	public override void OnChanged(float amount)
	{
	}
}
