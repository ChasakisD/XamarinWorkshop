using System;
using Xamarin.Forms;

namespace XamarinWorkshop
{
    public partial class MainPage : ContentPage
    {
        private int _counter;

        public MainPage()
        {
            InitializeComponent();
        }

        private void LoginButtonClicked(object sender, EventArgs e)
        {
            ClickedCountLabel.Text = $"You clicked {++_counter} times!";
        }
    }
}
