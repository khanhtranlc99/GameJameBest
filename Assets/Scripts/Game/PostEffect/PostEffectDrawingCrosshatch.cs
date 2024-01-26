using UnityEngine;

namespace Game.PostEffect
{
	[ExecuteInEditMode]
	public class PostEffectDrawingCrosshatch : MonoBehaviour
	{
		public static float ChangeWidth;

		public Shader ScShader;

		private float timeX = 1f;

		private Vector4 screenResolution;

		private Material scMaterial;

		[Range(1f, 10f)]
		public float Width = 2f;

		private Material Material
		{
			get
			{
				if (scMaterial == null)
				{
					scMaterial = new Material(ScShader)
					{
						hideFlags = HideFlags.HideAndDontSave
					};
				}
				return scMaterial;
			}
		}

		private void Start()
		{
			ChangeWidth = Width;
			ScShader = Shader.Find("PostEffect/Drawing_Crosshatch"); 
			enabled = false;
		}

		private void OnRenderImage(RenderTexture sourceTexture, RenderTexture destTexture)
		{
			if (ScShader != null)
			{
				timeX += Time.deltaTime;
				if (timeX > 100f)
				{
					timeX = 0f;
				}
				Material.SetFloat("_TimeX", timeX);
				Material.SetFloat("_Distortion", Width);
				Material.SetVector("_ScreenResolution", new Vector4(sourceTexture.width, sourceTexture.height, 0f, 0f));
				Graphics.Blit(sourceTexture, destTexture, Material);
			}
			else
			{
				Graphics.Blit(sourceTexture, destTexture);
			}
		}

		private void Update()
		{
			if (Application.isPlaying)
			{
				Width = ChangeWidth;
			}
		}

		private void OnDisable()
		{
			if ((bool)scMaterial)
			{
				UnityEngine.Object.DestroyImmediate(scMaterial);
			}
		}
	}
}
