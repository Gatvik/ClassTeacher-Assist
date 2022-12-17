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
    /// Interaction logic for AddTeachingWindow.xaml
    /// </summary>
    public partial class AddTeachingWindow : Window
    {
        PostgresContext db = new PostgresContext();
        Teacher currentTeacher;

        private BindingList<Teacher> teachers;
        public BindingList<Teacher> Teachers
        {
            get
            {
                var restrictedTeacherIds = db.Teachings.AsNoTracking().Include(teaching => teaching.Teacher)
                    .Where(teaching => teaching.ClassId == currentTeacher.Class.ClassId)
                    .Select(teaching => teaching.Teacher.TeacherId).ToList();
                return new BindingList<Teacher>(db.Teachers.AsNoTracking().Where(teacher => !(restrictedTeacherIds.Contains(teacher.TeacherId))).ToList());
            }
            set { teachers = value; }
        }
        public AddTeachingWindow(Teacher currentTeacher)
        {
            this.currentTeacher = currentTeacher;
            InitializeComponent();
        }

        private void AddTeachingButton_Click(object sender, RoutedEventArgs e)
        {
            int classId = currentTeacher.Class.ClassId;
            Teacher? teacher = TeacherComboBox.SelectedItem as Teacher;

            if (teacher is null)
            {
                MessageBox.Show("Ви повинні обрати вчителя");
                return;
            }

            Teaching newTeaching = new Teaching() { ClassId = classId, TeacherId = teacher.TeacherId };
            db.Teachings.Add(newTeaching);
            db.SaveChanges();
            MessageBox.Show("Викладання було успішно створено");
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if(db.Teachers.Count() == 0)
            {
                MessageBox.Show("У базу даних ще не додано жодного вчителя");
                Close();
            }

            if(Teachers.Count == 0)
            {
                MessageBox.Show("Всі вчителі вже викладають у вашому класі");
                Close();
            }
        }
    }
}
