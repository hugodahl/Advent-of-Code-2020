using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

class MainClass {

public static List<string> GetInput(bool useActualInput){
  if (useActualInput){
    return File.ReadLines("input.txt").ToList();
  }

  return new List<string>{
    "abcx",
    "abcy",
    "abcz",
    "",
    "ab",
    "bc",
    "",
    "acde",
    "eft",
    "des",
  };


}


  public static void Main (string[] args) {
    var inputs = GetInput(true);
    var groups = new List<Group>();

    var line = 0;
    var currentGroup = new Group();
    groups.Add(currentGroup);

    do {
      var currentInput = inputs[line];
      if (string.IsNullOrEmpty(currentInput)){
        currentGroup = new Group();
        groups.Add(currentGroup);
      }
      else {
        currentGroup.Add(currentInput);
      }

      line++;
    } while (line < inputs.Count);


//    groups.ForEach(grp => Console.WriteLine(grp.GetAnswerCount()));

    var totalAnswers = groups.Sum(grp => grp.GetAnswerCount());
    Console.WriteLine($"Total anwers: {totalAnswers}");


    // groups.ForEach(grp => Console.WriteLine(grp.GetMatchedAnswersCount()));

    var matchedAnswers = groups.Sum(grp => grp.GetMatchedAnswersCount());
    Console.WriteLine($"Total matched answers: {matchedAnswers}");

  }
}

class Group{
  public List<string> MemberAnswers {get; } = new List<string>();

  public void Add(string answer){
    this.MemberAnswers.Add(answer);
  }

  public int GetAnswerCount(){
    return this.GetAnswers().Distinct().Count();
  }

  public IEnumerable<char> GetAnswers(){
    return this.MemberAnswers.SelectMany(x => x);
  }

  public int GetMatchedAnswersCount(){
    var answers =  string.Join("", this.GetAnswers());
    var groups = answers.GroupBy(x => x);
    var matchedCounts = groups.Where(x => x.Count() == this.MemberAnswers.Count);
    var finalCount = matchedCounts.Count();

    Console.WriteLine($"Match data: {answers}; answers: {groups.Count()}; members: {this.MemberAnswers.Count};  groups: {matchedCounts.Count()}; matched: {finalCount} final");

    return finalCount;

  }

}