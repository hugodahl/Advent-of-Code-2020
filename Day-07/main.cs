using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

class MainClass {

  public static IEnumerable<string> GetRulesFromSample(){
    yield return "pink bags contain 4 purple bags";
    yield return "purple bags contain 2 white bags, 3 grey bags.";
    yield return "white bags contain 3 black bags";
    yield return "black bags contain no other bags.";


    yield return "pale chartreuse bags contain 3 faded orange bags.";
    yield return "drab gold bags contain 5 dark aqua bags.";
    yield return "mirrored magenta bags contain 3 dotted violet bags.";
    yield return "posh black bags contain 3 dark lavender bags, 3 mirrored coral bags, 1 dotted chartreuse bag.";
    yield return "striped yellow bags contain 5 pale red bags, 2 light lime bags, 5 clear indigo bags.";

  }


  public static IEnumerable<string> GetRulesFromFile(string filename){
    return File.ReadLines(filename);
  }


  public static IEnumerable<string> GetRules(bool loadFile){
    if (loadFile){
      return GetRulesFromFile("input.txt");
    }

    return GetRulesFromSample();
  }


  public static void Main (string[] args) {
    var allRules = GetRules(true).ToList();

    var targetBagColour = "shiny gold";
    //targetBagColour = "light chartreuse";
    //targetBagColour = "dark maroon";

    var parsedRules = allRules.Select(rule => BagDefinition.Parse(rule));

//    CountNestedColours(parsedRules, targetBagColour);

    var totalNests = CountNestedBags(parsedRules, targetBagColour);

    Console.WriteLine($"Total nested bags: {totalNests}");

  }

  private static int CountNestedBags(IEnumerable<BagDefinition> parsedRules, string targetBagColour, int level = 1){

    var rules = parsedRules.Where(rule => rule.Colour.Equals(targetBagColour, StringComparison.CurrentCultureIgnoreCase));
    var total = 0;

    if (false == rules.Any()){
      return 1;
    }

    foreach(var rule in rules){
      //Console.WriteLine($"{Indent(level)} -> {rule}");

      
      foreach(var subRule in rule.Rules){
        //Console.WriteLine($"{Indent(level)} {rule.Colour} - ({subRule.NumberOfBags} * ({CountNestedBags(parsedRules, subRule.BagColour, level +1)}));");
        total += (subRule.NumberOfBags * (CountNestedBags(parsedRules, subRule.BagColour, level +1)+1));
        // Console.WriteLine($"{Indent(level)} Current total: {total}");
      }
      //Console.WriteLine($"{Indent(level)} + {rule.Colour} contains {total} bags (level {level}).");
    }

    return total;

  }

  private static string Indent(int level, string item = "  "){
    var result = new StringBuilder();
    for(var i = 0; i < level; i++){
      result.Append(item);
    }

    return result.ToString();
  }


  private static void CountNestedColours(IEnumerable<BagDefinition> parsedRules, string targetBagColour)
  {
    var directLinks = parsedRules.Where(rule => rule.Rules.Any(bag => bag.BagColour?.Equals(targetBagColour, StringComparison.CurrentCultureIgnoreCase) ?? false));

    var allowedColours = directLinks.Select(rule => rule.Colour).ToList();

    var allowedCount = allowedColours.Count;

    while (true){

      var rule1 = parsedRules.Where(rule => rule.Rules.Any(r => allowedColours.Contains(r.BagColour)));

      var rule2 = rule1.Select(rule => rule.Colour);
        
      var rule3 = rule2.Except(allowedColours);

      var newColours = rule3.ToList();

      var finalColours = newColours.Except(allowedColours);  
      var newColoursCount = finalColours.Count();

      allowedColours.AddRange(finalColours);

      if (0 == newColoursCount){
        break;
      }
    }

    Console.WriteLine($"Allowed colours: {allowedColours.Count}");

  }
}

public class BagDefinition {
  const string SPLIT_TERM = "bags contain";

  public string Colour {get; set;}
  public List<BagRule> Rules {get;} = new List<BagRule>();

  public static BagDefinition Parse(string input){
    
    var definition = new BagDefinition();

    var inputs = input.Split(SPLIT_TERM);
    definition.Colour = inputs[0].Trim();

    var ruleDefinitions = inputs[1].Split(", ").Select(rd => rd.Trim());

    definition.Rules.AddRange(ruleDefinitions.Select(rd => BagRule.Parse(rd)));

    return definition;
  }

  public override string ToString(){
    return $"{this.Colour} {SPLIT_TERM} {string.Join(", ", this.Rules)}.";
  }

}

public class BagRule
{
  public string BagColour {get; set; }
  public int NumberOfBags{get;set;}


  public override string ToString(){
    return GetDescription();
  }

  private string GetDescription(){
    if (0 == this.NumberOfBags){
      return "no other bags";
    }

    return $"{NumberOfBags} {BagColour} bags";
  }

  public static BagRule Parse(string ruleString){
    var rule = new BagRule();

    if (ruleString.Equals("no other bags.", StringComparison.CurrentCultureIgnoreCase)){
      rule.NumberOfBags = 0;
      return rule;
    }

    var parts = ruleString.IndexOf(" ");

    var number = Convert.ToInt32(ruleString.Substring(0, parts));


    rule.BagColour = ruleString.Replace(".", string.Empty)
                               .Replace("bags", string.Empty)
                               .Replace("bag", string.Empty)
                               .Trim()
                               .Substring(parts+1);
    rule.NumberOfBags = number;

    return rule;
  }

}