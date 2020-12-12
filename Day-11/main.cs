using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;


namespace HugoDahl.AdventOfCode.Year2020
{

	public class Program
	{

    private static IEnumerable<string> GetInput(string filename){
      return File.ReadLines(filename);
    }

    public static IEnumerable<string> GetInput(bool useTestInput = false){
      var filename = "input.txt";
      if (useTestInput){
        filename = filename.Replace(".", ".test.");
      }

      return GetInput(filename);
    }


		public static int Main(string[] args)
		{
			var inputs = GetInput(true);

      var listed = inputs.ToList();

      var parsed = inputs.Select((input, rowIndex) => 
            input.Select((key, colIndex) => FloorLocation.Parse(key, new Point(rowIndex, colIndex))
                                                         ).ToList()
                                                         )
                 .ToList();

      var size = GetFileContentInfo(listed);
      var maxRounds = 10;

//       for(var round = 1; round <= maxRounds; round++){
//         parsed.ForEach(row => {
//           row.ForEach(cell => {
//             var contigs = cell.GetNeighbours(size.width, size.height)
//                               .Select(n => parsed[n.X][n.Y]);
//             cell.CalculateBusy(round, contigs);
//           });
//         });
//       }



      for(var round = 1; round <= maxRounds; round++){
        parsed.ForEach(row => {
          row.ForEach(cell => {
            cell.CalculateVisibleBusy(round, size.width, size.height, parsed);
          });
        });
      }


      var result = Enumerable.Range(1, maxRounds)
                             .Select(round => {
                                        var row = parsed[0].Count(c => c.IsOccupied(round));
                                        
                                        var rowResult = new {
                                          Round = round, 
                                          Occupieds = parsed.Sum(row => row.Count(cell => cell.IsOccupied(round))),
                                        };

                                        return rowResult;
                                    }
      );

      result.Select(res => $"Round: {res.Round};  {res.Occupieds} occupied seats")
            .ToList()
            .ForEach(Console.WriteLine);

      return 0;
		}

    private static void PrintGrid(IEnumerable<IEnumerable<FloorLocation>> parsed, int maxRounds){
      foreach(var round in Enumerable.Range(1, maxRounds))
      {
          var grid = string.Join(Environment.NewLine, parsed.Select(row => string.Join("", row.Select(cell => cell.GetState(round)))));

          Console.WriteLine($"----------------");
          Console.WriteLine($"---Round #{round}---");
          Console.WriteLine($"-------------");
          Console.WriteLine(grid);
          Console.WriteLine($"----------------");
       }
    }

    private static void PopulateInputArray<T>(IEnumerable<T> inputs, char[,] array){
      var indexed = inputs.Select((value, idx) => new {Value = value, Index = idx}).ToList();

      indexed.ForEach(value => {
        var innerValues = value.Value
                               .ToString()
                               .Select((inner, idx) => new {
                                                              Value = inner, 
                                                              Index = idx
                                                            }
                                      )
                               .ToList();
      
        innerValues.ForEach(inner => array[value.Index, inner.Index] = inner.Value);
      
      });

    }

    private static (bool consistant, int width, int height) GetFileContentInfo(IEnumerable<string> input){
      var height = input.Count();

      var sizes = input.Select(line => line.Length);
      var isConstantWidth = sizes.GroupBy(x => x).Count() == 1;
      if (isConstantWidth) {
        return (true, sizes.First(), height);
      }

      return (false, height, sizes.Max());

    }

    public static T[,] BuildInputArray<T>(int width, int height){
      return Array.CreateInstance(typeof(T), width, height) as T[,];
    }

	}

  public static class Helpers
  {
    public static bool IsChair(this char letter){
      return letter.Equals('L');
    }

    public static bool IsFloor(this char letter){
      return letter.Equals('.');
    }

    public static bool IsOccupied(this char letter){
      return letter.Equals('#');
    }

    public static bool IsBetween(this int value, int min, int max){
      return value >= min && value <= max;
    }

  }


  public class FloorLocation {
    public bool IsSeat { get; private set; }
    public bool IsFloor { get { return !this.IsSeat; }}
    public Point Position { get; private set; }

    private List<bool> _rounds = new List<bool>(){false};

    public bool IsOccupied(int round) {
      if (this.IsFloor){
        return false;
      }

      if(round > this._rounds.Count){
        Console.WriteLine($"Can't access round #{round} with only {this._rounds.Count} calculated.");
        return false;
      }

      return this._rounds[round];
    }

    public void CalculateVisibleBusy(int round, int maxRows, int maxColumns, List<List<FloorLocation>> seats){
      if (this.IsFloor){
        return;
      }

      var prevState = this.IsOccupied(round -1);

      var visibles = this.GetVisibleSeats(maxRows, maxColumns, seats)
                         .Select(pos => seats[pos.X][pos.Y])
                         .ToList();

      var busy = visibles.Where(seat => seat.IsOccupied(round-1))
                         .Count();

      if (!prevState){ // If we're empty
        this._rounds.Add(busy == 0);
        return;
      }

      if (busy > 5){
        this._rounds.Add(false);
        return;
      }

      this._rounds.Add(prevState);
    }

    public void CalculateBusy(int round, IEnumerable<FloorLocation> neighbours){
      if (this.IsFloor){
        return;
      }

      var prevState = this.IsOccupied(round -1);

      var busy = neighbours.Where(seat => seat.IsOccupied(round-1))
                           .Count();

      if (!prevState){ // If we're empty
        this._rounds.Add(busy == 0);
        return;
      }

      if (busy > 4){
        this._rounds.Add(false);
        return;
      }

      this._rounds.Add(prevState);
    }

    public static FloorLocation Parse(char value, Point position){
      var newSpot = new FloorLocation()
      {
        IsSeat = value.IsChair(),
        Position = position,
      };

      return newSpot;
    }

    public IEnumerable<Point> GetNeighbours(int maxRows, int maxCols){

      var minCol = Math.Max(0, this.Position.X -1);
      var maxCol = Math.Min(maxCols-1, this.Position.X + 1);
      var minRow = Math.Max(0, this.Position.Y - 1);
      var maxRow = Math.Min(maxRows-1, this.Position.Y + 1);

      var points = new List<Point>();

      for (var row = minRow; row <= maxRow; row++){
        for (var col = minCol; col <= maxCol; col++){
          yield return new Point(col, row);
        }
      }
    }

    public IEnumerable<Point> GetVisibleSeats(int maxRows, int maxCols, List<List<FloorLocation>> grid){
      var increments = new [] {-1, 0, 1};

      foreach(var x in increments){
        foreach(var y in increments){
          if (x == 0 && y == 0){
            continue; // Don't look at ourselves
          }

          var currX = this.Position.X + x;
          var currY = this.Position.Y + y;

          while (currX.IsBetween(0, maxRows -1) && currY.IsBetween(0, maxCols -1)){

            if (currX == this.Position.X && currY == this.Position.Y){
              continue; // Avoid looking at ourselves, again.
            }

            if (grid[currX][currY].IsSeat){
              yield return new Point(currX, currY);
              break;
            }

            currX += x;
            currY += y;

          }

        }
      }

    }

    public override string ToString(){
      return $"{(IsSeat ? 'L' : '.')} @ {Position}";
    }

    public char GetState(int round){

      if (this.IsFloor){
        return '.';
      }

      if (round > this._rounds.Count){
        return 'X';
      }

      return this._rounds[round] ? '#' : 'L';
    }

    public string ToString(int round){
      if (round > this._rounds.Count){
        return $"<INVALID (round {round})>";
      }
      
      return $"{this.GetState(round)} @ {Position}";
    }

  }

  public class Point
  {
    public int X, Y;

    public Point(int x, int y){
      this.X = x;
      this.Y = y;
    }

    public override string ToString(){
      return $"({X}, {Y})";
    }

  }

}
