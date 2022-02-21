using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MastermindConsole
{

    // class for all the main properties to be used
    class PublicProperties
    {
        // Boolean condition to determine whether the game is ongoing or has ended
        public static bool StartGame { get; set; } = true;

        // Boolean condition to determine if the game has been won
        public static bool GameWon { get; set; } = false;

        // Tells the computer how many digits the secret code will have (depends on difficulty setting)
        public static int NumsToGenerate { get; set; } = 4;

        // Sets how many guesses the user can have. Default is at 10.
        public static int NumOfGuesses { get; set; } = 10;

        // The user's turn number. Increments by 1 on each guess made by the user.
        public static int AttemptNumber { get; set; } = 0;

        // The minimum value for each individual digit in the secret code
        public static int MinNum { get; set; } = 1;

        // The maximum value for each individual digit in the secret code
        public static int MaxNum { get; set; } = 6;

        // The default settings for the total minimum range of the secret code.
        // This is expanded on in the SetMinMaxRange method after difficulty selection
        public static int MinRange { get; set; } = 1111;

        // The default settings for the total maximum range of the secret code.
        // This is expanded on in the SetMinMaxRange method after difficulty selection
        public static int MaxRange { get; set; } = 6666;

        // The secret code placed into an array to be compared to another array
        public static int[] NumberToGuess { get; set; }

        // The user's guess inside an array to be compared to another array
        public static int[] CurrentGuessArray { get; set; }

        // A list of numbers that have already been guessed
        public List<int> NumbersGuessed = new List<int>();

        // A list of all the attempts so they can be viewed during each turn and a full list after
        public static List<Attempt> GuessHistory { get; set; } = new List<Attempt>();
    }
    class Program : PublicProperties
    {
        static void Main(string[] args)
        {
            // while the game is active...
            while (StartGame)
            {
                //setup the game before the user starts guessing
                Setup Setup = new Setup();
                // loop through while the user has not had 10 guesses
                while (AttemptNumber < NumOfGuesses)
                {
                    UserAttempt Attempt = new UserAttempt();
                    // If user has won, display win message
                    if (GameWon)
                    {
                        //output to the user informing them they have won
                        Console.WriteLine("***** ***** YOU WIN!! ***** *****");
                        //break out of the loop if the user has won
                        break;
                    }
                }

                // method for displaying the history after the game is complete
                DisplayHistory History = new DisplayHistory();

                // Ask user if they want to play again
                Console.WriteLine("***** Do you want to play again? (y/n) *****");
                string answer = Console.ReadLine();
                if (answer == "y" || answer == "Y")
                {
                    StartGame = true;
                }
                else if (answer == "n" || answer == "N")
                {
                    Console.WriteLine("Thanks for playing.\n\n*****GAMEOVER*****");
                    StartGame = false;
                    Console.ReadKey();
                }
                else
                {
                    Console.WriteLine("Invalid selection.\n\n*****GAMEOVER*****");
                    StartGame = false;
                    Console.ReadKey();
                }
            }
        }
        class Setup : Functions
        {
            public Setup()
            {
                Console.Clear();
                GuessHistory = new List<Attempt>();
                SelectDifficulty();
                GenerateRandom();
                CurrentGuessArray = null;
                AttemptNumber = 0;
                GameWon = false;
                Console.Clear();
            }
        }
        class UserAttempt : Functions
        {
            public UserAttempt()
            {
                // display the turn number and also the amount of numbers and the range within which the secret code lies
                AttemptNumber += 1;
                Console.WriteLine("Guess number {0} (a {1} digit number between {2} and {3}): ",
                    AttemptNumber, NumsToGenerate, MinRange, MaxRange);
                // convert user input to int and output that int into the currentGuess variable 
                int.TryParse(Console.ReadLine(), out int currentGuess);
                // check user input fits within range
                if (currentGuess <= MaxRange && currentGuess >= MinRange)
                {
                    // Puts user guess into the array
                    CurrentGuessArray = ConvertToArray(currentGuess, NumsToGenerate);
                    // Adds user guess to the guess history list
                    GuessHistory.Add(new Attempt() { GuessNumber = AttemptNumber, GuessEntry = currentGuess, Result = GenerateAttemptResults()});
                    Console.WriteLine("Attempt {0} - {1}\nYou guessed: {2}", AttemptNumber, currentGuess, GenerateAttemptResults());
                }
                else
                {
                    // If an invalid number is entered i.e. too many digits, a turn is not deducted from guess limit
                    AttemptNumber -= 1;
                    Console.WriteLine("Please enter a valid number");
                }
            }
        }
        class DisplayHistory : Functions
        {
            public DisplayHistory()
            {
                //Prompt the user to select if they would like to see a full history of attempts
                Console.WriteLine("Would you like to view the full history of your attemps? (y/n)");
                var answer = Console.ReadLine();
                if (answer == "Y" || answer == "y")
                {
                    // Calls mathod to display all history
                    DisplayAllHistory();
                }
            }
        }

        class Functions : Program
        {
            // Method for generating a random number for the secret code
            public void GenerateRandom()
            {
                // generate a random number that fits within the confines of the difficulty selection
                // and store it in an array
                NumberToGuess = new int[NumsToGenerate];
                Random randomNum = new Random();
                for (int i = 0; i < NumsToGenerate; i++)
                {
                    NumberToGuess[i] = randomNum.Next(MinNum, MaxNum);
                }
            }

            public int[] ConvertToArray(int Guess, int numsToGenerate)
            {
                // converts the users guess into an array
                var result = new int[numsToGenerate];
                for (int i = result.Length - 1; i >= 0; i--)
                {
                    //this takes the end number from the guess and stores it in the correct location within the array
                    result[i] = Guess % 10;
                    //this removes that same number from the end of the users guess
                    Guess /= 10;
                }
                return result;
            }

            public string GenerateAttemptResults()
            {
                // This is to turn the user input into an attempt and display results depending on it's content
                // return an X, O or - depending whether the number appears, appears and is in correct positioning or doesnt appear
                string Result = "";
                // For loop to compare the user guess array 
                for (int num = 0; num < NumsToGenerate; num++)
                {
                    // if statement to compare array indexes to determine the response
                    if (CurrentGuessArray[num] == NumberToGuess[num])
                    {
                        Result += " [X] ";
                        // check to see if the number has already been guessed
                        if (NumbersGuessed.Find(x => x == CurrentGuessArray[num]) != CurrentGuessArray[num])
                        {
                            //if this number has not been guessed before, store this number in the NumbersGuessed list for future reference
                            NumbersGuessed.Add(CurrentGuessArray[num]);
                        }
                        continue;
                    }
                    // For loop to compare the user guess array 
                    for (int i = 0; i < NumsToGenerate; i++)
                    {
                        if (num == i)
                        {
                            // if number doesn't appear in secret code
                            if (i == (NumsToGenerate - 1))
                            {
                                Result += " [-] ";
                                break;
                            }
                            continue;
                        }

                        // check if current number guessed is located within the number to guess
                        if (CurrentGuessArray[num] == NumberToGuess[i])
                        {
                            // check if this number has alrady been found
                            if (NumbersGuessed.Find(x => x == CurrentGuessArray[num]) == CurrentGuessArray[num])
                            {
                                Result += " [-] ";
                                break;
                            }
                            //if the number has not been found, set [O] to show the user this number is correct but in the wrong place
                            Result += " [O] ";
                            break;
                        }
                        else
                        {
                            //if i is the last number and this is reached, this means that the number is not correct so display [-] to show the user this number is incorrect
                            if (i == (NumsToGenerate - 1))
                            {
                                Result += " [-] ";
                                break;
                            }
                            continue;
                        }
                    }
                }                
                CheckIfWon();
                return Result;
            }

            public void CheckIfWon()
            {
                // Check if the user's numbers match the secret code
                int countOfCorrectNumbers = 0;
                for (int i = 0; i < NumsToGenerate; i++)
                {
                    // Comparing each number in the array
                    if (CurrentGuessArray[i] == NumberToGuess[i])
                    {
                        // increase the amount of correct numbers
                        countOfCorrectNumbers += 1;
                    }
                }
                // If the correct numbers is the same as the total numbers to be guessed e.g. 4
                if (countOfCorrectNumbers == NumsToGenerate)
                {
                    GameWon = true;
                }
                else
                {
                    GameWon = false;
                }
            }
            public void SelectDifficulty()
            {
                // Difficulty selection for user
                Console.WriteLine("Please select a difficulty:");
                Console.WriteLine("*** Easy(4 numbers)     - 1\n*** Moderate(5 numbers) - 2\n*** Hard(6 numbers)     - 3");
                int.TryParse(Console.ReadLine(), out int DifficultySelected);
                NumsToGenerate = SetDificultyLevel(DifficultySelected);
                SetMinMaxRange(DifficultySelected);
            }
            public static int SetDificultyLevel(int DifficultySelected)
            {
                // switch statement to set the how many numbers the secret code is made up of depending on the diffiulty choice
                switch ((int)DifficultySelected)
                {
                    case 1:
                        return 4;
                    case 2:
                        return 5;
                    case 3:
                        return 6;
                    default:
                        return 4;
                }
            }
            public static void SetMinMaxRange(int DifficultySelected)
            {
                // switch statement to set the values of number ranges depending on difficult choice
                switch ((int)DifficultySelected)
                {
                    case 1:
                        MinRange = 1111;
                        MaxRange = 6666;
                        return;
                    case 2:
                        MinRange = 11111;
                        MaxRange = 66666;
                        return;
                    case 3:
                        MinRange = 111111;
                        MaxRange = 666666;
                        return;
                    default:
                        MinRange = 1111;
                        MaxRange = 6666;
                        return;
                }
            }
            public void DisplayAllHistory()
            {
                Console.Clear();
                Console.WriteLine("\n\n***** FULL ATTEMPT HISTORY *****");

                // foreach loop to display each history result stored in GuessHistory
                foreach (Attempt attempt in GuessHistory)
                {
                    Console.WriteLine("Attempt {0}: {1} - Result: {2}", attempt.GuessNumber, attempt.GuessEntry, attempt.Result);
                }
            }
        }
    }
    public class Attempt
    {
        public int GuessNumber { get; set; }
        public int GuessEntry { get; set; }
        public string Result { get; set; }
    }
}
