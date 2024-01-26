using System;

namespace Game.DialogSystem
{
	[Serializable]
	public class Dialog
	{
		[Serializable]
		public class DialogReplica
		{
			public string Actor;

			public string Replica;
		}

		public string DialogName;

		public bool SaveDialog;

		public DialogReplica[] Replics = new DialogReplica[0];
	}
}
