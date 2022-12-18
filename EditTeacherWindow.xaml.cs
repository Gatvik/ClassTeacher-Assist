using ClassTeacher_Assist.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
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
    /// Interaction logic for EditTeacherWindow.xaml
    /// </summary>
    public partial class EditTeacherWindow : Window
    {
        Teacher currentUser;
        PostgresContext db = new PostgresContext();

        private BindingList<Subject> subjects;
        public BindingList<Subject> Subjects
        {
            get
            {
                PostgresContext db = new PostgresContext();
                return new BindingList<Subject>(db.Subjects.AsNoTracking().ToList());  
            }
            set { subjects = value; }
        }
        public EditTeacherWindow(Teacher currentUser)
        {
            this.currentUser = currentUser;
            InitializeComponent();
        }

        private void EditTeacherButton_Click(object sender, RoutedEventArgs e)
        {
            Subject? subject = SubjectsComboBox.SelectedItem as Subject;

            string? gender = (GenderComboBox.SelectedItem as Label)?.Content.ToString();
            string firstName = NameTextBox.Text;
            string lastName = SurnameTextBox.Text;
            string patronymic = PatronymicTextBox.Text;
            string phoneNumber = PhoneNumberTextBox.Text;
            string address = AddressTextBox.Text;
            string email = EmailTextBox.Text;

            if (subject is null || gender is null)
            {
                MessageBox.Show("Предмет чи стать не можуть бути пустими");
                return;
            }

            if (
                string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName) ||
                string.IsNullOrWhiteSpace(patronymic) || string.IsNullOrWhiteSpace(phoneNumber) ||
                string.IsNullOrWhiteSpace(address) || string.IsNullOrWhiteSpace(email)
              )
            {
                MessageBox.Show("Усі поля мають бути заповнені.");
                return;
            }

            currentUser.Subject = subject;
            currentUser.FirstName = firstName; currentUser.LastName = lastName; currentUser.Patronymic = patronymic;
            currentUser.SubjectId = subject.SubjectId; currentUser.PhoneNumber = phoneNumber;
            currentUser.Address = address; currentUser.Email = email; currentUser.Gender = gender;

            db.Teachers.Update(currentUser);
            db.SaveChanges();
            MessageBox.Show("Дані вчителя оновлені");
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            PostgresContext db = new PostgresContext();
            Teacher teacher = currentUser;
            Subject subject = db.Subjects.Include(s => s.Teachers)
                                    .Where(s => s.Teachers.Contains(teacher)).First();
            SubjectsComboBox.SelectedItem = subject;
            NameTextBox.Text = teacher.FirstName;
            SurnameTextBox.Text = teacher.LastName;
            PatronymicTextBox.Text = teacher.Patronymic;

            for (int i = 0; i < GenderComboBox.Items.Count; i++)
            {
                Label? label = GenderComboBox.Items.GetItemAt(i) as Label;
                if (label is null) continue;
                if (label.Content.ToString() == teacher.Gender)
                {
                    GenderComboBox.SelectedItem = label;
                    break;
                }
            }

            PhoneNumberTextBox.Text = teacher.PhoneNumber;
            AddressTextBox.Text = teacher.Address;
            EmailTextBox.Text = teacher.Email;
        }
    }
}
