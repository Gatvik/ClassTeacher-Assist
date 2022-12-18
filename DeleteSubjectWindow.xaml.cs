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
    /// Interaction logic for DeleteSubjectWindow.xaml
    /// </summary>
    public partial class DeleteSubjectWindow : Window
    {
        PostgresContext db = new PostgresContext();
        
        private BindingList<Subject> subjects;

        public BindingList<Subject> Subjects
        {
            get
            {
                return new BindingList<Subject>(db.Subjects.AsNoTracking().ToList());
            }
            set { subjects = value; }
        }
        public DeleteSubjectWindow()
        {
            
            InitializeComponent();
        }

        private void DeleteSubjectButton_Click(object sender, RoutedEventArgs e)
        {
            Subject? subject = SubjectComboBox.SelectedItem as Subject;

            if (subject is null)
            {
                MessageBox.Show("Ви не обрали предмет");
                return;
            }

            db.Subjects.Remove(subject);
            db.SaveChanges();
            MessageBox.Show("Предмет був видалений");
            Close();
        }
    }
}
