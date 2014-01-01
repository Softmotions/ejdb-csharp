using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace sample
{
	class Tests : List<Test>
	{
		public void Add(Action<int> iteration, string name)
		{
			Add(Test.Create(iteration, name));
		}

		public void Run(int iterations)
		{
			// warmup 
			foreach (var test in this)
			{
				test.Iteration(iterations + 1);
				test.Watch = new Stopwatch();
				test.Watch.Reset();
			}

			var rand = new Random();
			for (int i = 1; i <= iterations; i++)
			{
				foreach (var test in this.OrderBy(ignore => rand.Next()))
				{
					test.Watch.Start();
					test.Iteration(i);
					test.Watch.Stop();
				}
			}

			foreach (var test in this.OrderBy(t => t.Watch.ElapsedMilliseconds))
			{
				Console.WriteLine(test.Name + " took " + test.Watch.ElapsedMilliseconds + "ms");
			}
		}
	}
}