using Game.GlobalComponent;
using UnityEngine;

namespace Game.MiniMap
{
	public class MetroMarkForMiniMap : MarkForMiniMap
	{
		private const float ColliderScaleRate = 2f;

		public Metro MainMetro;

		protected override void DrawIcon()
		{
			base.DrawIcon();
			BoxCollider boxCollider = drawedIconSprite.gameObject.AddComponent<BoxCollider>();
			BoxCollider boxCollider2 = boxCollider;
			Vector3 size = boxCollider.size;
			float x = size.x * 2f;
			Vector3 size2 = boxCollider.size;
			boxCollider2.size = new Vector3(x, size2.y * 2f, 0.01f);
			drawedIconSprite.gameObject.AddComponent<MarkButton>().Init(this);
		}

		public void Select()
		{
			drawedIconSprite.color = Color.green;
		}

		public void SetCurrent()
		{
			drawedIconSprite.color = Color.yellow + Color.red;
		}

		public void DisableSelected()
		{
			drawedIconSprite.color = Color.white;
		}

		public void SetNormalSprite(Sprite sprite)
		{
			drawedIconSprite.sprite = IconImage;
			drawedIconSprite.transform.localScale = new Vector3(IconScale, IconScale, IconScale);
			DisableSelected();
		}

		public void SetMetroSprite(Sprite sprite)
		{
			drawedIconSprite.sprite = sprite;
		}

		public override void MarckOnClick()
		{
			if (MetroManager.Instance.InMetro)
			{
				if (MainMetro.Equals(MetroManager.Instance.TerminusMetro))
				{
					MetroManager.Instance.TakeTheSubway();
				}
				else
				{
					MetroManager.Instance.SetTerminus(MainMetro);
				}
				if (MetroPanel.Instance)
				{
					MetroPanel.Instance.CheckSelected();
				}
			}
		}
	}
}
