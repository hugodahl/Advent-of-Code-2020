using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using System.Text;



class MainClass {

private static bool ValidateBirthYear(string input){
  var year = GetYear(input);
  if (false == year.HasValue){
    return false;
  }

  return year >= 1920 && year <= 2002;
}

private static bool ValidateIssueYear(string input){
  var year = GetYear(input);
  if (false == year.HasValue){
    return false;
  }

  return year >= 2010 && year <= 2020;
}

private static bool ValidateExpirationYear(string input){
  var year = GetYear(input);
  if (false == year.HasValue){
    return false;
  }

  return year >= 2020 && year <= 2030;
}

private static bool ValidateHeight(string input){
  if (input.Length < 4 || input.Length > 5){
    return false;
  }

  var unit = input.Substring(input.Length-2);

  var value = GetNumber(input.Substring(0, input.Length -2));

  if (false == value.HasValue){
    return false;
  }

  switch(unit){
    case "in": {
      return value >= 59 && value <= 76;
    }

    case "cm": {
      return value >= 150 && value <= 193;
    }

    default: {
      Console.WriteLine($"INVALID height: {input}");
      return false;
    }
  }

}

private static bool ValidateHairColour(string input){

  const string AllowedChars = "abcdef1234567890";

  if (7 > input.Length){
    return false;
  }

  if ('#' != input[0]){
    return false;
  }

  foreach(var ch in input.ToLowerInvariant().Skip(1)){
    if (false == AllowedChars.Contains(ch)){
      return false;
    }
  }

  return true;
}

private static bool ValidateEyeColour(string input){
  return MainClass.AllowedEyeColours.Any(col => col.Equals(input, StringComparison.CurrentCultureIgnoreCase));
}

private static bool ValidatePassportID(string input){
  var isValid = (9 == input.Length && input.All(c => char.IsNumber(c)));
//  Console.WriteLine($"PassportID: {input} is valid? {isValid}");
  return isValid;
}

private static bool ValidateCountryID(string input){
  return true; // Always true - ignored.
}

private static int? GetYear(string input){
  return GetNumber(input);
}

private static int? GetNumber(string input){
  if (int.TryParse(input, out var value)){
    return value;
  }

  return null;
}


public static string[] AllowedEyeColours = new [] {"amb", "blu", "brn", "gry", "grn", "hzl", "oth",};

public static readonly Dictionary<string, PassportField> PassportFields = new Dictionary<string, PassportField>{
  {"byr", new PassportField(){Name = "byr", IsRequired = true, Validator = ValidateBirthYear}},
  {"iyr", new PassportField(){Name = "iyr", IsRequired = true, Validator = ValidateIssueYear}},
  {"eyr", new PassportField(){Name = "eyr", IsRequired = true, Validator = ValidateExpirationYear}},
  {"hgt", new PassportField(){Name = "hgt", IsRequired = true, Validator = ValidateHeight}},
  {"hcl", new PassportField(){Name = "hcl", IsRequired = true, Validator = ValidateHairColour}},
  {"ecl", new PassportField(){Name = "ecl", IsRequired = true, Validator = ValidateEyeColour}},
  {"pid", new PassportField(){Name = "pid", IsRequired = true, Validator = ValidatePassportID}},
  {"cid", new PassportField(){Name = "cid", IsRequired = false, Validator = ValidateCountryID}},
};



  public static void Main (string[] args) {

    var entries = new List<Entry>();

    var inputString = new StringBuilder();

    using (var file = File.OpenText("input.txt"))
    {

      var loop = true;
      while(loop)
      {

        var line = file.ReadLine();

        if (null == line){
          line = string.Empty;
          loop = false;
        }


      if(line.Trim().Length == 0){
        var entry = Entry.Parse(inputString.ToString());
        entries.Add(entry);
        inputString.Clear();
      }
      else {
        inputString.AppendFormat(" {0}", line);
      }

      } // while

      // entries.ForEach(entry => Console.WriteLine(entry.ToString()));
      Console.WriteLine($"Total entries: {entries.Count}");

      Console.WriteLine($"Total passports: {entries.Count(etr => etr.IsPassport)}");

      Console.WriteLine($"Total VALID passports: {entries.Count(etr => etr.IsValidPassport)}");

    }
  }

}

class Entry
{
  private List<ItemField> _fields = new List<ItemField>();

  public static Entry Parse(string entry){
    var item = new Entry();
    item._fields.AddRange(entry.Split(' ').Select(data => ItemField.Parse(data)));

    return item;
  }

  public override string ToString(){
    return string.Join(' ', this.Fields.Select(field => field.ToString()));
  }

  public List<ItemField> Fields => this._fields;

  public bool IsPassport { get {
      var fieldResults = this.Fields.Join(MainClass.PassportFields.Where(ppt => ppt.Value.IsRequired).ToList(),
              fld => fld.Key,
              ppt => ppt.Key,
              (fld, ppt) => ppt)
              .ToList();

      // Console.WriteLine($"Field match count: {fieldResults.Count}");

      return fieldResults.Count >= (MainClass.PassportFields.Count(fld => fld.Value.IsRequired));
    }
  }

  public bool IsValidPassport { get {
    var fieldResults = this.Fields.Join(MainClass.PassportFields.Where(ppt => ppt.Value.IsRequired).ToList(),
              fld => fld.Key,
              ppt => ppt.Key,
              (fld, ppt) => fld.IsValid(ppt.Value.Validator))
              .ToList();

      return fieldResults.Count >= (MainClass.PassportFields.Count(pp => pp.Value.IsRequired)) && fieldResults.All(fr => fr);
    }


  }


}

class ItemField
{
  public string Key;
  public string Value;

  public static ItemField Parse(string input){
    var values = input.Split(':');

    var item = new ItemField() {Key = values[0], Value = string.Empty};
    if (values.Length > 1){
      item.Value = values[1];
    }

    return item;   
  }

  public bool IsValid(Func<string, bool> validator){
    return validator(this.Value);
  }

  public override string ToString(){
    return $"{this.Key}:{this.Value}";
  }

}

class PassportField
{
  public string Name;
  public bool IsRequired;
  public Func<string, bool> Validator;


  public override string ToString(){
    return $"{Name}: {IsRequired}";
  }

}
