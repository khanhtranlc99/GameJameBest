using UnityEngine;
using UnityEngine.UI;

namespace Code.Game.Race
{
	public class RaceItemView : MonoBehaviour
	{
		[SerializeField]
		private Text position;

		[SerializeField]
		private Text racerName;

		public void SetPosition(int position)
		{
			this.position.text = position.ToString();
		}

		public void SetRacerName(string racerName)
		{
			this.racerName.text = racerName;
		}
	}
}
