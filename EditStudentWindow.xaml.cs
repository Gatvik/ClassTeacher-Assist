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
    /// Interaction logic for EditStudentWindow.xaml
    /// </summary>
    public partial class EditStudentWindow : Window
    {
        PostgresContext db = new PostgresContext();
        Teacher currentUser;

        private BindingList<Student> students;
        public BindingList<Student> Students
        {
            get
            {
                return new BindingList<Student>(db.Students.Where(student => student.ClassId == currentUser.Class.ClassId).ToList());
            }
            set { students = value; }
        }

        private BindingList<Class> classes;
        public BindingList<Class> Classes
        {
            get
            {
                return new BindingList<Class>(db.Classes.ToList());
            }
            set { classes = value; }
        }
        public EditStudentWindow(Teacher currentUser)
        {
            this.currentUser = currentUser;
            InitializeComponent();
        }

        private void StudentComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Student? student = StudentComboBox.SelectedItem as Student;

            if (student is null)
            {
                MessageBox.Show("Ви не обрали учня");
                return;
            }

            ClassesComboBox.SelectedItem = db.Classes.Where(class_ => class_.ClassId == student.ClassId).First();
            NameTextBox.Text = student.FirstName;
            SurnameTextBox.Text = student.LastName;
            PatronymicTextBox.Text = student.Patronymic;
            
            for(int i = 0; i < GenderComboBox.Items.Count; i++)
            {
                Label? label = GenderComboBox.Items.GetItemAt(i) as Label;
                if (label is null) continue;
                if (label.Content.ToString() == student.Gender)
                {
                    GenderComboBox.SelectedItem = label;
                    break;
                }
            }

            PhoneNumberTextBox.Text = student.PhoneNumber;
            AddressTextBox.Text = student.Address;
            EmailTextBox.Text = student.Email;

        }

        private void EditStudentButton_Click(object sender, RoutedEventArgs e)
        {
            Student? student = StudentComboBox.SelectedItem as Student; // Old student
            Class? class_ = ClassesComboBox.SelectedItem as Class;

            string? gender = (GenderComboBox.SelectedItem as Label)?.Content.ToString();
            string firstName = NameTextBox.Text;
            string lastName = SurnameTextBox.Text;
            string patronymic = PatronymicTextBox.Text;
            string phoneNumber = PhoneNumberTextBox.Text;
            string address = AddressTextBox.Text;
            string email = EmailTextBox.Text;

            if (student is null || class_ is null || gender is null)
            {
                MessageBox.Show("Студент, якого ви змінюєте та його клас чи стать не можуть бути пустими");
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

            student.FirstName = firstName; student.LastName = lastName; student.Patronymic = patronymic;
            student.ClassId = class_.ClassId; student.PhoneNumber = phoneNumber;
            student.Address = address; student.Email = email; student.Gender = gender;

            string oldClassCode = currentUser.Class.ClassCode;
            string newClassCode = class_.ClassCode;

            //Якщо був змінений клас, то у старого класу з кількості учнів віднімається 1, у нового додається
            if (newClassCode != oldClassCode)
            {
                var oldClass = db.Classes.Where(class_ => class_.ClassCode == oldClassCode).First();
                oldClass.StudentsNumber -= 1;

                var newClass = db.Classes.Where(class_ => class_.ClassCode == newClassCode).First();
                newClass.StudentsNumber += 1;

                
                db.Classes.Update(newClass);
                db.Classes.Update(oldClass);
            }
            

            db.Students.Update(student);
            db.SaveChanges();
            MessageBox.Show("Дані учня оновлені");
            Close();
        }
    }
}
