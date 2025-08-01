using System;
using System.Collections.Generic;
using Maths_Game.Models;

namespace Maths_Game;

public class GameEngine
{
  private static readonly List<GameResult> gameHistory = new();
  private readonly Random random = new();
  private readonly Helpers helper = new Helpers();

  public void StartGame(String OperationType, char OperatorSymbol, ConsoleColor themeColor)
  {
    GameSession session = new GameSession
    {
      OperationType = OperationType,
      OperatorSymbol = OperatorSymbol,
      ThemeColor = themeColor
    };

    DisplayGameHeader(session);

    session.NumberOfQuestions = helper.GetNumberOfQuestions();
    if (session.NumberOfQuestions == 0) return; // Player chose to go back

    PlayGameSession(session);
    SaveGameResult(session);
    helper.PauseForUser();
  }

  private void DisplayGameHeader(GameSession session)
  {
    Console.Clear();

    // * Game-Specific header with theme color
    Console.ForegroundColor = session.ThemeColor;
    Console.WriteLine(new string('═', 70));
    Console.ForegroundColor = ConsoleColor.White;
    Console.WriteLine($"                    {session.OperationType.ToUpper()} PRACTICE");
    Console.ForegroundColor = session.ThemeColor;
    Console.WriteLine(new string('═', 70));
    Console.ResetColor();
    Console.WriteLine();
  }

  private void PlayGameSession(GameSession session)
  {
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine($"Starting {session.OperationType} practice with {session.NumberOfQuestions} questions...\n");
    Console.ResetColor();

    for (int i = 1; i <= session.NumberOfQuestions; i++)
    {
      try
      {
        if (ProcessQuestion(session, i))
        {
          session.CurrentScore++;
        }
      }
      catch (Exception ex)
      {
        helper.DisplayError($"Error processing question {i}: {ex.Message}");
        // Repeat the question
        i--;
      }
    }

    ShowGameSummary(session);
  }

  private bool ProcessQuestion(GameSession session, int questionNumber)
  {
    var (num1, num2, correctAnswer) = GenerateQuestion(session.OperatorSymbol);

    // Display question with theme color
    Console.ForegroundColor = session.ThemeColor;
    Console.WriteLine($"Question {questionNumber}/{session.NumberOfQuestions}:");
    Console.ForegroundColor = ConsoleColor.White;
    Console.Write($"  {num1} {session.OperatorSymbol} {num2} = ");
    Console.ForegroundColor = ConsoleColor.Yellow;

    string userInput = helper.ReadNonNullLine();
    Console.ResetColor();

    if (!int.TryParse(userInput, out int userAnswer))
    {
      helper.DisplayError("Invalid input. Please enter a whole number.");
      session.GameLog.Add($"Q{questionNumber}: {num1} {session.OperatorSymbol} {num2} = {correctAnswer} | Your answer: '{userInput}' (Invalid)");
      return false;
    }

    bool isCorrect = userAnswer == correctAnswer;
    DisplayAnswerFeedback(isCorrect, correctAnswer);

    string status = isCorrect ? "✅" : "❌";
    session.GameLog.Add($"Q{questionNumber}: {session.OperatorSymbol} {num2} = {correctAnswer} | Your answer: {userAnswer} {status}");

    Console.WriteLine();
    return isCorrect;
  }

  private void DisplayAnswerFeedback(bool isCorrect, int correctAnswer)
  {
    if (isCorrect)
    {
      Console.ForegroundColor = ConsoleColor.Green;
      Console.WriteLine("  ✅ Correct!");
    }
    else
    {
      Console.ForegroundColor = ConsoleColor.Red;
      Console.WriteLine($"  ❌ Incorrect. The correct answer is {correctAnswer}");
    }
    Console.ResetColor();
  }

  private (int, int, int) GenerateQuestion(char operation)
  {
    int num1, num2, answer;

    switch (operation)
    {
      case '+':
        num1 = random.Next(1, 101);
        num2 = random.Next(1, 101);
        answer = num1 + num2;
        break;

      case '-':
        num1 = random.Next(1, 101);
        num2 = random.Next(1, num1 + 1); // prevent negative results
        answer = num1 - num2;
        break;

      case '*':
        num1 = random.Next(1, 13);
        num2 = random.Next(1, 13);
        answer = num1 * num2;
        break;

      case '/':
        answer = random.Next(1, 11);
        num2 = random.Next(1, 11);
        num1 = answer * num2;
        break;
      default:
        throw new ArgumentException("Invalid operation");
    }

    return (num1, num2, answer);
  }

  private void ShowGameSummary(GameSession session)
  {
    Console.ForegroundColor = session.ThemeColor;
    Console.WriteLine(new string('═', 70));
    Console.ForegroundColor = ConsoleColor.White;
    Console.WriteLine($"                {session.OperationType.ToUpper()} GAME COMPLETED!");
    Console.ForegroundColor = session.ThemeColor;
    Console.WriteLine(new string('═', 70));
    Console.ResetColor();
    Console.WriteLine();

    double percentage = (double)session.CurrentScore / session.NumberOfQuestions * 100;

    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.WriteLine($"📊 Final Score: {session.CurrentScore}/{session.NumberOfQuestions} ({percentage:F1}%)");
    Console.ResetColor();

    DisplayPerformanceRating(percentage);
    DisplayGameReview(session.GameLog);
  }

  private void DisplayPerformanceRating(double percentage)
  {
    var (rating, color) = percentage switch
    {
      >= 90 => ("🏆 Excellent! Outstanding Work!", ConsoleColor.Yellow),
      >= 80 => ("🥇 Great job! Very good performance!", ConsoleColor.Green),
      >= 70 => ("🥈 Very Good! You did well!", ConsoleColor.Cyan),
      >= 65 => ("🥉 Good! Keep working hard!", ConsoleColor.Blue),
      >= 60 => ("🎉 Not bad! Room for improvement!", ConsoleColor.Magenta),
      _ => ("📚 Keep practicing! You'll get better!", ConsoleColor.Red)
    };

    Console.ForegroundColor = color;
    Console.WriteLine($"🎯 Performance: {rating}");
    Console.ResetColor();
    Console.WriteLine();
  }

  private void DisplayGameReview(List<string> gameLong)
  {
    Console.ForegroundColor = ConsoleColor.Gray;
    Console.WriteLine("📝 Game Review:");
    Console.WriteLine(new string('─', 70));

    foreach (string log in gameLong)
    {
      Console.WriteLine($"  {log}");
    }
    Console.WriteLine(new string('─', 70));
    Console.ResetColor();
  }

  private void SaveGameResult(GameSession session)
  {
    gameHistory.Add(new GameResult
    {
      Date = DateTime.Now,
      Operation = session.OperationType,
      Score = session.CurrentScore,
      QuestionsAsked = session.NumberOfQuestions
    });
  }

  public void ShowGameHistory()
  {
    Console.Clear();

    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.WriteLine(new string('═', 80));
    Console.ForegroundColor = ConsoleColor.White;
    Console.WriteLine("                          📊 GAME HISTORY");
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.WriteLine(new string('═', 80));
    Console.ResetColor();
    Console.WriteLine();

    if (gameHistory.Count == 0)
    {
      Console.ForegroundColor = ConsoleColor.Yellow;
      Console.WriteLine("No games played yet. Start a new game to see your history!");
      Console.ResetColor();
    }
    else
    {
      DisplayHistoryTable();
      DisplayStatistics();
    }

    Console.WriteLine();
    helper.PauseForUser();
  }

  private void DisplayHistoryTable()
  {
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine($"Total games played: {gameHistory.Count}");
    Console.ResetColor();
    Console.WriteLine();

    Console.ForegroundColor = ConsoleColor.Gray;
    Console.WriteLine("Date & Time          | Operation       | Score    | Accuracy");
    Console.WriteLine(new string('─', 80));
    Console.ResetColor();

    foreach (var game in gameHistory)
    {
      Console.WriteLine($"  {game}");
    }

    Console.ForegroundColor = ConsoleColor.Gray;
    Console.WriteLine(new string('─', 80));
    Console.ResetColor();
  }

  private void DisplayStatistics()
  {
    if (gameHistory.Count == 0) return;

    Console.WriteLine();
    Console.ForegroundColor = ConsoleColor.Magenta;
    Console.WriteLine("📈 Statistics:");
    Console.ResetColor();

    int totalQuestions = gameHistory.Sum(g => g.QuestionsAsked);
    int totalCorrect = gameHistory.Sum(g => g.Score);
    var operationCounts = new Dictionary<string, int>();

    foreach (var game in gameHistory)
    {
      string opKey = game.Operation ?? "Unknown";
      operationCounts[opKey] = operationCounts.GetValueOrDefault(opKey, 0) + 1;
    }

    double overallAccuracy = totalQuestions > 0 ? (double)totalCorrect / totalQuestions * 100 : 0;

    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.WriteLine($"  • Overall Accuracy: {overallAccuracy:F1}% ({totalCorrect}/{totalQuestions})");
    Console.WriteLine("  • Games by Operation:");
    Console.ResetColor();

    foreach (var kvp in operationCounts.OrderBy(x => x.Key))
    {
      Console.ForegroundColor = ConsoleColor.White;
      Console.WriteLine($"    - {kvp.Key}: {kvp.Value} games");
    }
    Console.ResetColor();
  }
}

