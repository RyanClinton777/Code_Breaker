using System;
using System.Collections;
using System.Diagnostics; //for debug.writeline()
using Xamarin.Forms;

/*
    INFORMATION
    As you know I did the 2018 lab exam app for practice, and since you said I could use it for my project I just improved on it and added extra featues

    FEATURES:
    Menu
        Just a title label, image, and buttons to play and view scoreboard
    Play
        Back button, might be redundant but in UWP debugger there doesn't seem to be a universal one like on a phone
        High score displayed in corner, updated when the page is loaded, and after the user finishes a game for instant feedback for new best score. accounts for no wins//no scores file
        Code
            generates 4 unique numbers
            hides code until game over, then displays it
        Entry
            Deletes non-numbers extra numbers (no more than 1 allowed)
            Doesn't allow user to enter guess w/out all 4 fields filled, notifies them
        Previous Guesses
            When a guess is made, it is added to this area
            Colours show accuracy of guess
            on 5th guess, user gets game over notification, is prevented from making any further guesses until new game is started
        New game
            Resets all variables, empties forms, gets rid of previous guesses etc
    Scoreboard
        Displays info about previously completed games, this data is stored in scores.txt (deliminated by ;). Creates file if it doesn't exist

    
    Scores.txt, is stored in Environment.Specialfolder.LocalApplicationData folder, so for UWP search for it in appdata folder if you want to examine it, not sure where android/ios puts it.
*/

/*
   TODO:
   -Custom icon
*/

namespace Code_Breaker
{
	public partial class Game : ContentPage
    {
        private const int MAX_GUESSES = 5; //avoiding "magic number" can refer to this variable instead of putting 5 all over the place

        //Colours for differnt guess accuracy for previous guesses, if all 4 are right game is over, so no colour needed
        private Color _guessed0 = Color.Red;
        private Color _guessed1 = Color.Orange;
        private Color _guessed2 = Color.Yellow;
        private Color _guessed3 = Color.Green;

        private static bool _gameOver = false; //Used to stop user from making extra guesses after game is over.
        private static int _guessCount = 0; //counts guesses, user gets up to 5 then gameover.
        private static int _matchingNumbers = 0; //Counts the number of digits in the users guess that match the code, to decide colour of squares or victory altogether.

        private static int[] _userGuess = new int[4]; //Latest Guess by the user
        private static int[] _code = new int[4]; //Holds the actual code that is to be broken

        DateTime _startDT; //holds time the new game was started, will be added to score board if the game is finished

        public Game()
        {
            InitializeComponent();
            NewGame();
            //Get high score string, put it in the label - will say N/A if there isn't one yet
            lblHighScore.Text = "Best: " + Score.GetHighScore();
        }

        private void BtnEnterGuess_Clicked(object sender, EventArgs e)
        {
            //Don't let the user make more guesses if the game has ended.
            if (_gameOver == true) return;

            //Don't let the user leave the fields empty
            if (String.IsNullOrEmpty(numEntry1.Text) ||
                String.IsNullOrEmpty(numEntry2.Text) ||
                String.IsNullOrEmpty(numEntry3.Text) ||
                String.IsNullOrEmpty(numEntry4.Text))
            {
                //Warn user and do nothing
                Debug.WriteLine("Null/Empty entry attempted");
                DisplayAlert("Warning", "Please enter a value for every number.", "OK");
                return;
            }

            //increment guess counter
            _guessCount++;
            //Put each number of the users guess into the _userGuess array.
            _userGuess[0] = Int32.Parse(numEntry1.Text); //This is how you parse in C#
            _userGuess[1] = Int32.Parse(numEntry2.Text);
            _userGuess[2] = Int32.Parse(numEntry3.Text);
            _userGuess[3] = Int32.Parse(numEntry4.Text);

            //Check how many digits user guesses correctly. Could use a loop here but it's only 4 things
            if (_userGuess[0] == _code[0]) _matchingNumbers++;
            if (_userGuess[1] == _code[1]) _matchingNumbers++;
            if (_userGuess[2] == _code[2]) _matchingNumbers++;
            if (_userGuess[3] == _code[3]) _matchingNumbers++;

            //pick appropriate colour based on accuracy of guess, or end game if entire code guessed
            Color color = _guessed0; //local var, store color. Red/0 numbers guessed right by default
            if (_matchingNumbers == 1) color = _guessed1;
            if (_matchingNumbers == 2) color = _guessed2;
            if (_matchingNumbers == 3) color = _guessed3;

            //All 4 guessed right, call Win() method
            if (_matchingNumbers == 4) Win();

            //Unsuccesful guess made
            else
            {
                //Adding rows for each guess.
                //You can't assign strings except for Text (can't do Orientation = "Horizontal") but if you type = and look through the options you'll find a workaround like StackOrientation.Horizontal
                //We create everything, add the children to the layout and add the layout to the children outer layout in Xaml
                var layout = new StackLayout { HorizontalOptions = LayoutOptions.Center, Orientation = StackOrientation.Horizontal };
                //Using named sizes like Large has to be done as below because different platforms use diff sizes, for simplicity just use numbers. (FontSize = 22)
                var label = new Label { Text = "Guess " + _guessCount + ":" + PrintArray(_userGuess), FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)), HorizontalOptions = LayoutOptions.Start }; //PrintArray() is a custom method that returns a string for each number in the array seperated by a space + a space at the start, just makes this less messy
                var boxview = new BoxView { Color = color }; //local variable color based on number of digits guessed correctly
                //Add label and boxview to our new layout
                layout.Children.Add(label); //Add() takes object of type Var as arg, so we can't use explicit types
                layout.Children.Add(boxview);
                //Add the new layout to the SLPreviousGuesses one.
                SLPreviousGuesses.Children.Add(layout);

                //If user has guessed 5 times, gameover
                if (_guessCount == MAX_GUESSES) GameOver();
            }

            //Reset matching number counter for next guess
            _matchingNumbers = 0;
        }

        //Called when user has used up all their guesses, reveals the code.
        private void GameOver() //to use DisplayAlert, method has to be async, idk what this means, you can just use a label for simplictiy
        {
            //Display code on top label
            lblCode.Text = (_code[0] + " " + _code[1] + " " + _code[2] + " " + _code[3]);
            Debug.WriteLine("YOU LOSE");
            //display message box
            DisplayAlert("Game Over", "You Lose - The correct code was" + PrintArray(_code), "OK");
            //set _gameOver flag to true, used to stop further moves on game end
            _gameOver = true;

            RecordScore(false);
        }

        //User guessed all 4 digits in code correctly, display victory message and end game.
        private void Win()
        {
            //Display code on top label
            lblCode.Text = (_code[0] + " " + _code[1] + " " + _code[2] + " " + _code[3]);
            Debug.WriteLine("YOU WIN");
            //display message box
            DisplayAlert("VICTORY", "YOU WIN - The correct code was" + PrintArray(_code), "OK");
            //set _gameOver flag to true, used to stop further moves on game end
            _gameOver = true;

            RecordScore(true);
        }

        //Create Score object using game data, call save method from Score class
        private void RecordScore(bool win)
        {
            //Strings to hold the guess and code
            String strGuess = "";
            String strCode = "";

            //loops 4 times, puts all 4 characters of both the final guess and the code into strings
            for (int i = 0; i < 4; i++)
            {
                strGuess += _userGuess[i];
                strCode += _code[i];
            }

            //Create Score Object
            Score newScore = new Score(_startDT, DateTime.Now, _guessCount, win, strGuess, strCode);
            //Call SaveScore method from Score class, saves to file.
            newScore.SaveScore();

            //Update the highscore, so it shows up in the label if the player just set a new one
            lblHighScore.Text = "Highscore: " + Score.GetHighScore();
        }

        //Generate new code, clear previous guesses, reset variables.
        private void BtnNewGame_Clicked(object sender, EventArgs e)
        {
            NewGame(); //The reason for seperate method is because I also want to use this code every time the page is loaded.
        }

        //Reset everything, start new game
        private void NewGame()
        {
            //Reset variables
            _matchingNumbers = 0;
            _guessCount = 0;
            _userGuess = new int[4];
            _gameOver = false;

            //Empty forms
            numEntry1.Text = "";
            numEntry2.Text = "";
            numEntry3.Text = "";
            numEntry4.Text = "";

            //update high score
            Score.GetHighScore();

            //clear SLPreviousGuesses, put the label back in.
            SLPreviousGuesses.Children.Clear();
            //Replace label
            SLPreviousGuesses.Children.Add(new Label
            {
                Text = "Previous Guesses:",
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.Start
            });

            //Generate code and put in _code array.
            GenerateCode();

            //set lblCode to show "? ? ? ?" - actual code will be displayed when the game ends.
            lblCode.Text = "? ? ? ?";

            //Record time started
            _startDT = DateTime.Now;
        }//end NewGame()

        //Returns a string with the numbers in the given array seperated by a space, with a space at the start as well.
        private static String PrintArray(int[] array)
        {
            String str = "";
            foreach (int num in array)
            {
                str += (" " + num);
            }
            return str;
        }

        //Generates a new code of unique numbers, fills _code array with them.
        private static void GenerateCode()
        {
            //Random object used to generate random numbers
            Random random = new Random();
            //Arraylist that will hold the previous numbers in the code, to prevent repitition
            ArrayList previousNumbers = new ArrayList(4);

            //Fill code array with random numbers (under 10)
            //There is a requirement for no repeating numbers, so we put every generated number in an ArrayList, and check against it.
            for (int i = 0; i < _code.Length; i++)
            {
                _code[i] = random.Next(10); //random number under 10 (0-9)
                Debug.WriteLine(_code[i]);
                //keeps regenerating the number until it gets one that hasn't been used yet.
                while (previousNumbers.Contains(_code[i]))
                {
                    Debug.WriteLine(_code[i] + " is repeated, generating a new number...");
                    _code[i] = random.Next(10);

                }

                Debug.WriteLine("Number " + (i + 1) + ": " + _code[i]);
                //adds current number to previousNumbers
                previousNumbers.Add(_code[i]);
            }//end for
        }

        //Used by the 4 Entry controls, to ensure only 1 digit in each
        private void NumEntry_TextChanged(Entry sender, TextChangedEventArgs e) //I changed Object sender to Entry sender so I can change the text, e.newTextValue is read only.
        {
            //Debug.WriteLine(sender.Text.Length - 1);
            //If there is more than 1 character in the entry box, delete the last character
            if (e.NewTextValue.Length > 1)
            {
                Debug.WriteLine("More than one character detected, deleting last character");
                sender.Text = e.OldTextValue;
            }
            //Stop non-numerical characters being entered - C# equivalent of string.charAt() is string[index] 
            if (e.NewTextValue.Length > 0 && !char.IsDigit(e.NewTextValue[e.NewTextValue.Length - 1]))
            {
                Debug.WriteLine("Non-numerical character detected, deleting last character");
                sender.Text = e.OldTextValue;
            }

            //Move focus to the next element, so from text box to text box and then from the last one to the Enter Guess button.
            //SendKeys.Send("{TAB}"); //Doesn't work in xamarin, have to do 1 by 1 in each event handler or add to array and go through dynamically
        }

        private void BtnBack_Clicked(object sender, EventArgs e)
        {
            MainPage g = new MainPage();
            Application.Current.MainPage = new NavigationPage(g);
        }

    }
}