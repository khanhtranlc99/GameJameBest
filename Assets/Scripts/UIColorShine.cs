using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
[RequireComponent(typeof(MaskableGraphic))]
public sealed class UIColorShine : UISpriteColorBase
{
	[SerializeField]
	private string m_LocationPropertyName = "_ShineLocation";

	private int shineLocationParameterID;

	[SerializeField]
	private string m_ShineWidthPropertyName = "_ShineWidth";

	private int shineWidthParameterID;

	[SerializeField]
	private string m_EmissionPropertyName = "_EmissionGain";

	private int shineEmissionParameterID;

	public float shinePositon;

	public float shineWidth = 0.01f;

	public float shineEmission = 0.3f;

	protected override void Initialize()
	{
		shineLocationParameterID = Shader.PropertyToID(m_LocationPropertyName);
		shineWidthParameterID = Shader.PropertyToID(m_ShineWidthPropertyName);
		shineEmissionParameterID = Shader.PropertyToID(m_EmissionPropertyName);
		base.Initialize();
	}

	protected override void UpdateShader()
	{
		uispriteRenderer.material.SetFloat(shineLocationParameterID, shinePositon);
		uispriteRenderer.material.SetFloat(shineWidthParameterID, shineWidth);
		uispriteRenderer.material.SetFloat(shineEmissionParameterID, shineEmission);
	}
}
