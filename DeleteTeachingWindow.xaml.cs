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
    /// Interaction logic for DeleteTeachingWindow.xaml
    /// </summary>
    public partial class DeleteTeachingWindow : Window
    {
        PostgresContext db = new PostgresContext();
        Teacher currentTeacher;
        private BindingList<Teacher> teachers;
        public BindingList<Teacher> Teachers
        {
            get
            {
                var permittedTeacherIds = db.Teachings.AsNoTracking().Include(teaching => teaching.Teacher)
                    .Where(teaching => teaching.ClassId == currentTeacher.Class.ClassId)
                    .Select(teaching => teaching.TeacherId).ToList();
                return new BindingList<Teacher>(db.Teachers.AsNoTracking()
                    .Where(teacher => permittedTeacherIds.Contains(teacher.TeacherId)).ToList());
            }
            set { teachers = value; }
        }

        public DeleteTeachingWindow(Teacher currentTeacher)
        {
            this.currentTeacher = currentTeacher;
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if(Teachers.Count == 0)
            {
                MessageBox.Show("У вашому класі і так не викладає жоден вчитель");
                Close();
            }
        }

        private void DeleteTeachingButton_Click(object sender, RoutedEventArgs e)
        {
            Teacher? teacher = TeacherComboBox.SelectedItem as Teacher;

            if(teacher is null)
            {
                MessageBox.Show("Ви не обрали вчителя");
                return;
            }

            int classId = currentTeacher.Class.ClassId;
            Teaching teachingForDeleting = db.Teachings.Where(teaching => teaching.ClassId == classId && teaching.TeacherId == teacher.TeacherId).First();

            db.Teachings.Remove(teachingForDeleting);
            db.SaveChanges();
            MessageBox.Show("Вчитель більше не викладає у вашому класі");
            Close();
        }
    }
}
