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
    public partial class EditTermPage : ContentPage
    {
        Term SelectedTerm;
        public EditTermPage(Term selectedTerm)
        {
            InitializeComponent();

            this.SelectedTerm = selectedTerm;

            termNameLabel.Text = selectedTerm.TermName;
            termNameEntry.Text = selectedTerm.TermName;
            startDatePicker.Date = selectedTerm.StartDate;
            endDatePicker.Date = selectedTerm.EndDate;

        }

        private void saveTermBtn_Clicked(object sender, EventArgs e)
        {

            if (termNameEntry.Text is null)
            {
                DisplayAlert("Null Value", "Please enter a Term Name", "Okay");
                return;
            }

            if (startDatePicker.Date.Day < System.DateTime.Now.Day)
            {
                DisplayAlert("Invalid Date", "Please choose a valid Start Date.", "Okay");
                return;
            }
            if (endDatePicker.Date < startDatePicker.Date)
            {
                DisplayAlert("Invalid Date", "End date must be after start date.", "Okay");
                return;
            }
            SelectedTerm.TermName = termNameEntry.Text;
            SelectedTerm.StartDate = startDatePicker.Date;
            SelectedTerm.EndDate = endDatePicker.Date;

           
        }

        private async void cancelBtn_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new ViewTermPage(SelectedTerm));
        }
    }
}