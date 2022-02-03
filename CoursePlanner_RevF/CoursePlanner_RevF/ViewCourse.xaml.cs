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
    public partial class ViewCourse : ContentPage
    {
        private SQLiteAsyncConnection _conn;
        private Course CurrCourse;
        public ViewCourse(Course selectedCourse)
        {
            InitializeComponent();

            CurrCourse = selectedCourse;
            _conn = DependencyService.Get<ISQLiteDb>().GetConnection();
        }
        protected async override void OnAppearing()
        {
            await _conn.CreateTableAsync<Course>();

            vCourseNameEntry.Text = CurrCourse.CourseTitle.ToString();
            vCourseStart.Date = CurrCourse.StartDate.Date;
            vCourseEnd.Date = CurrCourse.EndDate;
            vProfNameEntry.Text = CurrCourse.ProfName.ToString();
            vProfPhoneEntry.Text = CurrCourse.ProfPhone.ToString();
            vProfEmailEntry.Text = CurrCourse.ProfEmail.ToString();
            vCourseNotes.Text = CurrCourse.Notes;
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

            vCourseStatusPicker.SelectedIndex = statusIndex;


            base.OnAppearing();
        }  

        private async void vAssessments_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new ViewAssessments(CurrCourse));
        }
        private async void vCourseEdit_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new EditCourse(CurrCourse));
        }
        private async void vCourseCancel_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }

        private async void vDrop_Clicked(object sender, EventArgs e)
        {
            var confirm = await DisplayAlert("Alert", "Are you sure you want to drop this course?", "Yes", "No");
            if (confirm)
            {
                await _conn.DeleteAsync(CurrCourse);
                await Navigation.PopModalAsync();
            }
        }
    }
}