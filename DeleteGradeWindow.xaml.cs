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
    /// Interaction logic for DeleteGradeWindow.xaml
    /// </summary>
    public partial class DeleteGradeWindow : Window
    {
        PostgresContext db = new PostgresContext();
        Teacher currentTeacher;
        private BindingList<Grade> grades;

        public BindingList<Grade> Grades
        {
            get
            {
                return new BindingList<Grade>(db.Grades.AsNoTracking().Include(grade => grade.Student).Include(grade => grade.Teacher).ThenInclude(teacher => teacher.Subject)
                    .Where(grade => grade.Student.ClassId == currentTeacher.Class.ClassId).ToList());
            }
            set { grades = value; }
        }
        public DeleteGradeWindow(Teacher currentTeacher)
        {
            this.currentTeacher = currentTeacher;
            InitializeComponent();
        }

        private void DeleteGradeButton_Click_1(object sender, RoutedEventArgs e)
        {
            Grade? grade = GradeComboBox.SelectedItem as Grade;

            if (grade is null)
            {
                MessageBox.Show("Ви не обрали учня");
                return;
            }

            db.Grades.Remove(grade);
            db.SaveChanges();
            MessageBox.Show("Оцінка була видалена");
            Close();
        }
    }
}
