using System;
using System.Data.SqlClient;
using System.Windows;

namespace MailApp
{
    /// <summary>
    /// Логика взаимодействия для AddWindow.xaml
    /// </summary>
    public partial class AddWindow : Window
    {
        public AddWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            datePicker.SelectedDate = DateTime.Now;
        }

        private void addButton_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                if (tbSend_from.Text.Length == 0)
                {
                    throw new Exception("Поле 'От кого' обязательно для заполнения");
                } 
                else if (tbSubject.Text.Length == 0)
                {
                    throw new Exception("Поле 'Тема' обязательно для заполнения");
                }
                else if (tbSend_to.Text.Length == 0)
                {
                    throw new Exception("Поле 'Кому' обязательно для заполнения");
                }
                MailSubject mailsub = new MailSubject(tbSend_from.Text, tbSubject.Text, tbSend_to.Text,
                datePicker.SelectedDate.ToString(), tbComment.Text, cbImportance.Text, cbIsRead.IsChecked.ToString());
                addMail(mailsub);
            }
            catch (Exception ex) 
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void addMail(MailSubject mail) 
        {
            DBConnect conn = new DBConnect();
            SqlConnection sqlConnection = new SqlConnection(@"Data Source=" + conn.host + ";Initial Catalog=" + conn.db +
                ";" + "User ID=" + conn.user + ";Password=" + conn.password);
            try
            {
                sqlConnection.Open();
                SqlCommand sqlCommand = sqlConnection.CreateCommand();
                sqlCommand.CommandText = "INSERT INTO Mails (Subject, Send_from, Date_send, Send_to, Comment, Importance, isRead) VALUES('" + mail.subject + "', '" 
                    + mail.from + "', '" + mail.date + "', '" + mail.to + "', '" + mail.comment + "', '" + mail.importance + "', '" + mail.isRead + "')";

                if (sqlCommand.ExecuteNonQuery() > 0) 
                {
                    MessageBox.Show("Запись добавлена", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    throw new Exception("Ошибка добалвения записи");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                sqlConnection.Close();
                sqlConnection.Dispose();
                Close();
            }

        }

    }
}
