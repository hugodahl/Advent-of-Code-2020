using System;
using System.IO;
using System.Linq;

class MainClass {

  public static void Main (string[] args) {
    var input = File.ReadLines("input.txt").ToList();

    var rules = new [] {
      new MotionRule{Right = 1, Down = 1},
      new MotionRule{Right = 3, Down = 1},
      new MotionRule{Right = 5, Down = 1},
      new MotionRule{Right = 7, Down = 1},
      new MotionRule{Right = 1, Down = 2},
    };

    var lines = input.Count;
    var columns = input.FirstOrDefault().Length;

    Console.WriteLine($"Size (Lines x Cols): {lines} x {columns}");


    var trees = 0;
    var clears = 0;
    var product = (long)1;

    foreach(var rule in rules){

    var tree = 0;
    var clear = 0;


    for(int line = 0; line < lines; line++){
      var lineColumn = GetRelativeColumn(line * rule.Right, columns);
      var lineNumber = line * rule.Down;
      if (lineNumber >= lines) {
        break;
      }
      if (input[lineNumber][lineColumn] == '.')
      {
        clear++;
        continue;
      }

      tree++;

    }

      trees += tree;
      clears += clear;
      product *= tree;

      Console.WriteLine($"        {rule} -> Trees: {tree}; Clear: {clear}");
      Console.WriteLine($"        Running total: {trees} trees;  {clears} clears;  {product} product");
    }
    Console.WriteLine($"---------------------------------------------");
    Console.WriteLine($"Total: {trees} trees;  {clears} clears;  {product} product");

  }

  public static int GetRelativeColumn(int columnNumber, int columnSize){
    return columnNumber % columnSize;
  }

}

class MotionRule{
  public int Down {get; set;}
  public int Right {get; set;}

  public override string ToString(){
    return $"R{this.Right}+D{this.Down}";
  }
}