using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;


namespace HugoDahl.AdventOfCode.Year2020 {

	public class Program {


		public static readonly Dictionary<char, Instruction> Instructions = new Dictionary<char, Instruction>()
		{
			{'N', Instruction.North},
			{'E', Instruction.East},
			{'W', Instruction.West},
			{'S', Instruction.South},
			{'F', Instruction.Forward},
			{'R', Instruction.RotateRight},
			{'L', Instruction.RotateLeft},
		};


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
			Console.Clear();

			var inputs = GetInput();

			var movements = inputs.Select(input => Movement.Parse(input))
														 .ToList();

			var position = new Point(0, 0);
			var heading = Orientation.East;
			var pt1Answer = 0;
			var pt2Answer = 0;

			var wayPoint = new Point(10, 1);

			Console.WriteLine("----------------------------------");
			(position, heading, pt1Answer) = RunPart1(movements, position, heading);
			Console.WriteLine("----------------------------------");
			Console.WriteLine($"After part one: {position} heading {heading}. Answer: [{pt1Answer}]");
			Console.WriteLine("----------------------------------");
			Console.WriteLine();


			position = new Point(0, 0);
			heading = Orientation.East;

			Console.WriteLine("----------------------------------");
			(position, wayPoint, pt2Answer) = RunPart2(movements, position, wayPoint);
			Console.WriteLine("----------------------------------");
			Console.WriteLine($"After part two: {position} heading {heading}. Answer: [{pt2Answer}]");
			Console.WriteLine("----------------------------------");
			Console.WriteLine();
			Console.WriteLine();

			return 0;
		}

		private static (Point position, Orientation heading, int answer) RunPart1(List<Movement> movements, Point position, Orientation heading, bool printStatusMessages = false){

			var pos = position;
			var head = heading;

			movements.ForEach(movement => {
				if (printStatusMessages){Console.Write(movement);}
				(pos, head) = movement.ApplyToPoint(pos, head);
				if (printStatusMessages){Console.WriteLine($"  -->  [{pos} + {head}]");}
			});

			var total = Math.Abs(pos.Longitude) + Math.Abs(pos.Latitude);

			return (pos, head, total);
		}

		private static (Point position, Point waypoint, int answer) RunPart2(List<Movement> movements, Point position, Point waypoint, bool printStatusMessages = false){

			var currentPosition = position;
			var currentWaypoint = waypoint;

			foreach(var movement in movements){

				if (printStatusMessages){Console.Write(movement);}
				(currentWaypoint, currentPosition) = movement.ApplyToWaypoint(currentWaypoint, currentPosition);
				if (printStatusMessages){Console.WriteLine($"  -->  [{currentPosition} + {currentWaypoint}]");}

			}

			var result = Math.Abs(currentPosition.Longitude) + Math.Abs(currentPosition.Latitude);

			return (currentPosition, currentWaypoint, result);
		}


		public class Point{
			public readonly int Latitude;
			public readonly int Longitude;

			public Point(int longitude, int latitude){
				this.Longitude = longitude;
				this.Latitude = latitude;
			}

			public override string ToString(){
				return $"({this.Longitude} x {this.Latitude})";
			}

			public static Point Multiply(Point point, int factor){
				return new Point(point.Longitude * factor, point.Latitude * factor);
			}

			public static Point RotateRight(Point point){
				return new Point(point.Latitude, 0-point.Longitude);
			}

			public static Point RotateLeft(Point point){
				return new Point(0-point.Latitude, point.Longitude);
			}

		}

		public class Movement{
			public readonly int Amount = 0;

			public readonly Instruction Instruction;

			public Movement(Instruction instruction, int amount){
				this.Instruction = instruction;
				this.Amount = amount;
			}

			public static Movement Parse(string input){
				var op = ParseInstruction(input[0]);
				var amount =  int.Parse(input.Substring(1));
				var output = new Movement(op, amount);

				return output;
			}

			private bool IsRotation(){
				return Rotations.Contains(this.Instruction);
			}

			public (Point waypoint, Point position) ApplyToWaypoint(Point waypoint, Point position){

				if (this.IsRotation()){
					return (this.RotateWaypoint(waypoint), position);
				}

				if (this.Instruction == Instruction.Forward){
					var translation = Point.Multiply(waypoint, this.Amount);
					var newPosition = new Point(position.Longitude + translation.Longitude, position.Latitude + translation.Latitude);
					return (waypoint, newPosition);
				}

				return (this.ApplyToPoint(waypoint, Orientation.None).position, position);
			}

			public (Point position, Orientation heading)  ApplyToPoint(Point point, Orientation heading){

				var newHeading = this.ApplyOrientation(heading);
				var newPosition = this.ApplyMovement(point, heading);

				return (newPosition, newHeading);
			}


			public static Instruction ParseInstruction(char input){
				if (Instructions.ContainsKey(input))
					return Instructions[input];

				return Instruction.NoOp;
			}

			public override string ToString()
			{
				return $"{this.Instruction} by {this.Amount}";
			}

			private Orientation ApplyOrientation(Orientation origin){
				if (Rotations.Contains(this.Instruction)){
					switch(this.Instruction){
						case Instruction.RotateRight:
							return (Orientation)(Math.Abs(((int)origin + this.Amount)) % 360);
						case Instruction.RotateLeft:
							return (Orientation)(((int)origin + (360 - this.Amount)) % 360);
					}
				}

				return origin;
			}

			public Point RotateWaypoint(Point waypoint){

				if (false == Rotations.Contains(this.Instruction)){
					return waypoint;
				}

				var operations = this.Amount / 90;
				Func<Point, Point> method = Point.RotateLeft;

				if (this.Instruction == Instruction.RotateRight){
					method = Point.RotateRight;
				}

				var output = waypoint;
				for(var i = 0; i < operations; i++){
					output = method(output);
				}

				return output;
			}


			private Point ApplyMovement(Point position, Orientation heading){

				if (Rotations.Contains(this.Instruction)){
					return position;
				}

				var direction = this.Instruction;

				if (Instruction.Forward == this.Instruction){
					direction = MapForwardToInstruction(heading);
				}

				switch(direction){
					case Instruction.North:
						{
							return new Point(position.Longitude, position.Latitude + this.Amount);
						}

					case Instruction.East:
						{
							return new Point(position.Longitude + this.Amount, position.Latitude);
						}

					case Instruction.South:
						{
							return new Point(position.Longitude, position.Latitude - this.Amount);
						}
					case Instruction.West:
						{
							return new Point(position.Longitude - this.Amount, position.Latitude);
						}

					default:
						{
							return position;
						}

				}

			}

			private static Instruction MapForwardToInstruction(Orientation heading){
				switch(heading){
					case Orientation.North: return Instruction.North;
					case Orientation.East: 	return Instruction.East;
					case Orientation.South: return Instruction.South;
					case Orientation.West: 	return Instruction.West;
					default: 								return Instruction.NoOp;
				}
			}

			private static readonly Instruction[] Rotations = new [] {
				Instruction.RotateLeft,
				Instruction.RotateRight,
			};

		}

			public enum Orientation {
				None = North,
				North = 0,
				East = 90,
				South = 180,
				West = 270,
			}

			public enum Instruction {
				NoOp = 0,
				Default = NoOp,

				Forward = 1,
				RotateLeft = 2,
				RotateRight = 3,
				North = 11,
				East = 12,
				South = 13,
				West = 14,
			}


	}


}
