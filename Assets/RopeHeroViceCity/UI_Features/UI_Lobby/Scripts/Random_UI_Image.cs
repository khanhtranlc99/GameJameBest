using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class Random_UI_Image : MonoBehaviour
{
    public List<Sprite> lsData;
    public Image imageScene;
	public Image imageOld;
	Coroutine temp;
	int ran;
    private void OnEnable()
    {
        //if (temp != null)
        //{
        //    StopCoroutine(temp);
        //    temp = null;
        //}
        //temp = StartCoroutine(ChangeSprite());
    }
	private IEnumerator ChangeSprite()
	{
		imageOld.sprite = null;
 
		yield return new WaitForSeconds(1.5f);
		imageScene.DOColor(new Color32(0, 0, 0, 0), 1).OnComplete(delegate {

			ran = UnityEngine.Random.Range(0, lsData.Count);
			imageScene.sprite = lsData[ran];
			imageScene.DOColor(new Color32(255, 255, 255, 255), 1).OnComplete(delegate {

				if (temp != null)
				{
					StopCoroutine(temp);
					temp = null;
				}
				temp = StartCoroutine(ChangeSprite());
 

			});
		});



	}
}
