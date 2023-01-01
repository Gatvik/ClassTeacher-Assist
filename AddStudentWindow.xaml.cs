using ClassTeacher_Assist.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ClassTeacher_Assist
{
    /// <summary>
    /// Interaction logic for AddStudentWindow.xaml
    /// </summary>

    public partial class AddStudentWindow : Window
    {
        PostgresContext db = new PostgresContext();
        private BindingList<Class> classes;
        private Teacher currentUser;

        public BindingList<Class> Classes
        {
            get
            {
                return new BindingList<Class>(db.Classes.ToList());
            }
            set { classes = value; }
        }
        public AddStudentWindow(Teacher currentUser)
        {
            this.currentUser = currentUser;
            InitializeComponent();
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (Classes.Count == 0)
            {
                MessageBox.Show("Немає вільних класів. Натисніть кнопку \"Додати клас\" для створення нового класу.");
                Close();
            }
        }

        private void AddClassButton_Click(object sender, RoutedEventArgs e)
        {
            int classId = currentUser.Class.ClassId;
            string firstName = NameTextBox.Text;
            string lastName = SurnameTextBox.Text;
            string patronymic = PatronymicTextBox.Text;
            string? gender = (GenderComboBox.SelectedItem as Label)?.Content.ToString();
            string phoneNumber = PhoneNumberTextBox.Text;
            string address = AddressTextBox.Text;
            string email = EmailTextBox.Text;

            if (gender is null)
            {
                MessageBox.Show("Клас або стать не можуть бути пустими");
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

            var newStudent = new Student
            {
                ClassId = classId,
                FirstName = firstName,
                LastName = lastName,
                Patronymic = patronymic,
                Gender = gender,
                PhoneNumber = phoneNumber,
                Address = address,
                Email = email
            };

            //Додати до кількости студентів у класі одиницю
            var updatedClass = db.Classes.Where(class_ => class_.ClassId == classId).First();
            updatedClass.StudentsNumber = updatedClass.StudentsNumber + 1;

            if (GenerateApplicationCheckBox.IsChecked == true)
            {
                CreateApplication(newStudent, updatedClass);
            }

            db.Students.Add(newStudent);
            db.Classes.Update(updatedClass);
            db.SaveChanges();
            MessageBox.Show("Учня було успішно додано");
            Close();
        }

        private void CreateApplication(Student student, Class class_)
        {
            PostgresContext db = new PostgresContext();
            var word = new WordWrite(@"C:\Projects\C#\ClassTeacher-Assist\student_enroll.docx");
            class_ = db.Classes.AsNoTracking().Include(c => c.Teacher).AsNoTracking()
                .Where(c => c.ClassId == class_.ClassId).First();
            Dictionary<string, string> keyValuePairs = new Dictionary<string, string>()
            {
                {"<TEACHER_SURNAME>", class_.Teacher.LastName },
                {"<TEACHER_FIRSTNAME>", class_.Teacher.FirstName },
                {"<TEACHER_PATRONYMIC>", class_.Teacher.Patronymic },
                {"<STUDENT_SURNAME>", student.LastName },
                {"<STUDENT_FIRSTNAME>", student.FirstName },
                {"<STUDENT_PATRONYMIC>", student.Patronymic },
                {"<CLASS_CODE>", class_.ClassCode },
                {"<DATE>", DateTime.Now.ToString("dd.MM.yyyy") },
            };
            word.Process(keyValuePairs);
        }
    }

}
