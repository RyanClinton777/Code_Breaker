using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Xamarin.Forms;

namespace Code_Breaker
{
    //[XamlCompilation(XamlCompilationOptions.Compile)]

    public partial class Scoreboard : ContentPage
    {
        //List of Scores to be populated every time the page is opened in the LoadScores method, with be used to populate the listview.
        private List<Score> _scores = new List<Score>();

        public Scoreboard()
        {
            InitializeComponent();
            LoadScores();
        }

        //Load scores from file
        private void LoadScores()
        {
            //Get file location
            string file = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Scores.txt");

            //If the scores file exists
            if (File.Exists(file))
            {
                //Columns for reference - DTStart; DTEnd; Guesses; Success; FinalGuess; ActualCode

                //Create list of strings
                List<string> list = new List<string>();

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

                    //Create a Score object using this data
                    //I created an overloaded constructor that takes all strings and converts them there, to save having loads of code here.
                    _scores.Add(new Score(strlist[0], strlist[1], strlist[2], strlist[3], strlist[4], strlist[5]));

                    //For each column
                    for (int col = 0; col < 6; col++)
                    {

                        GridScores.Children.Add(new Label
                        { Text = strlist[col], HorizontalOptions = LayoutOptions.Center }, col, i);
                        Debug.WriteLine(strlist[col]);
                        //                        var Label = new Label { Text = strlist[col], FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)) Grid.SetRow., Grid.SetColumn(col) };

                    }
                }
            }
            //Else no scores yet, notify user
            else
            {
                //Notify user
                DisplayAlert("No scores found", "There are no record scores yet, go finish a game to make some!", "OK");
                Debug.WriteLine("Scores.txt not found - No games finished yet, or it isn't saving them properly");
            }

        }

        private void BtnBack_Clicked(object sender, EventArgs e)
        {
            MainPage g = new MainPage();
            Application.Current.MainPage = new NavigationPage(g);
        }
    }
}