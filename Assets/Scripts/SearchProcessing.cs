using Game.GlobalComponent;
using UnityEngine;

public class SearchProcessing : MonoBehaviour
{
	public ISeachProcess process;

	private SlowUpdateProc updateProcess;

	private bool isInit;

	public void Init()
	{
		isInit = true;
		process.Initialize();
		ISeachProcess seachProcess = process;
		updateProcess = new SlowUpdateProc(seachProcess.Processing, 0.3f);
	}

	private void FixedUpdate()
	{
		if (isInit)
		{
			updateProcess.ProceedOnFixedUpdate();
		}
	}

	private void OnDestroy()
	{
		if (isInit)
		{
			process.Release();
		}
	}
}
