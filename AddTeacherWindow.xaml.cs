using ClassTeacher_Assist.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
    /// Interaction logic for AddTeacherWindow.xaml
    /// </summary>
    public partial class AddTeacherWindow : Window
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
        public AddTeacherWindow()
        {
            InitializeComponent();
        }


        private void AddTeacherButton_Click(object sender, RoutedEventArgs e)
        {
            Subject? subject = ClassComboBox.SelectedItem as Subject;
            string firstName = NameTextBox.Text;
            string lastName = SurnameTextBox.Text;
            string patronymic = PatronymicTextBox.Text;
            string? gender = (GenderComboBox.SelectedItem as Label)?.Content.ToString();
            string phoneNumber = PhoneNumberTextBox.Text;
            string address = AddressTextBox.Text;
            string email = EmailTextBox.Text;

            if(subject is null || gender is null)
            {
                MessageBox.Show("Предмет або стать не можуть бути пустими");
                return;
            }

            if(
                string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName) ||
                string.IsNullOrWhiteSpace(patronymic) || string.IsNullOrWhiteSpace(phoneNumber) ||
                string.IsNullOrWhiteSpace(address) || string.IsNullOrWhiteSpace(email)
              )
            {
                MessageBox.Show("Усі поля мають бути заповнені.");
                return;
            }

            var newTeacher = new Teacher {
                SubjectId = subject.SubjectId, FirstName = firstName, LastName = lastName, Patronymic = patronymic,
                Gender = gender, PhoneNumber = phoneNumber, Address = address, Email = email
            };

            
            db.Teachers.Add(newTeacher);
            db.SaveChanges();
            MessageBox.Show("Вчителя було успішно додано");
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
            if (Subjects.Count == 0)
            {
                MessageBox.Show("Немає вільних предметів. Натисніть кнопку \"Додати предмет\" для створення нового предмету.");
                Close();
            }
        }
    }
}
