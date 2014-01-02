using System;
using System.Diagnostics;

namespace sample
{
	public class Test
	{
		public static Test Create(Action<int> iteration, string name)
		{
			return new Test { Iteration = iteration, Name = name };
		}

		public Action<int> Iteration { get; set; }
		public string Name { get; set; }
		public Stopwatch Watch { get; set; }
	}
}