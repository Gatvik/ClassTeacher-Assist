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
    /// Interaction logic for CalcAverageWindow.xaml
    /// </summary>
    public partial class CalcAverageWindow : Window
    {
        PostgresContext db = new PostgresContext();
        Teacher currentTeacher;
        private BindingList<Student> students;

        public BindingList<Student> Students
        {
            get
            {
                return new BindingList<Student>(db.Students.AsNoTracking().Where(s => s.ClassId == currentTeacher.Class.ClassId).ToList());
            }
            set { students = value; }
        }
        public CalcAverageWindow(Teacher currentTeacher)
        {
            this.currentTeacher = currentTeacher;
            InitializeComponent();
        }

        private void CalcAverageButton_Click(object sender, RoutedEventArgs e)
        {
            List<Student> studentsOfClass = db.Students.Where(s => s.ClassId == currentTeacher.Class.ClassId).ToList();
            DateTime? fromDate = FromDatePicker.SelectedDate;
            DateTime? toDate = ToDatePicker.SelectedDate;
            Dictionary<string, int> subjectToGradePairs = new Dictionary<string, int>();
            Dictionary<string, int> subjectToAppearencePairs = new Dictionary<string, int>();

            if (fromDate is null || toDate is null)
            {
                MessageBox.Show("Усі поля мають бути заповнені");
                return;
            }

            if (fromDate > toDate)
            {
                MessageBox.Show("Початкова дата не може бути більше за кінцеву дату");
                return;
            }

            ResultTextBox.Text = "";
            foreach (var student in studentsOfClass)
            {
                ResultTextBox.Text += $"Борги {student.LastName} {student.FirstName}\n";
                subjectToGradePairs = new Dictionary<string, int>();
                subjectToAppearencePairs = new Dictionary<string, int>();

                var studentGrades = db.Grades.AsNoTracking().Include(g => g.Student).AsNoTracking()
                .Include(g => g.Teacher).ThenInclude(t => t.Subject).AsNoTracking()
                .Where(g => (g.StudentId == student.StudentId) && (g.ReceivingDate >= fromDate) && (g.ReceivingDate <= toDate))
                .ToList();

                foreach (var grade in studentGrades)
                {
                    string subjectName = grade.Teacher.Subject.Name;
                    if (!subjectToGradePairs.TryGetValue(subjectName, out int gradeValue))
                    {
                        subjectToGradePairs[subjectName] = 0;
                        subjectToAppearencePairs[subjectName] = 0;
                    }
                    subjectToGradePairs[subjectName] += grade.Value;
                    subjectToAppearencePairs[subjectName] += 1;
                }
                int debts = 0;
                for (int i = 0; i < subjectToGradePairs.Count; i++)
                {
                    var subjToGradeKV = subjectToGradePairs.ElementAt(i);
                    var subjToAppearKV = subjectToAppearencePairs.ElementAt(i);
                    double grade = Math.Round((double)subjToGradeKV.Value / subjToAppearKV.Value, 1);
                    if (grade < 3)
                    {
                        debts++;
                        ResultTextBox.Text += $"{subjToGradeKV.Key}: {grade} бал(-ів)\n";
                    }
                    
                }

                if (debts == 0) ResultTextBox.Text += "Боргів немає\n";
            }

            

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if(Students.Count == 0)
            {
                MessageBox.Show("У вашому класі немає учнів");
                Close();
            }
        }
    }
}
