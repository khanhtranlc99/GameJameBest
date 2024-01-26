using UnityEngine;

namespace Game.Character.Extras
{
	public class HealthBar : MonoBehaviour
	{
		public static HealthBar Instance;

		public float PosX;

		public float PosY;

		public Texture2D HealthBarFull;

		public Texture2D HealthBarEmpty;

		private float health;

		private void Awake()
		{
			Instance = this;
		}

		public void SetHealth(float newHealth)
		{
			health = newHealth;
			if (health < 0f)
			{
				health = 0.1f;
			}
		}

		private void OnGUI()
		{
			float x = PosX * (float)Screen.width;
			float num = PosY * (float)Screen.height;
			float num2 = health / 100f * 200f;
			float num3 = 200f - num2;
			float num4 = 0f;
			if (health > 0.1f)
			{
				num4 = Mathf.Clamp01(1f - health / 100f) * 5f;
			}
			GUI.DrawTexture(new Rect(x, num, 50f, 200f), HealthBarEmpty, ScaleMode.StretchToFill);
			GUI.DrawTexture(new Rect(x, num + num3, 50f, 200f - num3 - num4), HealthBarFull, ScaleMode.StretchToFill);
		}
	}
}
