using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;

namespace Game.GlobalComponent
{
	public class CPUTest : PerformanceTest
	{
		private const float point = 0.27f;

		private const float MaxLeadTime = 50f;

		private float s;

		public float count = 100f;

		private DateTime time;

		private double t;

		private double absLeadTime;

		private double minLeadTime;

		private double maxLeadTime;

		private bool isInit;

		private float timer;

		private List<double> results = new List<double>();

		public override void Init()
		{
			absLeadTime = 0.0;
			maxLeadTime = 0.0;
			minLeadTime = 10000.0;
			t = 0.0;
			s = 0f;
			timer = DetectingTime;
			isInit = true;
		}

		public void FixedUpdate()
		{
			if (!IsEnd && isInit)
			{
				time = DateTime.Now;
				for (int i = 0; i < (int)(count * 0.27f); i++)
				{
					string sequence = Console.In.ReadToEnd();
					int length = sequence.Length;
					sequence = Regex.Replace(sequence, ">.*\n|\n", string.Empty);
					int length2 = sequence.Length;
					// var task = Task.Run(delegate
					// {
					// 	string input = Regex.Replace(sequence, "tHa[Nt]", "<4>");
					// 	input = Regex.Replace(input, "aND|caN|Ha[DS]|WaS", "<3>");
					// 	input = Regex.Replace(input, "a[NSt]|BY", "<2>");
					// 	input = Regex.Replace(input, "<[^>]*>", "|");
					// 	input = Regex.Replace(input, "\\|[^|][^|]*\\|", "-");
					// 	return input.Length;
					// });
					string[] array = Variants.variantsCopy();
				}
				t = DateTime.Now.Subtract(time).TotalMilliseconds;
				results.Add(t);
				if (absLeadTime <= 0.0)
				{
					absLeadTime = t;
				}
				if (t > maxLeadTime)
				{
					maxLeadTime = t;
				}
				if (t < minLeadTime)
				{
					minLeadTime = t;
				}
				absLeadTime = results.Sum() / (double)results.Count;
				Timer();
			}
		}

		private void Timer()
		{
			timer -= Time.deltaTime;
			if (timer <= 0f)
			{
				isInit = false;
				EndTesting();
			}
		}

		public override void EndTesting()
		{
			Result = (float)(100.0 - 2.0 * absLeadTime);
			MonoBehaviour.print(Result);
			CallEndTestingEvent(Result, this);
			t = s;
		}

		private static IEnumerable<string> Chunks(string sequence)
		{
			int numChunks = Environment.ProcessorCount;
			int start = 0;
			int chunkSize = sequence.Length / numChunks;
			while (true)
			{
				int num;
				numChunks = (num = numChunks - 1);
				if (num >= 0)
				{
					if (numChunks > 0)
					{
						yield return sequence.Substring(start, chunkSize);
					}
					else
					{
						yield return sequence.Substring(start);
					}
					start += chunkSize;
					continue;
				}
				break;
			}
		}

		private static int[] CountRegexes(string chunk)
		{
			int[] array = new int[9];
			string[] array2 = Variants.variantsCopy();
			for (int i = 0; i < 9; i++)
			{
				Match match = Regex.Match(chunk, array2[i]);
				while (match.Success)
				{
					array[i]++;
					match = match.NextMatch();
				}
			}
			return array;
		}
	}
}
