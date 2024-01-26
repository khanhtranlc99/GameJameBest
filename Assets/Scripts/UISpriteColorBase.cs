using UnityEngine;
using UnityEngine.UI;

public abstract class UISpriteColorBase : MonoBehaviour
{
	protected MaskableGraphic uispriteRenderer;

	public Shader m_Shader;

	private void OnEnable()
	{
		uispriteRenderer = base.gameObject.GetComponent<MaskableGraphic>();
		if (uispriteRenderer != null)
		{
			CreateMaterial();
			UISpriteColorBase component = base.gameObject.GetComponent<UISpriteColorBase>();
			Initialize();
		}
		else
		{
			UnityEngine.Debug.LogWarning($"'{GetType().ToString()}' without UISpriteRenderer, disabled.");
			base.enabled = false;
		}
	}

	private void OnDisable()
	{
		if (uispriteRenderer != null && uispriteRenderer.material != null && string.CompareOrdinal(uispriteRenderer.material.name, "UI/Default") != 0)
		{
			uispriteRenderer.material = null;
		}
	}

	private void Update()
	{
		if (uispriteRenderer == null)
		{
			uispriteRenderer = base.gameObject.GetComponent<MaskableGraphic>();
		}
		if (uispriteRenderer != null && uispriteRenderer.material != null)
		{
			UpdateShader();
		}
	}

	protected void CreateMaterial()
	{
		string text = GetType().ToString().Replace("UISpriteColorBase.", string.Empty);
		if (m_Shader == null)
		{
			UnityEngine.Debug.LogWarning(string.Format("Failed to load '{0}', {1} disabled.", "Shader", text));
			base.enabled = false;
			return;
		}
		if (!m_Shader.isSupported)
		{
			UnityEngine.Debug.LogWarning(string.Format("Shader '{0}' not supported, {1} disabled.", "Shader", text));
			base.enabled = false;
			return;
		}
		if (uispriteRenderer == null)
		{
			uispriteRenderer = base.gameObject.GetComponent<MaskableGraphic>();
		}
		bool flag = false;
		Color color = Color.white;
		Vector2 offset = Vector2.zero;
		Vector2 scale = Vector2.one;
		Vector2 offset2 = Vector2.zero;
		Vector2 scale2 = Vector2.one;
		bool flag2 = false;
		if (uispriteRenderer.material != null)
		{
			flag = uispriteRenderer.material.IsKeywordEnabled("PIXELSNAP_ON");
			color = uispriteRenderer.material.color;
			offset = uispriteRenderer.material.GetTextureOffset("_MainTex");
			scale = uispriteRenderer.material.GetTextureScale("_MainTex");
			offset2 = Vector2.zero;
			scale2 = Vector2.one;
			flag2 = uispriteRenderer.material.IsKeywordEnabled("_BumpMap");
			if (flag2)
			{
				offset2 = uispriteRenderer.material.GetTextureOffset("_BumpMap");
				scale2 = uispriteRenderer.material.GetTextureScale("_BumpMap");
			}
		}
		uispriteRenderer.material = new Material(m_Shader);
		uispriteRenderer.material.name = $"UISprite/{text}";
		if (flag)
		{
			uispriteRenderer.material.SetFloat("PixelSnap", 1f);
			uispriteRenderer.material.EnableKeyword("PIXELSNAP_ON");
		}
		uispriteRenderer.material.SetColor("_Color", color);
		uispriteRenderer.material.SetTextureOffset("_MainTex", offset);
		uispriteRenderer.material.SetTextureScale("_MainTex", scale);
		if (flag2)
		{
			uispriteRenderer.material.SetTextureOffset("_BumpMap", offset2);
			uispriteRenderer.material.SetTextureScale("_BumpMap", scale2);
		}
	}

	protected virtual void Initialize()
	{
	}

	protected abstract void UpdateShader();
}
