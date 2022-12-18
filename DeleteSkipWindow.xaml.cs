using ClassTeacher_Assist.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Linq;
using System.Windows;

namespace ClassTeacher_Assist
{
    /// <summary>
    /// Interaction logic for DeleteSkipWindow.xaml
    /// </summary>
    public partial class DeleteSkipWindow : Window
    {
        PostgresContext db = new PostgresContext();
        Teacher currentTeacher;
        private BindingList<Skip> skips;

        public BindingList<Skip> Skips
        {
            get
            {
                return new BindingList<Skip>(db.Skips.AsNoTracking().Include(skip => skip.Student).Include(skip => skip.Teacher)
                    .Where(skip => skip.Student.ClassId == currentTeacher.Class.ClassId).ToList());
            }
            set { skips = value; }
        }
        public DeleteSkipWindow(Teacher currentTeacher)
        {
            this.currentTeacher = currentTeacher;
            InitializeComponent();
        }

        private void DeleteSkipButton_Click(object sender, RoutedEventArgs e)
        {
            Skip? skip = SkipComboBox.SelectedItem as Skip;

            if (skip is null)
            {
                MessageBox.Show("Ви не обрали пропуск");
                return;
            }

            db.Skips.Remove(skip);
            db.SaveChanges();
            MessageBox.Show("Пропуск був видалений");
            Close();
        }
    }
}
