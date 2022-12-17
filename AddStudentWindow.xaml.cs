using ClassTeacher_Assist.Models;
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


            try
            {
                db.Students.Add(newStudent);
                db.Classes.Update(updatedClass);
                db.SaveChanges();
                MessageBox.Show("Учня було успішно додано");
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Виникла помилка при додаванні учня \n Текст помилки: {ex.Message}");
            }
        }
    }
}
