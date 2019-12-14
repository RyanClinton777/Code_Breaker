using System;
using System.Diagnostics; //for debug.writeline
using System.IO;
using Xamarin.Forms;

namespace Code_Breaker
{
    //This class holds scores for the scoreboard
        class Score
        {
            private DateTime dtStart; //date and time the game was started
            private DateTime dtEnd; //date and time the game was finished
            private int guesses; //number of guesses made
            private bool success; //true if player won
            private String finalGuess; //final numbers eneterd by player
            private String actualCode; //What the code actually was

            static private int highScore = 999; //Holds the high score, lowest number of guesses in a successful game. Default is 999, anything above 5 is impossible.

            //Constructor proper data types
            public Score(DateTime dtStart, DateTime dtEnd, int guesses, bool success, string finalGuess, string actualCode)
            {
                this.dtStart = dtStart;
                this.dtEnd = dtEnd;
                this.guesses = guesses;
                this.success = success;
                this.finalGuess = finalGuess;
                this.actualCode = actualCode;
            }

            //Constructor all strings
            public Score(string dtStart, string dtEnd, string guesses, string success, string finalGuess, string actualCode)
            {
                //
            }

            //Prints score to file, the method to load them is in the scoreboard.xaml.cs
            public void SaveScore()
            {
                Debug.WriteLine(this.ToString()); //Debug

                //Gets the path of the folder for local app storage in the users environment
                string file = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Scores.txt");
                //string file = Path.Combine("C:\\", "Scores.txt"); //Denied access everywhere, UWP debugger issue?

                try
                {
                    //Debug
                    Debug.WriteLine("File exists? " + File.Exists(file));
                    //If file doesn't exist, create it, and insert column info as first line
                    if (!File.Exists(file))
                    {
                        //Insert Column info for first line, will be skipped when loading scores
                        File.WriteAllText(file, "DTStart;DTEnd;Guesses;Success;FinalGuess;ActualCode" + Environment.NewLine);
                        //Insert score info
                        File.AppendAllText(file, this.ToString() + Environment.NewLine);
                        Debug.WriteLine("File not found - creating file"); //Debug
                    }
                    //Else append to existing file
                    else
                    {
                        //Insert score info
                        File.AppendAllText(file, this.ToString() + Environment.NewLine);
                        Debug.WriteLine("File found - append to file"); //Debug
                    }
                    //Debug
                    Debug.WriteLine("Writing to file - Path:" + Environment.SpecialFolder.LocalApplicationData);
                }
                //Score save fails, alert user
                catch (Exception e)
                {
                    //Debug
                    Debug.WriteLine("FAILED TO CREATE/WRITE TO FILE");
                    App.Current.MainPage.DisplayAlert("Save Failed", "Failed to record score - Exception: " + e.Message, "OK");
                    return;
                }

            }

            //Returns Score data in a String seperated by semicolons ;
            override
            public string ToString()
            {
                return (dtStart.ToString() + ";" + dtEnd.ToString() + ";" + guesses + ";" + success + ";" + finalGuess + ";" + actualCode);
            }

            //Is a setter and getter, gets the high score from the scores.txt file, sets highScore as it and returns it.
            //Updates the label at the top right of the screen (lblHighScore)
            public static string GetHighScore()
            {
                //Get file location
                string file = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Scores.txt");

                //If the scores file exists
                if (File.Exists(file))
                {
                    //Columns for reference - DTStart; DTEnd; Guesses; Success; FinalGuess; ActualCode

                    //Create list of strings
                    System.Collections.Generic.List<string> list = new System.Collections.Generic.List<string>();

                    //Create a streamreader, which is used to read text from a file
                    //The "using" keyword will make sure the passed in IDisposable Object is discarded from memory after the loop
                    using (StreamReader sr = new StreamReader(file))
                    {
                        while (sr.Peek() >= 0) //peek checks next character, 0 is ASCII code for null, this just runs it until it gets to the end
                        {
                            //add each line to the list, will end up with a list containing each row/line
                            list.Add(sr.ReadLine());
                        }
                    }

                    //For every item in the list (except the first), create Score object with the data and add to the _scores List
                    for (int i = 1; i < list.Count; i++) //starts at 1 to skip the line with the column info
                    {
                        //Create String array, fill with column values from this row by splitting the string
                        string[] strlist = list[i].Split(';'); //string.split() method splits the string, using ; as the delimiter


                        Debug.WriteLine("i = " + i + " - " + strlist[3]);
                        Debug.WriteLine(strlist[3].Equals("True"));
                        //If the player won
                        if (strlist[3].Equals("True"))
                        {
                            //get number of guesses for this score, compare to highscore
                            int score = Int32.Parse(strlist[2]);
                            Debug.WriteLine("Score: " + score);
                            //If this score is lower than the current highscore (won in less guesses), becomes new highScore
                            if (score < highScore) highScore = score;
                        }
                    }

                    Debug.WriteLine("Highscore: " + highScore);

                    //If highscore is still 999, there have been no winning games, and there is no highscore
                    if (highScore == 999) return "N/A";
                    //else return highscore
                    else return highScore.ToString()+" guess(es)";

                }//end outer if
                 //else there must be no scores, return N/A
                else
                {
                    return "N/A";
                }

            }// end method

        }//end of class
    }
