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
    public partial class EditCourse : ContentPage
    {
        private SQLiteAsyncConnection _conn;
        private Course CurrCourse;
        public EditCourse(Course course)
        {
            InitializeComponent();
            CurrCourse = course;
            _conn = DependencyService.Get<ISQLiteDb>().GetConnection();
        }

        protected override void OnAppearing()
        {
            courseTitleBar.Text = CurrCourse.CourseTitle.ToString();
            eCourseNameEntry.Text = CurrCourse.CourseTitle.ToString();
            eCourseStart.Date = CurrCourse.StartDate.Date;
            eCourseEnd.Date = CurrCourse.EndDate;
            eProfNameEntry.Text = CurrCourse.ProfName.ToString();
            eProfPhoneEntry.Text = CurrCourse.ProfPhone.ToString();
            eProfEmailEntry.Text = CurrCourse.ProfEmail.ToString();
            eCourseNotes.Text = CurrCourse.Notes;
            if (CurrCourse.NotificationEnabled == 1)
            {
                enableNotifications.IsToggled = true;
            }

            int statusIndex = 0;

            if (CurrCourse.CourseStatus == "In Progress")
            {
                statusIndex = 0;
            }
            else if (CurrCourse.CourseStatus == "Plan to Take")
            {
                statusIndex = 1;
            }
            else if (CurrCourse.CourseStatus == "Completed")
            {
                statusIndex = 2;
            }
            else if (CurrCourse.CourseStatus == "Dropped")
            {
                statusIndex = 3;
            }

            eCourseStatusPicker.SelectedIndex = statusIndex;
            base.OnAppearing();
        }

        private async void saveAssBtn_Clicked(object sender, EventArgs e)
        {
            CurrCourse.CourseTitle = eCourseNameEntry.Text;
            CurrCourse.StartDate = eCourseStart.Date;
            CurrCourse.EndDate = eCourseEnd.Date;
            CurrCourse.ProfName = eProfNameEntry.Text;
            CurrCourse.ProfPhone = eProfPhoneEntry.Text;
            CurrCourse.ProfEmail = eProfEmailEntry.Text;
            CurrCourse.Notes = eCourseNotes.Text;
            CurrCourse.CourseStatus = (String)eCourseStatusPicker.SelectedItem;
            CurrCourse.NotificationEnabled = enableNotifications.IsToggled ? 1 : 0;

            if (FieldCheck.IsNull(eCourseNameEntry.Text) &&
               FieldCheck.IsNull(eProfNameEntry.Text) &&
               FieldCheck.IsNull(eProfPhoneEntry.Text))
            {
                if (FieldCheck.IsValidEmail(eProfEmailEntry.Text))
                {
                    if (CurrCourse.StartDate < CurrCourse.EndDate)
                    {
                        await _conn.UpdateAsync(CurrCourse);
                        await Navigation.PopModalAsync();
                    }
                    else await DisplayAlert("Error.", "Start date cannot be before end date.", "Okay");
                }
                else await DisplayAlert("Error.", "All fields must be filled out correctly", "Okay");
            }
            else await DisplayAlert("Error.", "Please provide a valid email address", "Okay");
        }

        private async void addAssBtn_Clicked(object sender, EventArgs e)
        {

        }

        private async void eCourseCancel_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }

    }
}