using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace Game.Character.Demo
{
	public class ZombieHUD : MonoBehaviour
	{
		public Texture HUD;

		public Vector2 Position;

		public Vector2 Size;

		public Vector2 Pos2;

		public float Size2;

		public TextMeshProUGUI Text;

		private int zombies;

		private int worms;

		public static ZombieHUD Instance
		{
			get;
			private set;
		}

		public void ZombieKilled()
		{
			zombies++;
		}

		public void WormKilled()
		{
			worms++;
		}

		private void Awake()
		{
			Instance = this;
			if (!Text)
			{
				Text = base.gameObject.GetComponent<TextMeshProUGUI>();
			}
		}

		private void OnGUI()
		{
			float x = (float)Screen.width * Position.x;
			float y = (float)Screen.height * Position.y;
			float width = (float)Screen.width * Size.x;
			float height = (float)Screen.height * Size.y;
			GUI.DrawTexture(new Rect(x, y, width, height), HUD, ScaleMode.ScaleToFit);
			x = (float)Screen.width * Pos2.x;
			y = (float)Screen.height * Pos2.y;
			float num = (float)Screen.height * Size2;
			//Text.pixelOffset = new Vector2(x, y);
			Text.fontSize = (int)num;
			Text.text = worms.ToString() + "\n\n" + zombies.ToString();
		}
	}
}
