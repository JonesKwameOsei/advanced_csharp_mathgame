using System;
using System.Collections.Generic;
using Microsoft.VisualBasic;

namespace Maths_Game
{
  // Model to represent a game session
  public class GameResult
  {
    public DateTime Date { get; set; }
    public string? Operation { get; set; }
    public int Score { get; set; }
    public int QuestionsAsked { get; set; }

    public override string ToString()
    {
      return $"{Date:MM-dd-yyyy HH:mm:ss} | {Operation,-12} | Score: {Score} / {QuestionsAsked}";
    }
  }

  public class Math_Game
  {
    private static List<GameResult> gameHistory = new List<GameResult>();
    private static Random random = new();

    public static void Main(string[] args)
    {
      Console.Title = "Maths Game";
      Console.WriteLine("Maths Game");
      Console.WriteLine("==========");
      Console.WriteLine();

      while (true)
      {
        try
        {
          ShowMainMenu();
          string choice = GetUserChoice();

          switch (choice.ToLower())
          {
            case "1":
            case "a":
              PlayGame("Addition", '+');
              break;
            case "2":
            case "s":
              PlayGame("Subtraction", '-');
              break;
            case "3":
            case "m":
              PlayGame("Multiplication", '*');
              break;
            case "4":
            case "d":
              PlayGame("Division", '/');
              break;
            case "5":
            case "h":
              ShowGameHistory();
              break;
            case "6":
            case "q":
              Console.WriteLine("\n" + new string('=', 50));
              Console.WriteLine("Thank you for playing! Goodbye!");
              Console.WriteLine(new string('=', 50));
              return;
            default:
              Console.WriteLine("\n❌ Invalid selection. Please try again.");
              break;
          }
        }
        catch (Exception ex)
        {
          Console.WriteLine($"\n❌ An Unexpected error occurred: {ex.Message}");
        }
      }
    }

    private static void ShowMainMenu()
    {
      Console.Clear();
      Console.WriteLine(new string('=', 60));
      Console.WriteLine("                    🧮 MATH GAME 🧮");
      Console.WriteLine(new string('=', 60));
      Console.WriteLine();
      Console.WriteLine("Select an operation to play:");
      Console.WriteLine();
      Console.WriteLine("  1. [A]ddition      (+)");
      Console.WriteLine("  2. [S]subtraction  (-)");
      Console.WriteLine("  3. [M]ultiplication (×)");
      Console.WriteLine("  4. [D]ivision      (÷)");
      Console.WriteLine();
      Console.WriteLine("  5. [H]istory       📊");
      Console.WriteLine("  6. [Q]uit          🚪");
      Console.WriteLine();
      Console.WriteLine(new string('=', 60));
    }

    private static string GetUserChoice()
    {
      Console.Write("Enter your choice (1-6 or letter):");
      string input = ReadNonNullLine();

      return string.IsNullOrWhiteSpace(input) ? "" : input.Trim();
    }

    private static void PlayGame(string operationType, char operatorSymbol)
    {
      Console.Clear();
      Console.WriteLine(new string('=', 60));
      Console.WriteLine($"                    {operationType.ToUpper()} ");
      Console.WriteLine(new string('=', 60));
      Console.WriteLine();

      int numberOfQuestions = GetNumberOfQuestions();
      if (numberOfQuestions == 0) return; // If numberOfQuestions is 0, skip the game. That is, User chose to go back to main menu.

      int score = 0;
      List<string> gameLog = new List<string>();

      Console.WriteLine($"\nStarting {operationType} practice with {numberOfQuestions} questions...\n");

      for (int i = 1; i <= numberOfQuestions; i++)
      {
        try
        {
          var (num1, num2, correctAnswer) = GenerateQuestion(operatorSymbol);

          Console.WriteLine($"Question {i}/{numberOfQuestions}:");
          Console.Write($"  {num1} {operatorSymbol} {num2} = ");

          string userInput = Console.ReadLine()!;

          if (!int.TryParse(userInput, out int userAnswer))
          {
            Console.WriteLine("  ❌ Invalid input. Please enter a whole number.");
            gameLog.Add($"Q{i}: {num1} {operatorSymbol} {num2} = {correctAnswer} | Your answer: '{userInput}' (Invalid)");
            i--; // Repeat the question
            continue;
          }

          if (userAnswer == correctAnswer)
          {
            Console.WriteLine("  ✅ Correct!");
            score++;
            gameLog.Add($"Q{i}: {num1} {operatorSymbol} {num2} = {correctAnswer} |  Your answer: '{userAnswer}' ✅");
          }
          else
          {
            Console.WriteLine($"  ❌ Incorrect. The correct answer is {correctAnswer}.");
            gameLog.Add($"Q{i}: {num1} {operatorSymbol} {num2} = {correctAnswer} |  Your answer: '{userAnswer}' ❌");
          }
          Console.WriteLine();
        }
        catch (Exception ex)
        {
          Console.WriteLine($" ❌ Error processing question {i}: {ex.Message}");
          i--; // Repeat the question
        }
      }

      //  Show game Summary
      ShowGameSummary(operationType, score, numberOfQuestions, gameLog);

      // Save to history
      gameHistory.Add(new GameResult
      {
        Date = DateTime.Now,
        Operation = operationType,
        Score = score,
        QuestionsAsked = numberOfQuestions,
      });

      // Wait for user to continue
      PauseForUser();
    }

    private static int GetNumberOfQuestions()
    {
      while (true)
      {
        try
        {
          Console.Write("How many questions would you like to answer? (1-50, or 0 to go back): ");
          string input = ReadNonNullLine();

          if (!int.TryParse(input, out int questions))
          {
            Console.WriteLine("❌ Please enter a valid number.");
            continue;
          }

          if (questions == 0)
          {
            return 0; // Go back to main menu
          }

          if (questions < 1 || questions > 50)
          {
            Console.WriteLine("❌ Please enter a number between 1 and 50.");
            continue;
          }

          return questions;
        }
        catch (Exception ex)
        {
          Console.WriteLine($"❌ Error: {ex.Message}. Please try again.");
        }
      }
    }

    /*
    * A method to Generates a question based on the given operation.
    *<param name="operation">The operation to generate a question for. Must be one of '+', '-', '*', '/'.</param>
    * <returns>A tuple containing the two operands and the correct answer.</returns>
    * <exception cref="ArgumentException">If the operation is not one of the supported operations.</exception>
    */
    private static (int, int, int) GenerateQuestion(char operation)
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
          num2 = random.Next(1, num1 + 1); // This positive result
          answer = num1 - num2;
          break;

        case '*':
          num1 = random.Next(1, 13); // Keep multiplication reasonable
          num2 = random.Next(1, 13);
          answer = num1 * num2;
          break;

        case '/':
          // * Generate division that results in integers only
          answer = random.Next(1, 101);    // Result between 1 - 100
          num2 = random.Next(1, 101);     // Division between 1 and 100
          num1 = answer * num2;          // Ensures integer division
          break;

        default:
          throw new ArgumentException("Invalid operation");
      }

      return (num1, num2, answer);
    }

    private static void ShowGameSummary(string operationType, int score, int total, List<string> gameLog)
    {
      Console.WriteLine(new string('=', 60));
      Console.WriteLine($"                  {operationType.ToUpper()} GAME COMPLETED!");
      Console.WriteLine(new string('=', 60));
      Console.WriteLine();

      double percentage = (double)score / total * 100;
      Console.WriteLine($"📊 Final Score: {score} / {total} ({percentage:F1}%)");

      // Use switch expression to determine performance
      String performance = percentage switch
      {
        >= 90 => "🏆 Excellent! Outstanding Work!",
        >= 80 => "🥇 Great job! very good performance!",
        >= 70 => "🥈 Very Good! You did well!",
        >= 65 => "🥉 Good! Keep working hard!",
        >= 60 => "🎉 Not bad! Room for improvement!",
        _ => "📚 Keep practicing! You'll get better!"
      };

      Console.WriteLine($"🎯 Performance: {performance}");
      Console.WriteLine();

      Console.WriteLine("📝 Game Review:");
      Console.WriteLine(new string('-', 60));
      foreach (string log in gameLog)
      {
        Console.WriteLine($" {log}");
      }
      Console.WriteLine(new string('-', 60));
    }

    // * Show Game History
    private static void ShowGameHistory()
    {
      Console.Clear();
      Console.WriteLine(new string('=', 60));
      Console.WriteLine("                    📊 GAME HISTORY");
      Console.WriteLine(new string('=', 60));
      Console.WriteLine();

      if (gameHistory.Count == 0)
      {
        Console.WriteLine("No game played yet. Start a new game to see the history!");
      }
      else
      {
        Console.WriteLine($"Total games played: {gameHistory.Count}");
        Console.WriteLine();
        Console.WriteLine("Date & time     | Operation     | Score");
        Console.WriteLine(new string('-', 60));

        foreach (var game in gameHistory)
        {
          Console.WriteLine($"  {game}");
        }

        Console.WriteLine(new string('-', 60));

        // * Show statistics
        ShowStatistics();
      }

      Console.WriteLine();
      PauseForUser();
    }

    private static void ShowStatistics()
    {
      if (gameHistory.Count == 0) return;

      Console.WriteLine();
      Console.WriteLine("📈 Statistics:");

      int totalQuestions = 0;
      int totalCorrect = 0;

      var operationCounts = new Dictionary<string, int>();

      foreach (var game in gameHistory)
      {
        totalQuestions += game.QuestionsAsked;
        totalCorrect += game.Score;

        if (operationCounts.ContainsKey(game.Operation))
          operationCounts[game.Operation]++;
        else
          operationCounts[game.Operation] = 1;
      }

      double overallAccuracy = (double)totalCorrect / totalQuestions * 100;
      Console.WriteLine($"  . Overall Accuracy: {overallAccuracy:F1}% ({totalCorrect}/{totalQuestions})");

      Console.WriteLine("  . Games by Operation:");
      foreach (var kvp in operationCounts)
      {
        Console.WriteLine($"    - {kvp.Key}: {kvp.Value} games");
      }
    }

    private static void PauseForUser()
    {
      Console.WriteLine("\nPress any key to continue...");
      Console.ReadKey(true);
    }

    // * A Helper Method to get a non-empty string from the user
    public static string ReadNonNullLine()
    {
      return Console.ReadLine() ?? string.Empty;
    }
  }

}

