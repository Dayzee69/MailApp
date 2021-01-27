using System;
using System.Data.SqlClient;
using System.Windows;

namespace MailApp
{
    /// <summary>
    /// Логика взаимодействия для UpdateWindow.xaml
    /// </summary>
    public partial class UpdateWindow : Window
    {
        string id;
        public UpdateWindow(MailSubject mailsubject)
        {
            InitializeComponent();

            try 
            {
                id = mailsubject.id;
                tbComment.Text = mailsubject.comment;
                tbSend_from.Text = mailsubject.from;
                tbSend_to.Text = mailsubject.to;
                tbSubject.Text = mailsubject.subject;
                datePicker.SelectedDate = Convert.ToDateTime(mailsubject.date);
                if (mailsubject.importance == "Важное")
                {
                    cbImportance.SelectedIndex = 0;
                }
                else
                {
                    cbImportance.SelectedIndex = 1;
                }
                if (mailsubject.isRead == "True")
                {
                    cbIsRead.IsChecked = true;
                }
                else
                {
                    cbIsRead.IsChecked = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
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
                UpdateMail();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateMail() 
        {
            DBConnect conn = new DBConnect();
            SqlConnection sqlConnection = new SqlConnection(@"Data Source=" + conn.host + ";Initial Catalog=" + conn.db +
                ";" + "User ID=" + conn.user + ";Password=" + conn.password);
            try
            {

                sqlConnection.Open();
                SqlCommand sqlCommand = sqlConnection.CreateCommand();
                if (cbIsRead.IsChecked == true)
                {
                    sqlCommand.CommandText = "UPDATE Mails SET Subject = '" + tbSubject.Text + "', Send_from = '" + tbSend_from.Text +
                    "', Send_to = '" + tbSend_to.Text + "', Date_send = '" + datePicker.SelectedDate + "', Comment = '" + tbComment.Text +
                    "', Importance = '" + cbImportance.Text + "', isRead = 1 WHERE id = " + id + ";";
                }
                else 
                {
                    sqlCommand.CommandText = "UPDATE Mails SET Subject = '" + tbSubject.Text + "', Send_from = '" + tbSend_from.Text +
                    "', Send_to = '" + tbSend_to.Text + "', Date_send = '" + datePicker.SelectedDate + "', Comment = '" + tbComment.Text +
                    "', Importance = '" + cbImportance.Text + "', isRead = 0 WHERE id = " + id + ";";
                }


                if (sqlCommand.ExecuteNonQuery() > 0)
                {
                    MessageBox.Show("Запись изменена", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
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
