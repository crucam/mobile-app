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
    public partial class AddAssessment : ContentPage
    {
        private SQLiteAsyncConnection _conn;
        public Course _course;
        public AddAssessment(Course course)
        {
            InitializeComponent();
            _course = course;
            _conn = DependencyService.Get<ISQLiteDb>().GetConnection();
        }

        protected override async void OnAppearing()
        {

            await _conn.CreateTableAsync<Assessment>();
            var objectiveCount = await _conn.QueryAsync<Assessment>($"Select AssessmentType From Assessments Where CourseId = '{_course.Id}' And AssessmentType = 'Objective'");
            var performanceCount = await _conn.QueryAsync<Assessment>($"Select AssessmentType From Assessments Where CourseId = '{_course.Id}' And AssessmentType = 'Performance'");
            if (objectiveCount.Count == 0)
            {
                assTypePicker.Items.Add("Objective");
            }
            if (performanceCount.Count == 0)
            {
                assTypePicker.Items.Add("Performance");
            }
            if (objectiveCount.Count == 1)
            {
                assTypePicker.Items.Remove("Objective");
            }
            if (performanceCount.Count == 1)
            {
                assTypePicker.Items.Remove("Performance");
            }
            base.OnAppearing();
        }

        private async void assSave_Clicked(object sender, EventArgs e)
        {
            var ass = new Assessment();
            ass.AssessmentName = assNameEntry.Text.ToString();
            ass.AssessmentStart = assStart.Date;
            ass.AssessmentEnd = assEnd.Date;
            ass.CourseId = _course.Id;
            ass.AssessmentType = (string)assTypePicker.SelectedItem;
            ass.NotificationEnabled = EnableNotifications.IsToggled ? 1 : 0;

            if (FieldCheck.IsNull(assNameEntry.Text))
            {
                if (ass.AssessmentStart< ass.AssessmentEnd)
                {
                    await _conn.InsertAsync(ass);
                    await Navigation.PopModalAsync();
                }
                else await DisplayAlert("Error.", "Please ensure start date is before end date.", "Okay");
            }
            else await DisplayAlert("Error.", "Please ensure all fields are completed.", "Okay");
        }

        private async void assCancel_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }
    }
}