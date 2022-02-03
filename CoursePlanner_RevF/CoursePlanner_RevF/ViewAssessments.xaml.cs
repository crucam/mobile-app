using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CoursePlanner_RevF
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ViewAssessments : ContentPage
    {
        private Course _currentCourse;
        private SQLiteAsyncConnection _conn;
        private ObservableCollection<Assessment> _assessmentList;
        public ViewAssessments(Course currentCourse)
        {
            InitializeComponent();
            CourseName.Text = currentCourse.CourseTitle;
            _currentCourse = currentCourse;
            _conn = DependencyService.Get<ISQLiteDb>().GetConnection();
        }

        protected override async void OnAppearing()
        {
            CourseName.Text = _currentCourse.CourseTitle;
            await _conn.CreateTableAsync<Assessment>();
            var assessmentList = await _conn.QueryAsync<Assessment>($"Select * From Assessments Where CourseId = '{_currentCourse.Id}'");
            _assessmentList = new ObservableCollection<Assessment>(assessmentList);
            assessmentListView.ItemsSource = _assessmentList;
            base.OnAppearing();
        }

        private async void addAssBtn_Clicked(object sender, EventArgs e)
        {
            await _conn.CreateTableAsync<Assessment>();
            var assessmentCount = await _conn.QueryAsync<Assessment>($"Select AssessmentType From Assessments Where CourseId = '{_currentCourse.Id}'");
            if (assessmentCount.Count == 2)
            {
                await DisplayAlert("Alert", "You cannot add more than two exams. Please remove an exam and try again.", "Okay");
            }
            else await Navigation.PushModalAsync(new AddAssessment(_currentCourse));
        }

        private async void assessmentListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            Assessment assessment = (Assessment)e.SelectedItem;
            await Navigation.PushModalAsync(new AssessmentDetail(assessment));
        }

        private async void back_Clicked(object sender, EventArgs e)
        {
            Navigation.PopModalAsync();
        }
    }
}