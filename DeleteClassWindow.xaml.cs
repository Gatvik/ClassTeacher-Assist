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
using static System.Windows.Forms.Application;

namespace ClassTeacher_Assist
{
    /// <summary>
    /// Interaction logic for DeleteClassWindow.xaml
    /// </summary>
    public partial class DeleteClassWindow : Window
    {
        Teacher currentUser;
        PostgresContext db = new PostgresContext();
        public DeleteClassWindow(Teacher currentUser)
        {
            this.currentUser = currentUser;
            InitializeComponent();
        }

        private void DeleteTeacherButton_Click(object sender, RoutedEventArgs e)
        {
            var class_ = currentUser.Class;
            db.Classes.Remove(class_);
            db.SaveChanges();
            MessageBox.Show("Ваш клас було видалено");
            //System.Windows.Forms.Application
            Restart();
            Application.Current.Shutdown();
        }
    }
}
