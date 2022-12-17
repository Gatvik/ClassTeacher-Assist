using ClassTeacher_Assist.Models;
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
    /// Interaction logic for AddSubjectWindow.xaml
    /// </summary>
    public partial class AddSubjectWindow : Window
    {
        static PostgresContext db = new PostgresContext();
        public AddSubjectWindow()
        {
            InitializeComponent();
        }

        private void AddSubjectButton_Click(object sender, RoutedEventArgs e)
        {
            string name = NameTextBox.Text;
            string code = CodeTextBox.Text;

            if(string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(code))
            {
                MessageBox.Show("Назва та код не можуть бути пустими");
                return;
            }

            Subject newSubject = new Subject() { Name = name, Code = code};
            db.Subjects.Add(newSubject);
            db.SaveChanges();
            MessageBox.Show("Предмет було успішно додано");
            Close();
        }
    }
}
