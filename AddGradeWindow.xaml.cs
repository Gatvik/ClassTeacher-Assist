using ClassTeacher_Assist.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ClassTeacher_Assist
{
    /// <summary>
    /// Interaction logic for AddGradeWindow.xaml
    /// </summary>
    public partial class AddGradeWindow : Window
    {
        PostgresContext db = new PostgresContext();
        Teacher currentTeacher;
        private BindingList<Student> students;

        public BindingList<Student> Students
        {
            get
            {
                return new BindingList<Student>(db.Students.AsNoTracking().ToList());
            }
            set { students = value; }
        }
        public AddGradeWindow(Teacher currentTeacher)
        {
            this.currentTeacher = currentTeacher;
            InitializeComponent();
        }

        private void AddGradeButton_Click(object sender, RoutedEventArgs e)
        {
            Student? student = StudentComboBox.SelectedItem as Student;
            string? grade = (GradeComboBox.SelectedItem as Label)?.Content.ToString();
            DateTime? dateTime = DatePicker.SelectedDate;

            if (grade is null || student is null || dateTime == null)
            {
                MessageBox.Show("Учень, оцінка та дата не можуть бути пустими");
                return;
            }


            Grade newGrade = new Grade()
            {
                StudentId = student.StudentId,
                TeacherId = currentTeacher.TeacherId,
                Value = int.Parse(grade),
                ReceivingDate = dateTime.Value
            };

            db.Grades.Add(newGrade);
            db.SaveChanges();
            MessageBox.Show("Оцінка була успішно занесена");
            Close();
        }
    }
}
