using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

class MainClass {
  public static void Main (string[] args) {

    var operations = File.ReadLines("input.txt").Select(line => Instruction.Parse(line));

    var program = new Program(operations);

    Console.WriteLine($"Program has {program.Operations.Count} instructions");

    var operationToInvert = 0;
    var lastResult = false;

    while (false == lastResult){

      if (operationToInvert >= program.Operations.Count){
        Console.WriteLine($"Tried all methods.");
        break;
      }

      program.Operations[operationToInvert].Invert();
      
      if (0 == operationToInvert % 10 && operationToInvert > 0){
        Console.WriteLine($"Performed {operationToInvert}/{program.Operations.Count}");
      }

      program.Reset();
      lastResult = program.Run(maxOperations: 10000);

      // Reset our inverted operation
      program.Operations[operationToInvert].Invert();

      operationToInvert += 1;
    }

    var attempt = operationToInvert -1;

    Console.WriteLine($"Result: {lastResult}.  ->  Acc: {program.Accumulator}; Operations: {program.OperationsExecuted}.");

    if (lastResult){
      Console.WriteLine($"Success after altering operation #{attempt} - WRONG = {program.Operations[attempt]}");
    }

  }
}

public class Program {
  private int _accumulator = 0;
  private List<Instruction> _operations = new List<Instruction>();
  private int _nextOperation = 0;

  public int Accumulator {get {return this._accumulator;} private set {this._accumulator = value;}}
  public List<Instruction> Operations {get {return this._operations;}}
  public int OperationsExecuted {get; private set;}

  public Program(IEnumerable<Instruction> operations, int accumulator = 0){
    this.Accumulator = accumulator;
    this.Operations.AddRange(operations);
  }


  public void Reset(){
    this.Accumulator = 0;
    this._nextOperation = 0;
    this.OperationsExecuted = 0;
  }

  public bool Run(int initialAccumulator = 0, int maxOperations = 10000){
    this.Accumulator = initialAccumulator;
    this._nextOperation = 0;

    // Console.WriteLine($"Max operations to run: {maxOperations}");

    while(this._nextOperation < this.Operations.Count){
      var output = this.Operations[this._nextOperation].Execute(this._nextOperation, this.Accumulator);
      this._nextOperation = output.inst;
      this.Accumulator = output.acc;
      this.OperationsExecuted+=1;
      // Console.WriteLine($@"{this.OperationsExecuted}) Post execution: Acc: {this.Accumulator}. Next op: {this._nextOperation}.");

      if (this.OperationsExecuted >= maxOperations){
        // Console.WriteLine($"---- Exceeded max operations of {maxOperations}. Exiting. ----");
        return false;
      } 
    }

//     Console.WriteLine($"Total operations run: {this.OperationsExecuted}. Accumulator: {this.Accumulator}.");
    return true;

  }

}

public class Instruction{
  public string Operation {get; set;}
  public short Offset {get; set;}

  public static Instruction Parse(string input){
    var inputs = input.ToLowerInvariant()
                      .Split(" ");
    var inst = new Instruction(){
      Operation = inputs[0].Trim(),
      Offset = Convert.ToInt16(inputs[1].Trim())
    };

    return inst;
  }

  public override string ToString(){
    return $"{this.Operation}  {this.Offset}";
  }

  public (int inst, int acc) Execute(int instructionID, int accumulator){
    // Console.WriteLine($@"Running ""{this.ToString()}"" on ({instructionID}, {accumulator})");
    switch(this.Operation){
      case "nop":{
        return (instructionID + 1, accumulator);
      }

      case "acc":{
        return (instructionID + 1, accumulator + this.Offset);
      }

      case "jmp":{
        return (instructionID + this.Offset, accumulator);
      }

      default:
        throw new ArgumentOutOfRangeException();
    }
  }

    public bool Invert(){
      var before = this.Operation;
      switch(this.Operation){
        case "nop":
          this.Operation = "jmp";
          break;

        case "jmp":
          this.Operation = "nop";
          break;
      }
      var after = this.Operation;

//      Console.WriteLine($@"{nameof(Invert)}: from ""{before}"" to ""{after}"".");
      return (before != after);

    }

}