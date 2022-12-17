using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using ClassTeacher_Assist.Models;
using Microsoft.EntityFrameworkCore;

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
                MessageBox.Show("Ви повинні авторизуватися для продовження!");
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

        //Костиль. Чомусь метод викликається навіть до загрузки вікна, тому лише на другій і далі
        //загрузці будемо викликати його
        bool isFirst = true;
        private void Filtering_Changed(object sender, RoutedEventArgs e)
        {
            if (!isFirst) ChangeDataGrid();
            isFirst = false;

        }

        private void ChangeDataGrid()
        {
            PostgresContext db = new PostgresContext();
            string? selection = (TablesComboBox.SelectedItem as ComboBoxItem)?.Content as string;
            if (selection is null) return;

            switch (selection)
            {
                case "Учні":
                    List<Student> studentsOfClass = new();
                    if(SelfClassRadioButton.IsChecked == true)
                    {
                        studentsOfClass = db.Students.AsNoTracking().Include(s => s.Class).Where(st => st.ClassId == currentTeacher.Class.ClassId).ToList();
                    }
                    else if (AllClassesRadioButton.IsChecked == true)
                    {
                        studentsOfClass = db.Students.AsNoTracking().Include(s => s.Class).ToList();
                    }
                    FillDataGridWith<Student>(columns[selection], studentsOfClass, MainDataGrid);
                    CurrentTableTextBox.Text = "Учні";
                    break;
                case "Викладання":
                    List<Teaching> teachersOfClass = new();
                    if (SelfClassRadioButton.IsChecked == true)
                    {
                        teachersOfClass = db.Teachings.Include(teaching => teaching.Teacher).ThenInclude(teacher => teacher.Subject).Include(teaching => teaching.Class)
                        .Where(teaching => teaching.ClassId == currentTeacher.Class.ClassId).ToList();
                    }
                    else if (AllClassesRadioButton.IsChecked == true)
                    {
                        teachersOfClass = db.Teachings.Include(teaching => teaching.Teacher).ThenInclude(teacher => teacher.Subject)
                            .Include(teaching => teaching.Class).ToList();
                    }

                    FillDataGridWith<Teaching>(columns[selection], teachersOfClass, MainDataGrid);
                    CurrentTableTextBox.Text = "Викладання";
                    break;
                case "Оцінки":
                    List<Grade> gradesOfClass = new();
                    if (SelfClassRadioButton.IsChecked == true)
                    {
                        gradesOfClass = db.Grades.Include(grade => grade.Student).ThenInclude(student => student.Class).Include(grade => grade.Teacher).ThenInclude(teacher => teacher.Subject)
                        .Where(grade => grade.Student.ClassId == currentTeacher.Class.ClassId).ToList();
                    }
                    else if (AllClassesRadioButton.IsChecked == true)
                    {
                        gradesOfClass = db.Grades.Include(grade => grade.Student).ThenInclude(student => student.Class).Include(grade => grade.Teacher).ThenInclude(teacher => teacher.Subject).ToList();
                    }
                    FillDataGridWith<Grade>(columns[selection], gradesOfClass, MainDataGrid);
                    CurrentTableTextBox.Text = "Оцінки";
                    break;
                case "Пропуски":
                    List <Skip> skipsOfClass = new();
                    if (SelfClassRadioButton.IsChecked == true)
                    {
                        skipsOfClass = db.Skips.Include(skip => skip.Student).ThenInclude(student => student.Class).Include(skip => skip.Teacher).Where(skip => skip.Student.ClassId == currentTeacher.Class.ClassId).ToList();
                    }
                    else if (AllClassesRadioButton.IsChecked == true)
                    {
                        skipsOfClass = db.Skips.Include(skip => skip.Student).ThenInclude(student => student.Class).Include(skip => skip.Teacher).ToList();
                    }
                    FillDataGridWith<Skip>(columns[selection], skipsOfClass, MainDataGrid);
                    CurrentTableTextBox.Text = "Пропуски";
                    break;
                case "Усі вчителі":
                    AllClassesRadioButton.IsChecked = true;

                    var allTeachers = db.Teachers.Include(teacher => teacher.Subject).ToList();
                    FillDataGridWith<Teacher>(columns[selection], allTeachers, MainDataGrid);;
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
                    new DataGridTextColumn() { Binding = new Binding("Teacher.Subject.Code"), Header = "Предмет" },
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
                    //new DataGridTextColumn() { Binding = new Binding("Class.ClassCode"), Header = "Класний керівник класу" },
                    new DataGridTextColumn() { Binding = new Binding("Gender"), Header = "Стать" },
                    new DataGridTextColumn() { Binding = new Binding("PhoneNumber"), Header = "Номер телефону" },
                    new DataGridTextColumn() { Binding = new Binding("Address"), Header = "Адреса"},
                    new DataGridTextColumn() { Binding = new Binding("Email"), Header = "Ел. пошта"},
                    new DataGridTextColumn() { Binding = new Binding("Subject.Code"), Header = "Предмет"},
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
    }
}
