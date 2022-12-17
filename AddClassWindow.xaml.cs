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
    /// Interaction logic for AddClassWindow.xaml
    /// </summary>
    public partial class AddClassWindow : Window
    {
        public BindingList<Teacher> teachers;

        public BindingList<Teacher> Teachers
        {
            get
            {
                var db = new PostgresContext();
                var restrictedTeachers = db.Classes.Join(db.Teachers, class_ => class_.TeacherId,
                teacher => teacher.TeacherId,
                (class_, teacher) => teacher.TeacherId).ToArray();
                return new BindingList<Teacher>(db.Teachers.Where(teacher => !(restrictedTeachers.Contains(teacher.TeacherId))).ToList());
            }
            set { teachers = value; }
        }
        public AddClassWindow()
        {
            InitializeComponent();
        }

        private void AddClassButton_Click(object sender, RoutedEventArgs e)
        {
            var db = new PostgresContext();
            string classCode = CodeTextBox.Text;
            Teacher? teacher = TeacherComboBox.SelectedItem as Teacher;

            if (teacher is null || string.IsNullOrWhiteSpace(classCode))
            {
                MessageBox.Show("Код класу або класний керівник не можуть бути пустими");
                return;
            }

            foreach (var class_ in db.Classes)
            {
                if (class_.ClassCode == classCode)
                {
                    MessageBox.Show("Клас з таким кодом вже існує");
                    return;
                }
            }

            Class newClass = new Class() { ClassCode = classCode, TeacherId = teacher.TeacherId, StudentsNumber = 0 };
            
            db.Classes.Add(newClass);
            db.SaveChanges();
            MessageBox.Show("Клас було успішно додано");
            Close();

        }
    }
}