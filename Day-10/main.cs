using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace HugoDahl.AdventOfCode.Year2020 {
  public class Program  {

    public static IEnumerable<string> ReadInputFile(bool useTestFile = false){
      var filename = "input.txt";
      if (useTestFile){
        filename = "input.test.txt";
      }

      return File.ReadAllLines(filename);

    }


    public static int Main(string[] args){
        var inputs = ReadInputFile(false).Select(line => int.Parse(line));

        Console.WriteLine($"Our input has {inputs.Count()} entries.");

        GetWidestSpread(inputs);

        GetNumberOfArrangements(inputs);

        GetPermutations(inputs);

        return 0;
    }


    public static long GetPermutations(IEnumerable<int> inputs, int maxGap = 3){

      var values = inputs.Union(new []{0, inputs.Max() + maxGap})
                         .OrderBy(x => x)
                         .Reverse();

      // Console.WriteLine($"Working inputs: {string.Join(", ", values)}");

      var subtreeCounts = new Dictionary<long, long>();

      foreach (var jolt in values) {
        var possibleNext = values.Where(j => j > jolt && j <= jolt + maxGap);
        subtreeCounts[jolt] = possibleNext.Select(n => subtreeCounts[n]).Sum();
        if (subtreeCounts[jolt] == 0) subtreeCounts[jolt] = 1;
      }

Console.WriteLine($"Total: {subtreeCounts[0]}");

Console.WriteLine(subtreeCounts);

            return subtreeCounts[0];
    }


    public static int GetNumberOfArrangements(IEnumerable<int> inputs, int maxGap = 3){

      var sortedInputs = inputs.Union(new []{0, inputs.Max() + 3})
                               .OrderBy(x => x)
                               .Reverse()
                               .Select((value, idx) => new {value, idx})
                               .ToList();

      var jumps = sortedInputs.Sum(x => sortedInputs.Skip(x.idx)
                                                      .Take(maxGap)
                                                      .Where(gap => gap.value <= maxGap)
                                                      .Count());

      
      
      return jumps;

    }

    public static int GetWidestSpread(IEnumerable<int> inputs){
      
      var sortedInputs = inputs.Union(new []{0, inputs.Max() + 3})
                               .OrderBy(x => x)
                               .Reverse()
                               .Select((value, idx) => new {value, idx});

      var gaps = sortedInputs.Select(input => new {
                                                    input.value, 
                                                    input.idx, 
                                                    diff = sortedInputs.Skip(input.idx-1).First().value - input.value
                                                  })
                              .Where(gap => gap.diff > 0);

      var gapAmounts = gaps.GroupBy(gap => gap.diff)
                            .Select(gap => new {Size = gap.Key, Count = gap.Count()});


      Console.WriteLine(string.Join(", ", gapAmounts));

      return sortedInputs.Count();
    }


  }
}