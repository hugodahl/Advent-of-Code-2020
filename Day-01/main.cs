using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

class MainClass {
  public static void Main (string[] args) {
    var inputFile = "input.txt";
    var entries = ReadAllEntries(inputFile);
    Console.WriteLine($"There are {entries?.Count() ?? 0} in the file {inputFile}.");

    var values = entries.Select(x => int.Parse(x))
                        .ToList();


    for(var i = 0; i < values.Count; i++){
      for (var j = i; j < values.Count; j++){
        for (var k = j; k < values.Count; k++){
          if(values[i]+values[j]+values[k] == 2020){
            var vi = values[i];
            var vj = values[j];
            var vk = values[k];

            Console.WriteLine($"i: {i}; j: {j}; vi: {vi}; vj: {vj}; vk: {vk}; product: {vi*vj*vk}");
          }
        }
      }
    }



  }

  private static IEnumerable<string> ReadAllEntries(string filename)
  {
    if (!File.Exists(filename)){
      return null;
    }

    return File.ReadAllLines(filename);
  }


}