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

    public partial class AddCoursePage : ContentPage
    {
        private SQLiteAsyncConnection _conn;
        private Term _term;
        public AddCoursePage(Term selectedTerm)
        {
            InitializeComponent();
            _term = selectedTerm;
            _conn = DependencyService.Get<ISQLiteDb>().GetConnection();

        }

        private async void courseSave_Clicked(object sender, EventArgs e)
        {
            var course = new Course();
            course.CourseTitle = courseNameEntry.Text;
            course.StartDate = courseStart.Date;
            course.EndDate = courseEnd.Date;
            course.CourseStatus = (string)courseStatusPicker.SelectedItem;
            course.ProfName = profNameEntry.Text;
            course.ProfPhone = profPhoneEntry.Text;
            course.ProfEmail = profEmailEntry.Text;
            course.NotificationEnabled = enableNotifications.IsToggled ? 1 : 0;
            course.Notes = courseNotesEntry.Text;
            course.TermId = _term.Id;

            if (FieldCheck.IsNull(courseNameEntry.Text) &&
               FieldCheck.IsNull(profNameEntry.Text) &&
               FieldCheck.IsNull(profPhoneEntry.Text))
            {
                if (FieldCheck.IsValidEmail(profEmailEntry.Text))
                {
                    if (course.StartDate < course.EndDate)
                    {
                        await _conn.InsertAsync(course);
                        await Navigation.PopModalAsync();
                    }
                    else await DisplayAlert("Error.", "Start date cannot be before end date.", "Okay");
                }
                else await DisplayAlert("Error.", "All fields must be filled out correctly", "Okay");
            }
            else await DisplayAlert("Error.", "Please enter a valid email address", "Okay");
        }

        private async void courseCancel_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }
    }
}