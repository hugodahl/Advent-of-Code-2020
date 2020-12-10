using System;
using System.IO;
using System.Linq;

class MainClass {
  public static void Main (string[] args) {
    var inputs = File.ReadAllLines("input.txt")
                     .Select(line => PasswordData.Parse(line))
                     .ToList();

    var groups = inputs.GroupBy(input => input.Validate()).ToDictionary(x => x.Key, x => x.ToList());
    Console.WriteLine($"Trues: {groups[true].Count()};  Falses: {groups[false].Count()}");
  }
}

public class PasswordData
{
  public byte MinOccurs{get;set;}
  public byte MaxOccurs{get;set;}
  public char Letter {get;set;}
  public string Password {get;set;}

  public bool Validate() {
    //return this.ValidateCharCounts();
    return this.ValidateCharPositions();
  }

  public bool ValidateCharPositions(){
    Console.WriteLine($"Pasword[{this.MinOccurs}] = {this.Password[this.MinOccurs-1]}; Password[{this.MaxOccurs}] = {this.Password[MaxOccurs-1]}");
    return (this.Password[this.MinOccurs - 1] == this.Letter) ^ (this.Password[this.MaxOccurs - 1] == this.Letter);
  }

  public bool ValidateCharCounts(){
    var letterCount = this.Password.Count(l => l == this.Letter);
    Console.WriteLine($"The letter '{this.Letter}' appears {letterCount} times in the password \"{this.Password}\" ({this.ToString()})");
    return letterCount >= this.MinOccurs && letterCount <= this.MaxOccurs;
  }

  public static PasswordData Parse(string input)
  {
    var elements = input.Split(' ');
    if (3 != elements.Length)
    {
      throw new InvalidOperationException($"The input {input} is not valid.");
    }

    var minMax = elements[0].Split('-');
    if (minMax.Length != 2){
      throw new InvalidOperationException($"First element is not valid: {elements[0]}.");
    }

    if (elements[1].Length < 1){
      throw new InvalidOperationException($"There is no character specified for the rule. Value read: {elements[1]}.");
    }

    var item = new PasswordData();
    item.Password = elements[2].Trim();

    if (string.IsNullOrEmpty(item.Password)){
      throw new InvalidOperationException($"There is no password value to test. Value: {item.Password}");
    }

    item.MinOccurs = byte.Parse(minMax[0]);
    item.MaxOccurs = byte.Parse(minMax[1]);

    item.Letter = elements[1][0];

    return item;
  }

  public override string ToString(){
    return $"{this.Letter} {{{this.MinOccurs}-{this.MaxOccurs}}} -> {this.Password}";
  }
  
}
