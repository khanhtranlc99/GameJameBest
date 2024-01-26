using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.GlobalComponent
{
	[RequireComponent(typeof(MenuPanelManager))]
	public class ControlsPanelManager : MonoBehaviour
	{
		private static ControlsPanelManager instance;

		private ControlPanel activePanel;

		private Dictionary<ControlsType, ControlPanel> panels = new Dictionary<ControlsType, ControlPanel>();

		private MenuPanelManager panelManager;
		private CanvasGroup canvasGroup;

		public static ControlsPanelManager Instance
		{
			get
			{
				if (instance == null)
				{
					instance = GameObject.Find("UI/Canvas/Game/Controls").GetComponent<ControlsPanelManager>();
				}
				return instance;
			}
		}

		private void Awake()
		{
			instance = this;
			SetUpPanels();
			activePanel = GetControlPanel(GetComponent<MenuPanelManager>());
			activePanel.OnOpen();
			canvasGroup = GetComponent<CanvasGroup>();
		}

		private void SetUpPanels()
		{
			panels.Clear();
			ControlPanel[] componentsInChildren = GetComponentsInChildren<ControlPanel>(includeInactive: true);
			foreach (ControlPanel controlPanel in componentsInChildren)
			{
				if (!(controlPanel is SubPanel))
				{
					panels.Add(controlPanel.GetPanelType(), controlPanel);
				}
			}
		}

		public void SwitchPanel(ControlsType controlsType)
		{
			ControlPanel controlPanel = panels[controlsType];
			if ((bool)controlPanel && !(controlPanel == activePanel))
			{
				activePanel.OnClose();
				Animator panelAnimator = controlPanel.GetPanelAnimator();
				if (panelManager == null)
				{
					panelManager = GetComponent<MenuPanelManager>();
				}
				panelManager.OpenPanel(panelAnimator);
				controlPanel.OnOpen();
				activePanel = controlPanel;
			}
		}

		public void SwitchSubPanel(ControlsType controlsType, int subPanelIndex)
		{
			SwitchPanel(controlsType);
			SubPanelsController subPanelsController = activePanel as SubPanelsController;
			if ((bool)subPanelsController)
			{
				subPanelsController.OnOpen(subPanelIndex);
			}
		}

		public Transform GetControlPanel(ControlsType controlsType)
		{
			SetUpPanels();
			return panels[controlsType].transform;
		}

		private ControlPanel GetControlPanel(MenuPanelManager MPM)
		{
			Animator firstOpen = MPM.FirstOpen;
			ControlPanel component = firstOpen.GetComponent<ControlPanel>();
			if ((bool)component)
			{
				return component;
			}
			MenuPanelManager component2 = firstOpen.GetComponent<MenuPanelManager>();
			if ((bool)component2)
			{
				return GetControlPanel(component2);
			}
			UnityEngine.Debug.LogFormat(firstOpen, "Check this GameObject for having 'ControlPanel' or 'MenuPanelManager' component.");
			throw new ArgumentNullException("Can't find ControlPanel component or MenuPanelManager");
		}
	}
}
