using ClassTeacher_Assist.Models;
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
    /// Interaction logic for DeleteStudentWindow.xaml
    /// </summary>
    public partial class DeleteStudentWindow : Window
    {
        PostgresContext db = new PostgresContext();
        private BindingList<Student> students;

        public BindingList<Student> Students
        {
            get
            {
                return new BindingList<Student>(db.Students.Where(student => student.ClassId == currentUser.Class.ClassId).ToList());
            }
            set { students = value; }
        }
        Teacher currentUser;
        public DeleteStudentWindow(Teacher currentUser)
        {
            this.currentUser = currentUser;
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (Students.Count == 0)
            {
                MessageBox.Show("У вашому класі немає учнів");
                Close();
            }
        }

        private void DeleteStudentButton_Click(object sender, RoutedEventArgs e)
        {
            Student? student = StudentComboBox.SelectedItem as Student;

            if (student is null)
            {
                MessageBox.Show("Ви не обрали учня");
                return;
            }

            var updatedClass = db.Classes.Where(class_ => class_.ClassCode == currentUser.Class.ClassCode).First();
            updatedClass.StudentsNumber = updatedClass.StudentsNumber - 1;

            db.Students.Remove(student);
            db.Classes.Update(updatedClass);
            db.SaveChanges();
            MessageBox.Show("Учень був видалений");
            Close();
        }
    }
}
