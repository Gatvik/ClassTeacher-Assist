using ClassTeacher_Assist.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
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
    /// Interaction logic for AddSkipWindow.xaml
    /// </summary>
    public partial class AddSkipWindow : Window
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
        public AddSkipWindow(Teacher currentTeacher)
        {
            this.currentTeacher = currentTeacher;
            InitializeComponent();
        }

        private void AddSkipButton_Click(object sender, RoutedEventArgs e)
        {
            Student? student = StudentComboBox.SelectedItem as Student;
            string? reason = (ReasonComboBox.SelectedItem as Label)?.Content.ToString();
            string? description = string.IsNullOrWhiteSpace(DescriptionTextBox.Text) ? null : DescriptionTextBox.Text;
            DateTime? dateTime = DatePicker.SelectedDate;

            if (reason is null || student is null || dateTime == null)
            {
                MessageBox.Show("Учень, причина та дата не можуть бути пустими");
                return;
            }


            Skip newSkip = new Skip()
            {
                StudentId = student.StudentId, TeacherId = currentTeacher.TeacherId,
                Reason = reason, Description = description, ReceivingDate = dateTime.Value
            };

            db.Skips.Add(newSkip);
            db.SaveChanges();
            MessageBox.Show("Пропуск був успішно занесений");
            Close();
        }
    }
}
