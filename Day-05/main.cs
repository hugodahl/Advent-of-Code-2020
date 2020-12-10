using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

class MainClass {

  private static List<string> GetInputs(string filename = ""){

    if (string.IsNullOrEmpty(filename)){
      return new List<string>{
        "FBFBBFFRLR", 
        "BFFFBBFRRR", 
        "FFFBBBFRRR",
        "BBFFBBFRLL",
      };
    }

    return File.ReadLines(filename).ToList();

  }


  public static void Main (string[] args) {
    var inputs = GetInputs("input.txt")
                     .Select(text => Seat.Parse(text))
                     .ToList();

    Console.WriteLine($"Items: {inputs.Count}");

    var maxSeatID = inputs.Max(input => input.GetSeatID());

    Console.WriteLine($"Max seat ID: {maxSeatID}");

    var allSeats = Enumerable.Range(0, 127*7);

    var missingSeats = allSeats.Except(inputs.Select(i => i.GetSeatID()))
                           .ToList();

    Console.WriteLine($"Missing items: {missingSeats.Count}");

    var finals = missingSeats.Select((seat, pos) => new {Seat = seat, Pos = pos})
                              .Where(x => x.Seat != x.Pos)
                              .ToList();

    var missingSeat = finals.Single();

    Console.WriteLine(missingSeat);
    
  }
}

class Seat
{

  private string _rowPos;
  private string _seatPos;

  public string Location;

  public int GetRow(int size = 128){
    return GetConvertedBits(this.RowPos, 'F', 'B');
  }

  public int GetSeat(int size = 8){
    return GetConvertedBits(this.SeatPos, 'L', 'R');
  }

  private static string GetBitValues(string input, char zeroValue, char oneValue){
    var bits =  input.Replace(zeroValue, '0')
                     .Replace(oneValue, '1')
                     .Reverse()
                     .ToArray();

    return new string(bits);
  }

  private static int GetConvertedBits(string input, char zeroValue, char oneValue){
    var bits = GetBitValues(input, zeroValue, oneValue);
    var result = bits.Select((bit, index) => (bit == '0' ? 0 : 1) << index);

    // Console.WriteLine($"Input: {input}; {string.Join(",", result)}, sum: {result.Sum()}");

    return result.Sum();
  }

  public int GetSeatID(){
    return (this.GetRow() * 8) + this.GetSeat();
  }

  public string RowPos { get {return _rowPos; } private set { _rowPos = value; }}
  public string SeatPos {get {return _seatPos; } private set { _seatPos = value; }}

  public static Seat Parse(string position){
    var seat = new Seat {Location = position };
    seat.RowPos = seat.Location.Substring(0, 7);
    seat.SeatPos = seat.Location.Substring(7);
    return seat;
  }

  public override string ToString() {
    return $"{this.Location}: [ {this.RowPos} @ {this.SeatPos} ] - ({this.GetRow()} x {this.GetSeat()}) = {this.GetSeatID()}";
  }

}