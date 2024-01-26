using System;
using UnityEngine;

namespace Game.GlobalComponent.Training
{
	public class TrainingInput : TrainingBase
	{
		[Serializable]
		public class TestAxis
		{
			public string AxisName;

			public float HoldTime = 2f;

			public bool CanBeNegative;

			public bool IsButton;

			[HideInInspector]
			public float CurrentHoldTime;

			[HideInInspector]
			public bool TestComplete;
		}

		public TestAxis[] TestableAxis;

		public override void ClickOnPanel()
		{
		}

		public override string GetContinueMessage()
		{
			return "Try it yourself to continue";
		}

		public override void StartTraining()
		{
		}

		public override void EndTraining()
		{
			TestAxis[] testableAxis = TestableAxis;
			foreach (TestAxis testAxis in testableAxis)
			{
				testAxis.CurrentHoldTime = 0f;
				testAxis.TestComplete = false;
			}
			TrainingManager.Instance.TrainingEnd();
		}

		private void Update()
		{
			TestAxis[] testableAxis = TestableAxis;
			foreach (TestAxis testAxis in testableAxis)
			{
				if (!testAxis.TestComplete && CheckUsedAxis(testAxis.AxisName, testAxis.CanBeNegative, testAxis.IsButton))
				{
					testAxis.CurrentHoldTime += Time.deltaTime;
					if (testAxis.CurrentHoldTime >= testAxis.HoldTime)
					{
						testAxis.TestComplete = true;
					}
				}
			}
			bool flag = true;
			TestAxis[] testableAxis2 = TestableAxis;
			foreach (TestAxis testAxis2 in testableAxis2)
			{
				if (!testAxis2.TestComplete)
				{
					flag = false;
					break;
				}
			}
			if (flag)
			{
				EndTraining();
			}
		}

		private bool CheckUsedAxis(string axisName, bool canBeNegative, bool isButton)
		{
			if (!isButton)
			{
				float num = Controls.GetAxis(axisName);
				if (canBeNegative)
				{
					num = Mathf.Abs(num);
				}
				return num > 0f;
			}
			return Controls.GetButton(axisName);
		}
	}
}
