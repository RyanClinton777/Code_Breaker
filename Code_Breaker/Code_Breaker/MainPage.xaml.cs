using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Code_Breaker
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            AddImages();
        }

        private void AddImages()
        {
            var assembly = typeof(MainPage); //get current assembly
            string filename = "Code_Breaker.Images.lock.png"; //Find file
            imgMain.Source = ImageSource.FromResource(filename, assembly); //Set as source for imgMain
        }

        private void BtnPlay_Clicked(object sender, EventArgs e)
        {
            Game g = new Game();
            Application.Current.MainPage = new NavigationPage(g);
        }

        private void BtnScores_Clicked(object sender, EventArgs e)
        {
            Scoreboard s = new Scoreboard();
            Application.Current.MainPage = new NavigationPage(s);
        }
    }
}

