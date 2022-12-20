using System;
using System.Collections.Generic;
using System.Data;
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
using Npgsql;

namespace ClassTeacher_Assist
{
    /// <summary>
    /// Interaction logic for QueryEditWindow.xaml
    /// </summary>
    public partial class QueryEditWindow : Window
    {
        public QueryEditWindow()
        {
            InitializeComponent();
            QueryTextBox.Text = "SELECT ";
        }

        private void ClearQueryButton_Click(object sender, RoutedEventArgs e) => QueryTextBox.Text = "SELECT ";

        private void ExitQueryButton_Click(object sender, RoutedEventArgs e) => Close();

        private void ExecQueryButton_Click(object sender, RoutedEventArgs e)
        {
            string query = QueryTextBox.Text;

            if (query == "SELECT " || string.IsNullOrWhiteSpace(query))
            {
                MessageBox.Show("Невалідний запит", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!query.StartsWith("SELECT "))
            {
                MessageBox.Show("Дозволяються лише запити типу SELECT", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                NpgsqlConnection connection = new NpgsqlConnection("Host=localhost;Database=postgres;Username=postgres;Password=evalone19200319;");


                var ds = new DataSet();
                var dt = new DataTable();
                NpgsqlDataAdapter adapter;
                try
                {
                    connection.Open();
                    adapter = new NpgsqlDataAdapter(query, connection);
                }
                catch
                {
                    MessageBox.Show("Invalid SQL command");
                    return;
                }
                finally
                {
                    connection.Close();
                }
                ds.Reset();
                adapter.Fill(ds);
                dt = ds.Tables[0];
                QueryDataGrid.DataContext = dt.DefaultView;

            }
            catch (Exception er)
            {
                MessageBox.Show($"Виникла помилка при виконанні запиту. Текст: {er.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
