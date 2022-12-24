using ClassTeacher_Assist.Models;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using PdfSharpCore;
using PdfSharpCore.Pdf;
using PdfSharpCore.Drawing;
using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.Collections.Generic;

namespace ClassTeacher_Assist
{
    /// <summary>
    /// Interaction logic for GetScorecardWindow.xaml
    /// </summary>
    public partial class GetScorecardWindow : Window
    {
        PostgresContext db = new PostgresContext();
        private BindingList<Student> students;

        public BindingList<Student> Students
        {
            get
            {
                return new BindingList<Student>(db.Students.AsNoTracking().Include(s => s.Class).AsNoTracking()
                    .Where(student => student.ClassId == currentUser.Class.ClassId).ToList());
            }
            set { students = value; }
        }
        Teacher currentUser;
        public GetScorecardWindow(Teacher currentUser)
        {
            this.currentUser = currentUser;
            InitializeComponent();
        }

        private void GetScorecardButton_Click(object sender, RoutedEventArgs e)
        {
            Student? student = StudentComboBox.SelectedItem as Student;

            if (student is null)
            {
                MessageBox.Show("Ви не обрали учня");
                return;
            }

            PdfDocument document = new PdfDocument();
            PdfPage page = document.AddPage();
            XGraphics gfx = XGraphics.FromPdfPage(page);
            XFont font = new XFont("Times New Roman", 14, XFontStyle.Regular);
            
            int yStep = 30; int currentStep = 90;
            int xOffset = 15;

            var dateTimeNow = DateTime.Now;
            //Якщо ми знаходимося у першому семестрі (9-12 місяць), то дата початку навчального року - теперишня дата, якщо ми у другому
            //семестрі (інші місяці), то дата початку навчального року - тепер. рік мінус один.
            var startYear = dateTimeNow.Month >= 9 && dateTimeNow.Month <= 12 ? dateTimeNow.Year : dateTimeNow.Year - 1;
            var yearStartDate = new DateTime(startYear, 9, 1);
            var yearEndDate = new DateTime(startYear + 1, 5, 31);

            //Якщо рік початку навчального року дорівнює теперишньому року, то ми у першому семестрі і його початок - це
            //і є початок року, якщо ні - то ми у другому семестрі і дата початку другого семестру - це перше січня
            var halfYearStartDate = startYear == dateTimeNow.Year ? yearStartDate : new DateTime(startYear + 1, 1, 1);
            //Якщо рік початку навчального року дорівнює теперишньому року, то ми у першому семестрі і його кінець - це перше січня
            //якщо ні, то ми у другому семестрі і його початок дорівнює кінцю усього навчального року
            var halfYearEndDate = startYear == dateTimeNow.Year ? new DateTime(startYear + 1, 1, 1) : yearEndDate;

            gfx.DrawString("Табель", font, XBrushes.Black, new XRect(0, 0, page.Width, page.Height), XStringFormats.TopCenter);
            gfx.DrawString($"призначений учню {student.Class.ClassCode} класу", font, XBrushes.Black, new XRect(0, 25, page.Width, page.Height), XStringFormats.TopCenter);
            gfx.DrawString($"{student.FullName}", font, XBrushes.Black, new XRect(0, 50, page.Width, page.Height), XStringFormats.TopCenter);
            gfx.DrawString($"Оцінки за предметами", font, XBrushes.Black, new XRect(0, 75, page.Width, page.Height), XStringFormats.TopCenter);

            //Fill grades
            var studentGrades = db.Grades.AsNoTracking().Include(g => g.Student).AsNoTracking()
                .Include(g => g.Teacher).ThenInclude(t => t.Subject).AsNoTracking()
                .Where(g => (g.StudentId == student.StudentId)).ToList();

            if (SemesterRadioButton.IsChecked == true)
            {
                studentGrades = studentGrades.Where(g => g.ReceivingDate >= halfYearStartDate && g.ReceivingDate <= halfYearEndDate).ToList();
            }
            else if (YearRadioButton.IsChecked == true)
            {
                studentGrades = studentGrades.Where(g => g.ReceivingDate >= yearStartDate && g.ReceivingDate <= yearEndDate).ToList();
            }
            else throw new Exception("Something went wrong");
            
            Dictionary<string, int> subjectToGradePairs = new Dictionary<string, int>();
            Dictionary<string, int> subjectToAppearencePairs = new Dictionary<string, int>();

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
            
            for (int i = 0; i < subjectToGradePairs.Count; i++)
            {
                var subjToGradeKV = subjectToGradePairs.ElementAt(i);
                var subjToAppearKV = subjectToAppearencePairs.ElementAt(i);
                double grade = Math.Round((double)subjToGradeKV.Value / subjToAppearKV.Value);
                gfx.DrawString($"{subjToGradeKV.Key}: {grade} бал(-ів)", font, XBrushes.Black, new XRect(xOffset, currentStep, page.Width, page.Height), XStringFormats.TopLeft);
                currentStep += yStep;
            }

            var studentSkips = db.Skips.AsNoTracking().Where(sk => sk.SkipId == student.StudentId).ToList();
            var permittedSkips = studentSkips.Where(s => s.Reason == "Поважна").ToList();

            if (SemesterRadioButton.IsChecked == true)
            {
                studentSkips = studentSkips.Where(sk => sk.ReceivingDate >= halfYearStartDate && sk.ReceivingDate <= halfYearEndDate).ToList();
            }
            else if (YearRadioButton.IsChecked == true)
            {
                studentGrades = studentGrades.Where(g => g.ReceivingDate >= yearStartDate && g.ReceivingDate <= yearEndDate).ToList();
            }

            gfx.DrawString($"Пропуски", font, XBrushes.Black, new XRect(xOffset, currentStep, page.Width, page.Height), XStringFormats.TopCenter);
            currentStep += yStep;
            gfx.DrawString($"Кількість - {studentSkips.Count}", font, XBrushes.Black, new XRect(xOffset, currentStep, page.Width, page.Height), XStringFormats.TopLeft);
            currentStep += yStep;
            gfx.DrawString($"З них за поважною причиною - {permittedSkips.Count}", font, XBrushes.Black, new XRect(xOffset, currentStep, page.Width, page.Height), XStringFormats.TopLeft);
            currentStep += yStep;

            currentStep += yStep;
            gfx.DrawString($"Підпис", font, XBrushes.Black, new XRect(50, currentStep, page.Width, page.Height), XStringFormats.TopLeft);
            gfx.DrawString($"{DateTime.Now.ToString("dd.MM.yyyy")}", font, XBrushes.Black, new XRect(-50, currentStep, page.Width, page.Height), XStringFormats.TopRight);

            Guid guid = Guid.NewGuid();
            string docs = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string scoreboardPath = Path.Combine(docs, $"scoreboard-{guid}.pdf");
            
            document.Save(scoreboardPath);
            MessageBox.Show("Табель збережено до директорії \"Мої документи\"");
        }

        
    }
}
