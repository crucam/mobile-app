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
    public partial class ViewTermPage : ContentPage
    {
        private SQLiteAsyncConnection _conn;
        private Term CurrTerm;
        private ObservableCollection<Course> _courseList;

        public ViewTermPage(Term selectedTerm)
        {
            InitializeComponent();

            CurrTerm = selectedTerm;
            _conn = DependencyService.Get<ISQLiteDb>().GetConnection();
            Title = selectedTerm.TermName;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            viewTermPage.Text = CurrTerm.TermName;
            viewTermStart.Text = CurrTerm.StartDate.ToString("MM/dd/yy");
            viewTermEnd.Text = CurrTerm.EndDate.ToString("MM/dd/yy");

            await _conn.CreateTableAsync<Course>();
            var courseList = await _conn.QueryAsync<Course>($"Select * from Courses where TermId='{CurrTerm.Id}'");
            _courseList = new ObservableCollection<Course>(courseList);
            courseListView.ItemsSource = _courseList;

            base.OnAppearing();

        }


        private async void addCourseBtn_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new AddCoursePage(CurrTerm));
        }

        private async void editTermBtn_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new EditTermPage(CurrTerm));
        }

        private async void dropTermBtn_Clicked(object sender, EventArgs e)
        {
            var confirm = await DisplayAlert("Alert", "Are you sure you want to drop this term?", "Yes", "No");
            if (confirm)
            {
                await _conn.DeleteAsync(CurrTerm);
                await Navigation.PopModalAsync();
            }
        }

        private async void homeBtn_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new MainPage());
        }

        private async void courseListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            Course course = (Course)e.SelectedItem;
            await Navigation.PushModalAsync(new ViewCourse(course));
        }
    }
}