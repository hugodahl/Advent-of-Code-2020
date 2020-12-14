using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

using Open.Numeric.Primes.Extensions;
namespace HugoDahl.AdventOfCode.Year2020
{


	public class Program
	{
		public static IEnumerable<string> GetInput(bool testData = false)
		{
			var filename = "input.txt";
			if (testData)
			{
				filename = "input.test.txt";
			}

			return File.ReadAllLines(filename);
		}


		public static int Main(string[] args)
		{
			Console.Clear();

			var inputs = GetInput(true).ToArray();

			if (false == long.TryParse(inputs[0], out var startTime))
			{
				Console.Error.WriteLine($"Could not parse the value {inputs[0]} as a {typeof(ulong).Name}.");
				return 1;
			}

			var routeIDs = inputs[1].Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
															.Where(value => value.All(character => char.IsNumber(character)))
															.Select(value => int.Parse(value))
															.ToArray();

			Console.WriteLine("------------------------");
			Console.WriteLine($"Start time: {startTime}.{Environment.NewLine}Routes: {string.Join(',', routeIDs)}.");
			Console.WriteLine("------------------------");
			Console.WriteLine(string.Empty);


			var result1 = RunPart1(startTime, routeIDs); ;
			Console.WriteLine("------------------------");
			Console.WriteLine($"--- Part #1: {result1}.");
			Console.WriteLine("------------------------");
			Console.WriteLine(string.Empty);


			var result2 = RunPart2(startTime, routeIDs);
			Console.WriteLine("------------------------");
			Console.WriteLine($"--- Part #2: {result2}.");
			Console.WriteLine("------------------------");
			Console.WriteLine(string.Empty);


			return 0;

		}

		public static long RunPart1(long startTime, IEnumerable<int> routeIDs)
		{
			var timeSpan = routeIDs.Select(id => new { ID = id, Minutes = startTime % id })
														 .Select(id => new { ID = id.ID, Minutes = id.Minutes, WaitTime = id.ID - id.Minutes})
														 .OrderBy(data => data.WaitTime);

Console.WriteLine(string.Join("; ", timeSpan.Select(x => $"{x.ID} @ {x.Minutes} (wait: {x.WaitTime}")));

			var soonest = timeSpan.First();

			return soonest.ID * soonest.WaitTime;
		}

		public static long RunPart2(long startValue, IEnumerable<int> routeIDs)
		{

			var maxValues = routeIDs.OrderByDescending(x => x)
															.ToArray();

			var totals = new long[maxValues.Length];

			maxValues.CopyTo(totals, 0);


			var values = Enumerable.Range(1, 20).Select((x, idx) => new {Value = x, Key = idx});


			var min = values.Min((x) => x.Key);


			while(true){
				if (Math.Abs(totals[0] - totals[1]) == 1){
					break;
				}

				if (totals[1] > totals[0]){
					totals[0] +=  maxValues[0];
					continue;
				}
				totals[1] += maxValues[1];

			}

			var answer = string.Join(Environment.NewLine, totals.Select((val, idx) => new {Value = val, Original = maxValues[idx]})
												 .Select(x => $"{x.Original} => {x.Value}"));

			Console.WriteLine(answer);

			return 0;
		}

	}


}
