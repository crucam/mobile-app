using Plugin.LocalNotifications;
using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace CoursePlanner_RevF
{
    [Table("Terms")]
    public class Term
    {
        [PrimaryKey, AutoIncrement, Column("_id")]
        public int Id { get; set; }

        public string TermName { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
    }
    [Table("Courses")]
    public class Course
    {
        [PrimaryKey, AutoIncrement, Column("_id")]
        public int Id { get; set; }
        public int TermId { get; set; }
        public string CourseTitle { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string CourseStatus { get; set; }
        public string ProfName { get; set; }
        public string ProfEmail { get; set; }
        public string ProfPhone { get; set; }
        public string Notes { get; set; }
        public int NotificationEnabled { get; set; }
    }
    [Table("Assessments")]
    public class Assessment
    {
        [PrimaryKey, AutoIncrement, Column("_id")]
        public int Id { get; set; }
        public string AssessmentName { get; set; }
        public DateTime AssessmentStart { get; set; }
        public DateTime AssessmentEnd { get; set; }
        public string AssessmentType { get; set; }
        public int CourseId { get; set; }
        public string AssessmentNotes { get; set; }
        public int NotificationEnabled { get; set; }
    }

    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        private SQLiteAsyncConnection _conn;
        public ObservableCollection<Term> _termList;
        private bool pushNotification = true;
        public MainPage()
        {
            InitializeComponent();
            _conn = DependencyService.Get<ISQLiteDb>().GetConnection();

        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            //Create all tables, if already created will be ignored
            await _conn.CreateTableAsync<Term>();
            await _conn.CreateTableAsync<Course>();
            await _conn.CreateTableAsync<Assessment>();

            var termList = await _conn.Table<Term>().ToListAsync();
            var courseList = await _conn.Table<Course>().ToListAsync();
            var assessmentList = await _conn.Table<Assessment>().ToListAsync();

            //Create new term if the term table is empty, populate it with the appropriate default information
            if (!termList.Any())
            {
                //Create default term
                var defTerm = new Term();
                defTerm.TermName = "Term 1";
                defTerm.StartDate = new DateTime(2022, 01, 01);
                defTerm.EndDate = new DateTime(2022, 06, 30);
                await _conn.InsertAsync(defTerm);
                termList.Add(defTerm);

                //Create default course
                var defCourse = new Course();
                defCourse.CourseTitle = "Mobile Applications";
                defCourse.StartDate = new DateTime(2022, 01, 01);
                defCourse.EndDate = new DateTime(2022, 02, 01);
                defCourse.CourseStatus = "In Progress";
                defCourse.ProfName = "Cameron Cruz";
                defCourse.ProfEmail = "ccruz75@wgu.edu";
                defCourse.ProfPhone = "760-887-0923";
                defCourse.Notes = "Last class!";
                defCourse.TermId = defTerm.Id;
                await _conn.InsertAsync(defCourse);

                //Create Objective Assessment
                var defObjAssessment = new Assessment();
                defObjAssessment.AssessmentName = "Obj Test Assessment";
                defObjAssessment.AssessmentStart = new DateTime(2022, 01, 02);
                defObjAssessment.AssessmentEnd = new DateTime(2022, 02, 01);
                defObjAssessment.AssessmentType = "Objective Assessment";
                defObjAssessment.CourseId = defCourse.Id;
                defObjAssessment.NotificationEnabled = 1;
                await _conn.InsertAsync(defObjAssessment);


                //Create Performance Assessment
                var defPerfAssessment = new Assessment();
                defPerfAssessment.AssessmentName = "Perf Test Assessment";
                defPerfAssessment.AssessmentStart = new DateTime(2022, 01, 02);
                defPerfAssessment.AssessmentEnd = new DateTime(2022, 02, 01);
                defPerfAssessment.AssessmentType = "Performance Assessment";
                defPerfAssessment.CourseId = defCourse.Id;
                defPerfAssessment.NotificationEnabled = 1;
                await _conn.InsertAsync(defPerfAssessment);
            }

            //Notifications
            if (pushNotification == true)
            {
                pushNotification = false;
                int courseId = 0;
                foreach (Course course in courseList)
                {
                    courseId++;
                    if (course.NotificationEnabled == 1)
                    {
                        if (course.StartDate == DateTime.Today)
                            CrossLocalNotifications.Current.Show("Reminder", $"{course.CourseTitle} starts today!", courseId);
                        if (course.EndDate == DateTime.Today)
                            CrossLocalNotifications.Current.Show("Reminder", $"{course.CourseTitle} ends today!", courseId);
                    }
                }

                int assessmentId = courseId;
                foreach (Assessment assessment in assessmentList)
                {
                    assessmentId++;
                    if (assessment.NotificationEnabled == 1)
                    {
                        if (assessment.AssessmentEnd == DateTime.Today)
                            CrossLocalNotifications.Current.Show("Reminder", $"{assessment.AssessmentName} begins today!", assessmentId);
                        if (assessment.AssessmentEnd == DateTime.Today)
                            CrossLocalNotifications.Current.Show("Reminder", $"{assessment.AssessmentName} ends today!", assessmentId);
                    }
                }
            }

            _termList = new ObservableCollection<Term>(termList);
            termListView.ItemsSource = _termList;
            base.OnAppearing();
        }

        private async void addTermBtn_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new NewTermPage(this));
        }

        private async void termListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var selectedTerm = termListView.SelectedItem as Term;

            if (selectedTerm != null)
            {
                await Navigation.PushModalAsync(new ViewTermPage(selectedTerm));
            }
        }
    }
}
