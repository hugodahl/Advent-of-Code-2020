using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

class MainClass {

  public static IEnumerable<long> GetSampleInput(){
    yield return 35;
    yield return 20;
    yield return 15;
    yield return 25;
    yield return 47;
    yield return 40;
    yield return 62;
    yield return 55;
    yield return 65;
    yield return 95;
    yield return 102;
    yield return 117;
    yield return 150;
    yield return 182;
    yield return 127;
    yield return 219;
    yield return 299;
    yield return 277;
    yield return 309;
    yield return 576;
  }

  public static IEnumerable<long> GetInputFromFile(string filename){
    return File.ReadLines(filename).Select(value => long.Parse(value)); 
  }

  public static IEnumerable<long> GetInput(bool useTestInput = false){
    if (useTestInput){
      return GetSampleInput();
    }

    return GetInputFromFile("input.txt");
  }

  public static void Main (string[] args) {
     var input = GetInput(false);
     var preambleLength = 25;



    var mismatch = FindMismatch(input.ToList(), preambleLength);

    Console.WriteLine(mismatch);


    var sequence =  FindSumSequence(input.Take(mismatch.index).Reverse().ToList(), mismatch.value);

    Console.WriteLine($"Sum sequence: {string.Join(", ", sequence)}");
    Console.WriteLine($"Sum sequence answer: {sequence.Min() + sequence.Max()}");
  }


  public static (long value, int index) FindMismatch(IList<long> items, int preambleLength){

    var currentIndex = preambleLength;
    var itemCount = items.Count();

    while(currentIndex < itemCount){
      var startIndex = (currentIndex - preambleLength);
      var inputList = items.Skip(startIndex)
                           .Take(preambleLength)
                           .ToList();

      var currentValue = items.Skip(currentIndex).First();

      var hasSum = SequenceContainsSum(inputList, currentValue);

      //Console.WriteLine($"[{hasSum}] {currentValue} is summable in {string.Join(",", inputList)}.");

      if (false == hasSum){
        Console.WriteLine($"---------");
        Console.WriteLine($"ANSWER: {currentValue} @ {currentIndex}");
        Console.WriteLine($"---------");

        return (currentValue, currentIndex);
      }

      currentIndex++;
    }

    return (-1, -1);
  }


  public static bool SequenceContainsSum(IEnumerable<long> items, long value){
    var itemList = items.ToList();
    for(var i = 0; i < items.Count(); i++){
      var baseValue = itemList[i];
      var hasSum = itemList.Skip(i+1).Select(x => baseValue + x).Any(x => x == value);

      if (hasSum){
        return true;
      }

    }

    return false;
  }

  private static IEnumerable<long> FindSumSequence(IList<long> items, long value){
    var currentIndex = 0;

    while(currentIndex < items.Count){
      var currentSequence = new List<long>();

      for(var iAdd = 1; iAdd < items.Count; iAdd++){
        var sequence = items.Skip(currentIndex).Take(iAdd);
        var sequenceSum = sequence.Sum();

        // Console.WriteLine($"{currentIndex} @ {iAdd} = {sequenceSum} ({string.Join(",", sequence)})");

        if (sequenceSum > value){
          break;
        }

        if (sequenceSum == value){
          return sequence.Reverse();
        }

      }


      currentIndex++;
    }

    return Enumerable.Empty<long>();
  }


}