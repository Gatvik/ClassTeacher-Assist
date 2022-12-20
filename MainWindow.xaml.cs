using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using ClassTeacher_Assist.Models;
using Microsoft.EntityFrameworkCore;
using static System.Windows.Forms.Application;

namespace ClassTeacher_Assist
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static Teacher? currentTeacher;

        public MainWindow()
        {
            InitializeComponent();
            Authorization();
        }

        public void Authorization()
        {
            PostgresContext db = new PostgresContext();
            var auth = new AuthorizationWindow();
            bool? isAuthorized = auth.ShowDialog();

            if(isAuthorized != true) 
            {
                Close();
                return;
            }

            currentTeacher = Common.CurrentUser;

            

            UpdateInfoTextBox();
        }

        public void UpdateInfoTextBox()
        {
            PostgresContext db = new PostgresContext();
            InfoTextBox.Text = $"Добрий день, {currentTeacher?.FirstName} \n Ваш клас: {currentTeacher.Class.ClassCode} Кількість учнів: {currentTeacher.Class.StudentsNumber}";
        }

        private void QuitButton_Click(object sender, RoutedEventArgs e)
        {
            //System.Windows.Forms.Application
            Restart();
            Application.Current.Shutdown();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TablesComboBox_SelectionChanged(null, null);
        }

        private void FillDataGridWith<T>(List<DataGridTextColumn> columns, List<T> items, DataGrid dataGrid)
        {
            dataGrid.Columns.Clear();
            dataGrid.Items.Clear();
            columns.ForEach(column => dataGrid.Columns.Add(column));
            items.ForEach(student => dataGrid.Items.Add(student));
        }

        private void TablesComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ChangeDataGrid();
        }
        private void Filtering_Changed(object sender, RoutedEventArgs e)
        {
            ChangeDataGrid();
        }

        private void ChangeDataGrid()
        {
            PostgresContext db = new PostgresContext();
            string? selection = (TablesComboBox.SelectedItem as ComboBoxItem)?.Content as string;
            if (selection is null) return;
            //try { StatsComboBox.SelectedIndex = 0; } catch { }

            switch (selection)
            {
                case "Учні":
                    try
                    {
                        List<Student> studentsOfClass = new();

                        if (SelfClassRadioButton.IsChecked == true) 
                            studentsOfClass = db.Students.AsNoTracking().Include(s => s.Class).Where(st => st.ClassId == currentTeacher.Class.ClassId).ToList();
                        else if (AllClassesRadioButton.IsChecked == true)
                            studentsOfClass = db.Students.AsNoTracking().Include(s => s.Class).ToList();

                        if (SortAscRadioButton.IsChecked == true)
                            studentsOfClass = studentsOfClass.OrderBy(st => st.LastName).ToList();
                        else if (SortDescRadioButton.IsChecked == true)
                            studentsOfClass = studentsOfClass.OrderByDescending(st => st.LastName).ToList();

                        FillDataGridWith<Student>(columns[selection], studentsOfClass, MainDataGrid);
                        CurrentTableTextBox.Text = "Учні";
                    }
                    catch {}
                    break;
                case "Викладання":
                    List<Teaching> teachersOfClass = new();
                    if (SelfClassRadioButton.IsChecked == true)
                    {
                        teachersOfClass = db.Teachings.AsNoTracking().Include(teaching => teaching.Teacher).ThenInclude(teacher => teacher.Subject).AsNoTracking().Include(teaching => teaching.Class).AsNoTracking()
                        .Where(teaching => teaching.ClassId == currentTeacher.Class.ClassId).ToList();
                    }
                    else if (AllClassesRadioButton.IsChecked == true)
                    {
                        teachersOfClass = db.Teachings.AsNoTracking().Include(teaching => teaching.Teacher).ThenInclude(teacher => teacher.Subject).AsNoTracking()
                            .Include(teaching => teaching.Class).AsNoTracking().ToList();
                    }

                    if (SortAscRadioButton.IsChecked == true)
                        teachersOfClass = teachersOfClass.OrderBy(t => t.Teacher.LastName).ToList();
                    else if (SortDescRadioButton.IsChecked == true)
                        teachersOfClass = teachersOfClass.OrderByDescending(t => t.Teacher.LastName).ToList();

                    FillDataGridWith<Teaching>(columns[selection], teachersOfClass, MainDataGrid);
                    CurrentTableTextBox.Text = "Викладання";
                    break;
                case "Оцінки":
                    List<Grade> gradesOfClass = new();
                    if (SelfClassRadioButton.IsChecked == true)
                    {
                        gradesOfClass = db.Grades.AsNoTracking().Include(grade => grade.Student).ThenInclude(student => student.Class)
                            .Include(grade => grade.Teacher).ThenInclude(teacher => teacher.Subject).AsNoTracking()
                        .Where(grade => grade.Student.ClassId == currentTeacher.Class.ClassId).ToList();
                    }
                    else if (AllClassesRadioButton.IsChecked == true)
                    {
                        gradesOfClass = db.Grades.AsNoTracking().Include(grade => grade.Student).ThenInclude(student => student.Class)
                            .Include(grade => grade.Teacher).ThenInclude(teacher => teacher.Subject).AsNoTracking().ToList();
                    }

                    if (SortAscRadioButton.IsChecked == true)
                        gradesOfClass = gradesOfClass.OrderBy(g => g.Value).ToList();
                    else if (SortDescRadioButton.IsChecked == true)
                        gradesOfClass = gradesOfClass.OrderByDescending(g => g.Value).ToList();

                    FillDataGridWith<Grade>(columns[selection], gradesOfClass, MainDataGrid);
                    CurrentTableTextBox.Text = "Оцінки";
                    break;
                case "Пропуски":
                    List <Skip> skipsOfClass = new();
                    if (SelfClassRadioButton.IsChecked == true)
                    {
                        skipsOfClass = db.Skips.AsNoTracking().Include(skip => skip.Student).ThenInclude(student => student.Class)
                            .Include(skip => skip.Teacher).AsNoTracking()
                            .Where(skip => skip.Student.ClassId == currentTeacher.Class.ClassId).ToList();
                    }
                    else if (AllClassesRadioButton.IsChecked == true)
                    {
                        skipsOfClass = db.Skips.AsNoTracking().Include(skip => skip.Student).ThenInclude(student => student.Class)
                            .Include(skip => skip.Teacher).AsNoTracking().ToList();
                    }
                    FillDataGridWith<Skip>(columns[selection], skipsOfClass, MainDataGrid);
                    CurrentTableTextBox.Text = "Пропуски";
                    break;
                case "Усі вчителі":
                    AllClassesRadioButton.IsChecked = true;

                    var allTeachers = db.Teachers.AsNoTracking().Include(t => t.Class).AsNoTracking().Include(teacher => teacher.Subject).AsNoTracking().ToList();

                    if (SortAscRadioButton.IsChecked == true)
                        allTeachers = allTeachers.OrderBy(t => t.LastName).ToList();
                    else if (SortDescRadioButton.IsChecked == true)
                        allTeachers = allTeachers.OrderByDescending(t => t.LastName).ToList();

                    FillDataGridWith<Teacher>(columns[selection], allTeachers, MainDataGrid);;
                    CurrentTableTextBox.Text = "Усі вчителі";
                    break;
                case "Усі предмети":
                    AllClassesRadioButton.IsChecked = true;

                    var allSubjects = db.Subjects.AsNoTracking().ToList();
                    FillDataGridWith<Subject>(columns[selection], allSubjects, MainDataGrid); ;
                    CurrentTableTextBox.Text = "Усі вчителі";
                    break;
                case "Усі класи":
                    AllClassesRadioButton.IsChecked = true;

                    var allClasses = db.Classes.AsNoTracking().Include(c => c.Teacher).AsNoTracking().ToList();

                    if (SortAscRadioButton.IsChecked == true)
                        allClasses = allClasses.OrderBy(c => c.StudentsNumber).ToList();
                    else if (SortDescRadioButton.IsChecked == true)
                        allClasses = allClasses.OrderByDescending(c => c.StudentsNumber).ToList();

                    FillDataGridWith<Class>(columns[selection], allClasses, MainDataGrid); ;
                    CurrentTableTextBox.Text = "Усі вчителі";
                    break;
            }
        }

        

        private Dictionary<string, List<DataGridTextColumn>> columns = new Dictionary<string, List<DataGridTextColumn>>()
        {
            { "Учні", new List<DataGridTextColumn>()
                {
                    new DataGridTextColumn() { Binding = new Binding("FirstName"), Header = "Ім'я" },
                    new DataGridTextColumn() { Binding = new Binding("LastName"), Header = "Прізвище" },
                    new DataGridTextColumn() { Binding = new Binding("Patronymic"), Header = "По-батькові" },
                    new DataGridTextColumn() { Binding = new Binding("Class.ClassCode"), Header = "Клас" },
                    new DataGridTextColumn() { Binding = new Binding("Gender"), Header = "Стать" },
                    new DataGridTextColumn() { Binding = new Binding("PhoneNumber"), Header = "Номер телефону" },
                    new DataGridTextColumn() { Binding = new Binding("Address"), Header = "Адреса"},
                    new DataGridTextColumn() { Binding = new Binding("Email"), Header = "Ел. пошта"},
                }
            },
            { "Викладання",  new List<DataGridTextColumn>()
                {
                    new DataGridTextColumn() { Binding = new Binding("Teacher.FirstName"), Header = "Ім'я" },
                    new DataGridTextColumn() { Binding = new Binding("Teacher.LastName"), Header = "Прізвище" },
                    new DataGridTextColumn() { Binding = new Binding("Teacher.Patronymic"), Header = "По-батькові" },
                    new DataGridTextColumn() { Binding = new Binding("Teacher.Gender"), Header = "Стать" },
                    new DataGridTextColumn() { Binding = new Binding("Class.ClassCode"), Header = "Клас" },
                    new DataGridTextColumn() { Binding = new Binding("Teacher.Subject.Name"), Header = "Предмет"},
                }
            },
            { "Оцінки", new List<DataGridTextColumn>()
                {
                    new DataGridTextColumn() { Binding = new Binding("Student.FirstName"), Header = "Ім'я учня" },
                    new DataGridTextColumn() { Binding = new Binding("Student.LastName"), Header = "Прізвище учня" },
                    new DataGridTextColumn() { Binding = new Binding("Student.Patronymic"), Header = "По-батькові учня" },
                    new DataGridTextColumn() { Binding = new Binding("Student.Class.ClassCode"), Header = "Клас" },
                    new DataGridTextColumn() { Binding = new Binding("Teacher.FirstName"), Header = "Ім'я вчителя" },
                    new DataGridTextColumn() { Binding = new Binding("Teacher.LastName"), Header = "Прізвище вчителя" },
                    new DataGridTextColumn() { Binding = new Binding("Teacher.Patronymic"), Header = "По-батькові вчителя" },
                    new DataGridTextColumn() { Binding = new Binding("Teacher.Subject.Name"), Header = "Предмет" },
                    new DataGridTextColumn() { Binding = new Binding("Value"), Header = "Бал" },
                    new DataGridTextColumn() { Binding = new Binding("ReceivingDateNormal"), Header = "Дата виставлення" },
                } 
            },
            { "Пропуски", new List<DataGridTextColumn>()
                {
                    new DataGridTextColumn() { Binding = new Binding("Student.FirstName"), Header = "Ім'я учня" },
                    new DataGridTextColumn() { Binding = new Binding("Student.LastName"), Header = "Прізвище учня" },
                    new DataGridTextColumn() { Binding = new Binding("Student.Patronymic"), Header = "По-батькові учня" },
                    new DataGridTextColumn() { Binding = new Binding("Student.Class.ClassCode"), Header = "Клас" },
                    new DataGridTextColumn() { Binding = new Binding("Teacher.FirstName"), Header = "Ім'я вчителя" },
                    new DataGridTextColumn() { Binding = new Binding("Teacher.LastName"), Header = "Прізвище вчителя" },
                    new DataGridTextColumn() { Binding = new Binding("Teacher.Patronymic"), Header = "По-батькові вчителя" },
                    new DataGridTextColumn() { Binding = new Binding("Reason"), Header = "Причина" },
                    new DataGridTextColumn() { Binding = new Binding("Description"), Header = "Опис" },
                    new DataGridTextColumn() { Binding = new Binding("ReceivingDateNormal"), Header = "Дата виставлення" },
                }
            },
            { "Усі вчителі", new List<DataGridTextColumn>()
                {
                    new DataGridTextColumn() { Binding = new Binding("FirstName"), Header = "Ім'я" },
                    new DataGridTextColumn() { Binding = new Binding("LastName"), Header = "Прізвище" },
                    new DataGridTextColumn() { Binding = new Binding("Patronymic"), Header = "По-батькові" },
                    new DataGridTextColumn() { Binding = new Binding("Class.ClassCode"), Header = "Класний керівник класу" },
                    new DataGridTextColumn() { Binding = new Binding("Gender"), Header = "Стать" },
                    new DataGridTextColumn() { Binding = new Binding("PhoneNumber"), Header = "Номер телефону" },
                    new DataGridTextColumn() { Binding = new Binding("Address"), Header = "Адреса"},
                    new DataGridTextColumn() { Binding = new Binding("Email"), Header = "Ел. пошта"},
                    new DataGridTextColumn() { Binding = new Binding("Subject.Name"), Header = "Предмет"},
                }
            },
            { "Усі предмети", new List<DataGridTextColumn>()
                {
                    new DataGridTextColumn() { Binding = new Binding("Name"), Header = "Назва" },
                    new DataGridTextColumn() { Binding = new Binding("Code"), Header = "Код" }
                }
            },
            { "Усі класи", new List<DataGridTextColumn>()
                {
                    new DataGridTextColumn() { Binding = new Binding("ClassCode"), Header = "Код класу" },
                    new DataGridTextColumn() { Binding = new Binding("StudentsNumber"), Header = "Кілкькість учнів" },
                    new DataGridTextColumn() { Binding = new Binding("Teacher.FullName"), Header = "ПІБ класного керівника" },
                }
            },
            {
                "Кількість пропусків студента", new List<DataGridTextColumn>()
                {
                    new DataGridTextColumn() { Binding = new Binding("LastName"), Header = "Прізвище" },
                    new DataGridTextColumn() { Binding = new Binding("FirstName"), Header = "Ім'я" },
                    new DataGridTextColumn() { Binding = new Binding("Patronymic"), Header = "По-батькові" },
                    new DataGridTextColumn() { Binding = new Binding("Class.ClassCode"), Header = "Клас" },
                    new DataGridTextColumn() { Binding = new Binding("Skips.Count"), Header = "Кількість пропусків" },
                }
            },
            {
                "Кращі учні за оцінками", new List<DataGridTextColumn>()
                {
                    new DataGridTextColumn() { Binding = new Binding("LastName"), Header = "Прізвище" },
                    new DataGridTextColumn() { Binding = new Binding("FirstName"), Header = "Ім'я" },
                    new DataGridTextColumn() { Binding = new Binding("Patronymic"), Header = "По-батькові" },
                    new DataGridTextColumn() { Binding = new Binding("Class.ClassCode"), Header = "Клас" },
                    new DataGridTextColumn() { Binding = new Binding("SumOfGrades"), Header = "Сума оцінок" },
                }
            },
            {
                "Найпопулярніщі предмети серед вчителів", new List<DataGridTextColumn>()
                {
                    new DataGridTextColumn() { Binding = new Binding("Name"), Header = "Предмет" },
                    new DataGridTextColumn() { Binding = new Binding("Teachers.Count"), Header = "Кількість викладачів" },
                }
            }
            
        };

        private void AddStudentButton_Click(object sender, RoutedEventArgs e)
        {
            var window = new AddStudentWindow(currentTeacher);
            window.ShowDialog();
            TablesComboBox_SelectionChanged(null, null);
            UpdateInfoTextBox();
        }

        private void DeleteStudentButton_Click(object sender, RoutedEventArgs e)
        {
            var window = new DeleteStudentWindow(currentTeacher);
            window.ShowDialog();
            TablesComboBox_SelectionChanged(null, null);
            UpdateInfoTextBox();
        }

        private void EditStudentButton_Click(object sender, RoutedEventArgs e)
        {
            var window = new EditStudentWindow(currentTeacher);
            window.ShowDialog();
            TablesComboBox_SelectionChanged(null, null);
            UpdateInfoTextBox();
        }

        private void AddSkipButton_Click(object sender, RoutedEventArgs e)
        {
            var window = new AddSkipWindow(currentTeacher);
            window.ShowDialog();
            TablesComboBox_SelectionChanged(null, null);
        }

        private void DeleteSkipButton_Click(object sender, RoutedEventArgs e)
        {
            var window = new DeleteSkipWindow(currentTeacher);
            window.ShowDialog();
            TablesComboBox_SelectionChanged(null, null);
        }

        private void AddGradeButton_Click(object sender, RoutedEventArgs e)
        {
            var window = new AddGradeWindow(currentTeacher);
            window.ShowDialog();
            TablesComboBox_SelectionChanged(null, null);
        }

        private void DeleteGradeButton_Click(object sender, RoutedEventArgs e)
        {
            var window = new DeleteGradeWindow(currentTeacher);
            window.ShowDialog();
            TablesComboBox_SelectionChanged(null, null);
        }

        private void AddTeachingButton_Click(object sender, RoutedEventArgs e)
        {
            var window = new AddTeachingWindow(currentTeacher);
            window.ShowDialog();
            TablesComboBox_SelectionChanged(null, null);
        }

        private void DeleteTeachingButton_Click(object sender, RoutedEventArgs e)
        {
            var window = new DeleteTeachingWindow(currentTeacher);
            window.ShowDialog();
            TablesComboBox_SelectionChanged(null, null);
        }

        private void DeleteTeacherButton_Click(object sender, RoutedEventArgs e)
        {
            var window = new DeleteTeacherWindow(currentTeacher);
            window.ShowDialog();
        }

        private void EditTeacherButton_Click(object sender, RoutedEventArgs e)
        {
            var window = new EditTeacherWindow(currentTeacher);
            window.ShowDialog();
            TablesComboBox_SelectionChanged(null, null);
        }

        private void DeleteClassButton_Click(object sender, RoutedEventArgs e)
        {
            var window = new DeleteClassWindow(currentTeacher);
            window.ShowDialog();
        }

        private void AddSubjectButton_Click(object sender, RoutedEventArgs e)
        {
            var window = new AddSubjectWindow();
            window.ShowDialog();
            TablesComboBox_SelectionChanged(null, null);
        }

        private void DeleteSubjectButton_Click(object sender, RoutedEventArgs e)
        {
            var window = new DeleteSubjectWindow();
            window.ShowDialog();
            TablesComboBox_SelectionChanged(null, null);
        }

        private void EditSubjectButton_Click(object sender, RoutedEventArgs e)
        {
            var window = new EditSubjectWindow();
            window.ShowDialog();
            TablesComboBox_SelectionChanged(null, null);
        }

        private void CalcAverageButton_Click(object sender, RoutedEventArgs e)
        {
            var window = new CalcAverageWindow(currentTeacher);
            window.ShowDialog();
        }

        private void FindStudentButton_Click(object sender, RoutedEventArgs e)
        {
            PostgresContext db = new PostgresContext();
            string phoneNumber = NumberTextBox.Text;

            Teacher? teacher = db.Teachers.AsNoTracking().Include(t => t.Subject).AsNoTracking().Include(t => t.Class).AsNoTracking()
                .Where(t => t.PhoneNumber == phoneNumber).FirstOrDefault();
            Student? student = db.Students.AsNoTracking().Include(s => s.Class).AsNoTracking()
                .Where(s => s.PhoneNumber == phoneNumber).FirstOrDefault();

            if (teacher is not null)
                FillDataGridWith(columns["Усі вчителі"], new List<Teacher>() { teacher }, MainDataGrid);
            else if (student is not null)
                FillDataGridWith(columns["Учні"], new List<Student>() { student }, MainDataGrid);
            else
            {
                MessageBox.Show("За вказаним номером телефону не було знайдено вчителя чи учня");
                return;
            }

            TablesComboBox.SelectedIndex = -1;
            CurrentTableTextBox.Text = "Пошук";
        }

        private void FindGradeByStudentButton_Click(object sender, RoutedEventArgs e)
        {
            PostgresContext db = new PostgresContext();
            string lastName = LastNameTextBox.Text;

            List<Grade> gradesOfStudent = db.Grades.AsNoTracking().Include(g => g.Student).ThenInclude(s => s.Class).AsNoTracking()
                .Include(g => g.Teacher).ThenInclude(t => t.Subject).AsNoTracking()
                .Where(g => g.Student.LastName == lastName).ToList();

            if (gradesOfStudent.Count == 0)
            {
                MessageBox.Show("За вказаним прізвищем не було знайдено жодної оцінки");
                return;
            }

            FillDataGridWith(columns["Оцінки"], gradesOfStudent, MainDataGrid);
            TablesComboBox.SelectedIndex = -1;
            CurrentTableTextBox.Text = "Пошук";
        }

        private void StatsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PostgresContext db = new PostgresContext();
            string? selectedStat = ((ComboBoxItem)StatsComboBox.SelectedItem).Content.ToString();

            if (selectedStat is null)
                return;

            switch (selectedStat)
            {
                case "Кількість пропусків за учнями":
                    //var dateTimeNow = DateTime.Now;
                    //var startYear = dateTimeNow.Month >= 9 && dateTimeNow.Month <= 12 ? dateTimeNow.Year : dateTimeNow.Year - 1;
                    //var yearStartDate = new DateTime(startYear, 9, 1);
                    //var yearEndDate = new DateTime(startYear + 1, 5, 31);
                    var skipsPerStudent = db.Students.AsNoTracking().Include(s => s.Skips).AsNoTracking().Include(s => s.Class).ToList();


                    if (skipsPerStudent.Count == 0)
                    {
                        MessageBox.Show("У базі немає оцінок");
                        return;
                    }

                    FillDataGridWith<Student>(columns["Кількість пропусків студента"], skipsPerStudent, MainDataGrid);
                    CurrentTableTextBox.Text = "Статистика";
                    break;
                case "Кращі 5 учнів за оцінками":
                    var students = db.Students.AsNoTracking().Include(s => s.Grades).AsNoTracking().Include(s => s.Class).AsNoTracking().ToList();

                    if (students.Count == 0)
                    {
                        MessageBox.Show("У базі немає учнів");
                        return;
                    }

                    students.ForEach(s => s.SumOfGrades = s.Grades.Sum(g => g.Value));
                    students = students.OrderByDescending(s => s.SumOfGrades).Take(5).ToList();

                    FillDataGridWith<Student>(columns["Кращі учні за оцінками"], students, MainDataGrid);
                    CurrentTableTextBox.Text = "Статистика";
                    break;
                case "5 найпоширеніших предметів серед вчителів":
                    var subjects = db.Subjects.AsNoTracking().Include(s => s.Teachers).AsNoTracking()
                        .OrderByDescending(s => s.Teachers.Count).Take(5).ToList();

                    if (subjects.Count == 0)
                    {
                        MessageBox.Show("У базі немає предметів (що наврядчи можливо :/)");
                        return;
                    }

                    FillDataGridWith<Subject>(columns["Найпопулярніщі предмети серед вчителів"], subjects, MainDataGrid);

                    break;
            }
            
        }
    }
}
