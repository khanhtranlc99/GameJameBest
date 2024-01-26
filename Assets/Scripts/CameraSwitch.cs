using UnityEngine;
using UnityEngine.UI;

public class CameraSwitch : MonoBehaviour
{
	public GameObject[] objects;

	private int currentActiveObject;

	public Text text;

	private void OnEnable()
	{
		text.text = objects[currentActiveObject].name;
	}

	public void NextCamera()
	{
		int num = (currentActiveObject + 1 < objects.Length) ? (currentActiveObject + 1) : 0;
		for (int i = 0; i < objects.Length; i++)
		{
			objects[i].SetActive(i == num);
		}
		currentActiveObject = num;
		text.text = objects[currentActiveObject].name;
	}
}
