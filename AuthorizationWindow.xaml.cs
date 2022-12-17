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
    /// Interaction logic for AuthorizationWindow.xaml
    /// </summary>
    public partial class AuthorizationWindow : Window
    {
        static Teacher? currentUser;

        private BindingList<Teacher> teachers;

        public BindingList<Teacher> Teachers
        {
            get 
            {
                var db = new PostgresContext();
                return new BindingList<Teacher>(db.Teachers.AsNoTracking().Include(t => t.Class)
                    .Include(t => t.Subject).ToList()); 
            }
            set { teachers = value; }
        }
        
        public AuthorizationWindow()
        {
            InitializeComponent();
        }

        private void AddTeacherButton_Click(object sender, RoutedEventArgs e)
        {
            var window = new AddTeacherWindow();
            window.ShowDialog();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            currentUser = TeacherComboBox.SelectedItem as Teacher;

            if(currentUser is null)
            {
                MessageBox.Show("Для входу оберіть себе зі списку вчителів або додайте нового вчителя");
                return;
            }

            if(currentUser.Class is null)
            {
                MessageBox.Show("За вами не закріплений клас. Якщо вважаєте, що це помилка, натисніть кнопку \"Додати клас\" нижче.");
                return;
            }

            DialogResult = true;
            Common.CurrentUser = currentUser;
            Close();
        }

        private void AddSubjectButton_Click(object sender, RoutedEventArgs e)
        {
            var window = new AddSubjectWindow();
            window.ShowDialog();
        }

        private void AddClassButton_Click(object sender, RoutedEventArgs e)
        {
            var window = new AddClassWindow();
            window.ShowDialog();
        }
    }
}
