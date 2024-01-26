using Game.Items;
using Game.Shop;
using System.Collections.Generic;
using UnityEngine;

public class UIHighlightsManager : MonoBehaviour
{
	private static UIHighlightsManager instance;

	[SerializeField]
	[Separator("Highlights arrays")]
	private List<GameObject> m_ShopButtons;

	[SerializeField]
	private GameObject m_ExitShopButton;

	[SerializeField]
	private GameObject m_BuyShopButton;

	[SerializeField]
	private GameObject m_DinamicHighLights;

	private ShopCategory m_TargetShopCategory;

	private ShopItem m_TargetShopItem;

	public static UIHighlightsManager Instance => instance;

	public static bool InstanceExist => (!(instance == null)) ? true : false;

	public void SetTargetItem(GameItem gameItem)
	{
		if (gameItem != null)
		{
			if (ShopManager.Instance != null)
			{
				ShopManager.Instance.GetShopItem(gameItem.ID, out m_TargetShopItem, out m_TargetShopCategory);
				foreach (ShopCategory shopCategore in ShopManager.Instance.GetShopCategores())
				{
					if (shopCategore.GameItem.Type == ItemsTypes.Money || shopCategore.GameItem.Type == ItemsTypes.Bonus || shopCategore == m_TargetShopCategory)
					{
						shopCategore.gameObject.SetActive(value: true);
						shopCategore.Container.SetActive(value: false);
					}
					else
					{
						shopCategore.gameObject.SetActive(value: false);
						shopCategore.Container.SetActive(value: false);
					}
				}
			}
			return;
		}
		if (ShopManager.Instance != null)
		{
			foreach (ShopCategory shopCategore2 in ShopManager.Instance.GetShopCategores())
			{
				shopCategore2.gameObject.SetActive(value: true);
			}
		}
		m_TargetShopCategory = null;
		m_TargetShopItem = null;
	}

	public void ActivateShopButtonsHighlights(bool value)
	{
		foreach (GameObject shopButton in m_ShopButtons)
		{
			shopButton.SetActive(value);
		}
	}

	public void ActivateExitShopButtonsHighlights(bool value)
	{
		m_ExitShopButton.SetActive(value);
	}

	public void ActivateBuyShopButtonsHighlights(bool value)
	{
		if(m_BuyShopButton!=null)
			m_BuyShopButton.SetActive(value);
	}

	private void ActivateDinamicHighLight(bool value)
	{
		m_DinamicHighLights.SetActive(value);
	}

	private void SetPositionDinamicHighLight(Vector3 pos)
	{
		m_DinamicHighLights.transform.position = pos;
	}

	private void Awake()
	{
		instance = this;
	}

	private void OnDestroy()
	{
		if (InstanceExist && instance == this)
		{
			instance = null;
		}
	}

	private void Update()
	{
		if (m_TargetShopItem != null && ShopManager.Instance != null && ShopManager.Instance.currentItem != null && ShopManager.Instance.Links.Categories.activeInHierarchy)
		{
			if (ShopManager.Instance.currentItem.GameItem.ID == m_TargetShopItem.GameItem.ID)
			{
				ActivateDinamicHighLight(value: false);
				ActivateBuyShopButtonsHighlights(value: true);
				return;
			}
			if (ShopManager.Instance.activeCategory != m_TargetShopCategory)
			{
				ActivateDinamicHighLight(value: true);
				SetPositionDinamicHighLight(m_TargetShopCategory.transform.position);
			}
			else if (ShopManager.Instance.currentItem != m_TargetShopItem)
			{
				ActivateDinamicHighLight(value: true);
				SetPositionDinamicHighLight(m_TargetShopItem.transform.position);
			}
			else
			{
				ActivateDinamicHighLight(value: false);
			}
			ActivateBuyShopButtonsHighlights(value: false);
		}
		else
		{
			ActivateDinamicHighLight(value: false);
			ActivateBuyShopButtonsHighlights(value: false);
		}
	}
}
