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
    /// Interaction logic for EditSubjectWindow.xaml
    /// </summary>
    public partial class EditSubjectWindow : Window
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
        public EditSubjectWindow()
        {
            InitializeComponent();
        }

        private void SubjectsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Subject? subject = SubjectsComboBox.SelectedItem as Subject;

            if (subject is null) return;

            NameTextBox.Text = subject.Name;
            CodeTextBox.Text = subject.Code;
        }

        private void EditSubjectButton_Click(object sender, RoutedEventArgs e)
        {
            Subject? subject = SubjectsComboBox.SelectedItem as Subject;

            if (subject is null)
            {
                MessageBox.Show("Ви не обрали предмет");
                return;
            }

            string name = NameTextBox.Text;
            string code = CodeTextBox.Text;

            if(string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(code)) 
            {
                MessageBox.Show("Усі поля мають бути заповнені");
                return;
            }

            subject.Name = name;
            subject.Code = code;

            db.Subjects.Update(subject);
            db.SaveChanges();

            MessageBox.Show("Предмет було змінено");
            Close();
        }
    }
}
