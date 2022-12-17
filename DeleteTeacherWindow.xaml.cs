using ClassTeacher_Assist.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
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
    /// Interaction logic for DeleteTeacherWindow.xaml
    /// </summary>
    public partial class DeleteTeacherWindow : Window
    {
        PostgresContext db = new PostgresContext();
        Teacher currentTeacher;
        public DeleteTeacherWindow(Teacher currentTeacher)
        {
            InitializeComponent();
            this.currentTeacher = currentTeacher;
        }

        private void DeleteTeacherButton_Click(object sender, RoutedEventArgs e)
        {
            var teacher = db.Teachers.AsNoTracking().Where(t => t.TeacherId == currentTeacher.TeacherId).FirstOrDefault();
            db.Teachers.Remove(teacher);
            db.SaveChanges();
            MessageBox.Show("Ваш обліковий запис було видалено");
            Application.Current.Shutdown();
        }
    }
}
