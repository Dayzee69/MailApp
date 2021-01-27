using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;

namespace MailApp
{

    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
            
        }

        string mail = "";

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                Hide();
                AuthWindow authWindow = new AuthWindow();
                authWindow.ShowDialog();

                if (authWindow.State == AuthWindow.ResultClose.Close)
                {
                    Close();
                }
                else 
                {
                    mail = authWindow.Mail;
                    GetData();
                    Show();
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void GetData() 
        {
            DBConnect conn = new DBConnect();
            SqlConnection sqlConnection = new SqlConnection(@"Data Source=" + conn.host + ";Initial Catalog=" + conn.db + 
                ";" + "User ID=" + conn.user + ";Password=" + conn.password);

            try
            {
                sqlConnection.Open();
                SqlCommand sqlCommand = sqlConnection.CreateCommand();
                sqlCommand.CommandText = "SELECT convert(varchar, Date_send, 104) Date_send, ID, Subject, Send_from, Send_to, Comment, Importance," +
                    " isRead FROM Mails WHERE Send_to LIKE '" + mail + "';";
                SqlDataAdapter dataAdapter = new SqlDataAdapter(sqlCommand);
                DataTable dataTable = new DataTable();
                dataAdapter.Fill(dataTable);
                dataGrid.ItemsSource = dataTable.DefaultView;
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                sqlConnection.Close();
                sqlConnection.Dispose();
            }
            dataGrid.SelectedIndex = 0;
        }

        private void MenuItemAdd_Click(object sender, RoutedEventArgs e)
        {
            try 
            {
                AddWindow addWindow = new AddWindow();
                addWindow.ShowDialog();
                GetData();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void MenuItemUpdate_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                /*ItemArray index
                 * 0 - Дата
                 * 1 - ID
                 * 2 - Тема
                 * 3 - От кого
                 * 4 - Кому
                 * 5 - Коммент
                 6 - Важность
                 7 - Прочитано
                 */
                if (dataGrid.SelectedIndex == -1)
                {
                    throw new Exception("Выбранный индекс строки равен -1");
                }
                DataRowView dataRow = (DataRowView)dataGrid.SelectedItem;
                string cellValue = dataRow.Row.ItemArray[0].ToString();
                MailSubject ms = new MailSubject(dataRow.Row.ItemArray[3].ToString(), dataRow.Row.ItemArray[2].ToString(), 
                    dataRow.Row.ItemArray[4].ToString(), dataRow.Row.ItemArray[0].ToString(), dataRow.Row.ItemArray[5].ToString(),
                    dataRow.Row.ItemArray[6].ToString(), dataRow.Row.ItemArray[7].ToString());
                ms.id = dataRow.Row.ItemArray[1].ToString();
                UpdateWindow updateWindow = new UpdateWindow(ms);
                updateWindow.ShowDialog();
                GetData();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void MenuItemExit_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
            Application.Current.Shutdown();
        }
    }
}
