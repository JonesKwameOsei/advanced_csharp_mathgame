using System;

namespace Maths_Game.Models;

public class GameResult
{
  public DateTime Date { get; set; }
  public string? Operation { get; set; }
  public int Score { get; set; }
  public int QuestionsAsked { get; set; }
  public double Accuracy => QuestionsAsked > 0 ? (double)Score / QuestionsAsked * 100 : 0;

  public override string ToString()
  {
    return $"{Date:MM-dd-yyyy HH:mm:ss} | {Operation,-15} | Score: {Score,2}/{QuestionsAsked,-2} | {Accuracy:F1%}";
  }
}

public class GameSession
{
  public string OperationType { get; set; } = string.Empty;
  public char OperatorSymbol { get; set; }
  public int numberOfQuestions { get; set; }
  public int CurrentScore { get; set; }
  public List<string> GameLog { get; set; } = [];
  public ConsoleColor ThemeColor { get; set; }
}

public enum GameOperation
{
  Addition = 1,
  Subtraction = 2,
  Multiplication = 3,
  Division = 4,
  History = 5,
  Quit = 6
}
