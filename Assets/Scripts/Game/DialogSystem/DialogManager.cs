using Game.GlobalComponent;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.DialogSystem
{
	public class DialogManager : MonoBehaviour
	{
		public List<Dialog> Dialogs = new List<Dialog>();

		[InspectorButton("SaveCheckedDialogsAsJson")]
		public string SaveCheckedDialogs = string.Empty;

		[Space(5f)]
		public Text DialogText;

		public Text ActorText;

		public GameObject DialogPanel;

		[TextArea]
		public string ExportedDialog;

		[InspectorButton("ExportDialog")]
		public string ExportDialogInList;

		private bool finishTyping;

		private bool nextLeprica;

		private bool replicaFinished;

		private int indexLeprica;

		private Animator panelBeforeDialog;

		private readonly List<Dialog> dialogsStash = new List<Dialog>();

		private Dialog currentDialog;

		private static DialogManager instance;

		public static DialogManager Instance
		{
			get
			{
				if (instance == null)
				{
					throw new Exception("DialogController is not initialized");
				}
				return instance;
			}
		}

		private void Awake()
		{
			instance = this;
		}

		public void StartDialog(string jsonFileText)
		{
			Dialog dialog = MiamiSerializier.JSONDeserialize<Dialog>(jsonFileText);
			StartDialog(dialog);
		}

		public void StartDialog(int dialogIndexInList)
		{
			StartDialog(Dialogs[dialogIndexInList]);
		}

		public void StartDialog(Dialog dialog)
		{
			if (dialogsStash.Count == 0)
			{
				dialogsStash.Add(dialog);
				StartCoroutine(TypeDialog(dialog));
			}
			else if (dialogsStash.Find((Dialog x) => x.DialogName == dialog.DialogName) == null)
			{
				dialogsStash.Add(dialog);
			}
		}

		public void Continue()
		{
			if (replicaFinished)
			{
				nextLeprica = true;
			}
			else
			{
				finishTyping = true;
			}
		}

		public void ExitFromDialog()
		{
			DialogPanel.SetActive(false);
			indexLeprica = 0;
			replicaFinished = false;
			finishTyping = false;
			Time.timeScale = 1f;
			StopAllCoroutines();
			dialogsStash.Remove(currentDialog);
			if (dialogsStash.Count > 0)
			{
				StartCoroutine(TypeDialog(dialogsStash[0]));
			}
		}

		public void ExportDialog()
		{
			if (ExportedDialog.Length > 0)
			{
				Dialog newDialog = MiamiSerializier.JSONDeserialize<Dialog>(ExportedDialog);
				if (Dialogs.Find((Dialog x) => x.DialogName == newDialog.DialogName) == null)
				{
					Dialogs.Add(newDialog);
					UnityEngine.Debug.Log(newDialog.DialogName + " added in list.");
				}
				else
				{
					UnityEngine.Debug.Log("Dialog " + newDialog.DialogName + " already exists in list.");
				}
				ExportedDialog = string.Empty;
			}
		}

		public void SaveCheckedDialogsAsJson()
		{
			List<Dialog> list = new List<Dialog>();
			foreach (Dialog dialog in Dialogs)
			{
				if (dialog.SaveDialog)
				{
					dialog.SaveDialog = false;
					SaveDialog(dialog, dialog.DialogName);
					list.Add(dialog);
				}
			}
			foreach (Dialog item in list)
			{
				Dialogs.Remove(item);
			}
			list.Clear();
		}

		public static void SaveDialog(Dialog dialog, string dialogName)
		{
		}

		private IEnumerator TypeDialog(Dialog dialog)
		{
			Time.timeScale = 0f;
			DialogPanel.SetActive(true);
			DialogText.text = string.Empty;
			currentDialog = dialog;
			WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();
			while (true)
			{
				if (!replicaFinished)
				{
					ActorText.text = dialog.Replics[indexLeprica].Actor;
					for (int z = 0; z <= dialog.Replics[indexLeprica].Replica.Length - 1; z++)
					{
						DialogText.text += dialog.Replics[indexLeprica].Replica[z];
						replicaFinished = (DialogText.text.Length == dialog.Replics[indexLeprica].Replica.Length);
						if (finishTyping)
						{
							DialogText.text = dialog.Replics[indexLeprica].Replica;
							replicaFinished = true;
							finishTyping = false;
							break;
						}
						yield return waitForEndOfFrame;
					}
				}
				if (nextLeprica)
				{
					replicaFinished = false;
					DialogText.text = string.Empty;
					nextLeprica = false;
					if (indexLeprica.Equals(dialog.Replics.Length - 1))
					{
						break;
					}
					indexLeprica++;
				}
				yield return waitForEndOfFrame;
			}
			ExitFromDialog();
		}
	}
}
