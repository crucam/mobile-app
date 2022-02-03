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
    public partial class EditAssessment : ContentPage
    {

        private SQLiteAsyncConnection _conn;
        public Assessment _assessment;
        public EditAssessment(Assessment ass)
        {
            InitializeComponent();
            _assessment = ass;
            _conn = DependencyService.Get<ISQLiteDb>().GetConnection();
        }

        protected async override void OnAppearing()
        {
            await _conn.CreateTableAsync<Assessment>();

            editAssBar.Text = _assessment.AssessmentName;
            assNameEntry.Text = _assessment.AssessmentName;
            assStart.Date = _assessment.AssessmentStart;
            assEnd.Date = _assessment.AssessmentEnd;
            assNotes.Text = _assessment.AssessmentNotes;
            

            if (_assessment.NotificationEnabled == 1)
            {
                enableNotifications.IsToggled = true;
            }

            base.OnAppearing();
        }

        private async void assSave_Clicked(object sender, EventArgs e)
        {
            _assessment.AssessmentName = assNameEntry.Text;
            _assessment.AssessmentStart= assStart.Date;
            _assessment.AssessmentEnd= assEnd.Date;
            _assessment.NotificationEnabled = enableNotifications.IsToggled == true ? 1 : 0;
            _assessment.AssessmentNotes = assNotes.Text;

            if (FieldCheck.IsNull(assNameEntry.Text))
            {
                if (_assessment.AssessmentStart< _assessment.AssessmentEnd)
                {
                    await _conn.UpdateAsync(_assessment);
                    await Navigation.PopModalAsync();
                }
                else await DisplayAlert("Error.", "Please ensure start date is before end date.", "Okay");
            }
            else await DisplayAlert("Error.", "Please ensure all fields are completed.", "Okay");
        }

        private void assCancel_Clicked(object sender, EventArgs e)
        {
            Navigation.PopModalAsync();
        }
    }
}