using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CoursePlanner_RevF
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NewTermPage : ContentPage
    {
        public MainPage _mainPage;
        private SQLiteAsyncConnection _conn;
        public NewTermPage(MainPage mainPage)
        {
            InitializeComponent();
            _mainPage = mainPage;
            _conn = DependencyService.Get<ISQLiteDb>().GetConnection();

        }

        private async void aSaveTermBtn_Clicked(object sender, EventArgs e)
        {
            if (aTermNameEntry.Text is null)
            {
                await DisplayAlert("Null Value", "Please enter a Term Name", "Okay");
                return;
            }

            if (aStartDatePicker.Date.Day < System.DateTime.Now.Day)
            {
                await DisplayAlert("Invalid Date", "Please choose a valid Start Date.", "Okay");
                return;
            }
            if (aEndDatePicker.Date.Day < aStartDatePicker.Date.Day)
            {
                await DisplayAlert("Invalid Date", "End date must be after start date.", "Okay");
                return;
            }

            var newTerm = new Term();

            newTerm.TermName = aTermNameEntry.Text;
            newTerm.StartDate = aStartDatePicker.Date;
            newTerm.EndDate = aEndDatePicker.Date;

            await _conn.InsertAsync(newTerm);
            _mainPage._termList.Add(newTerm);
            await Navigation.PopModalAsync();

        }
        private async void aCancelBtn_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }
    }
}