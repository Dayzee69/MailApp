using System;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Windows;

namespace MailApp
{
    /// <summary>
    /// Логика взаимодействия для AuthWindow.xaml
    /// </summary>
    public partial class AuthWindow : Window
    {
        public AuthWindow()
        {
            InitializeComponent();
        }

        protected internal ResultClose State = ResultClose.Close;
        
        protected internal string Mail = "";

        public enum ResultClose { Authentication, Close };

        private static byte[] aeskey = new byte[32] { 1, 21, 12, 32, 54, 99, 12, 33, 60, 47, 28, 51, 96, 33, 39, 5, 86, 77, 73, 13, 4,
            14, 38, 69, 46, 74, 2, 94, 45, 39, 40, 17};

        private void buttonEnter_Click(object sender, RoutedEventArgs e)
        {
            try 
            {
                string pass = "";
                if (pbPass.Password == "" || tbMail.Text == "")
                    throw new Exception("Поля 'Почта' и 'Пароль' обязательны для заполнения");

                DBConnect conn = new DBConnect();
                SqlConnection sqlConnection = new SqlConnection(@"Data Source=" + conn.host + ";Initial Catalog=" + conn.db +
                    ";" + "User ID=" + conn.user + ";Password=" + conn.password);
                sqlConnection.Open();
                SqlCommand sqlCommand = sqlConnection.CreateCommand();
                sqlCommand.CommandText = "SELECT Password FROM MyChatUsers WHERE MailAdress = '%" + tbMail.Text + "%';";
                using (DbDataReader reader = sqlCommand.ExecuteReader())
                {
                    if(!reader.HasRows)
                        throw new Exception("Почта введена не правильно или не зарегистрирована");

                    while (reader.Read())
                    {
                        pass = (reader.GetValue(0).ToString());
                    }
                }

                if (pbPass.Password != FromAes256(pass) || pass == "")
                {
                    throw new Exception("Неверный пароль");
                }
                else
                {
                    Mail = "%" + tbMail.Text + "%";
                    State = ResultClose.Authentication;
                    Close();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                pbPass.Password = "";
            }

        }

        private static string FromAes256(string str)
        {
            try 
            {
                byte[] shifr = Convert.FromBase64String(str);
                byte[] bytesIv = new byte[16];
                byte[] mess = new byte[shifr.Length - 16];
                //Списываем соль
                for (int i = shifr.Length - 16, j = 0; i < shifr.Length; i++, j++)
                    bytesIv[j] = shifr[i];
                //Списываем оставшуюся часть сообщения
                for (int i = 0; i < shifr.Length - 16; i++)
                    mess[i] = shifr[i];
                //Объект класса Aes
                Aes aes = Aes.Create();
                //Задаем тот же ключ, что и для шифрования
                aes.Key = aeskey;
                //Задаем соль
                aes.IV = bytesIv;
                //Строковая переменная для результата
                string text = "";
                byte[] data = mess;
                ICryptoTransform crypt = aes.CreateDecryptor(aes.Key, aes.IV);
                using (MemoryStream ms = new MemoryStream(data))
                {
                    using (CryptoStream cs = new CryptoStream(ms, crypt, CryptoStreamMode.Read))
                    {
                        using (StreamReader sr = new StreamReader(cs))
                        {
                            //Результат записываем в переменную text в вие исходной строки
                            text = sr.ReadToEnd();
                        }
                    }
                }
                return text;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return "";
            }
        }

        private static string ToAes256(string src)
        {
            try
            {
                //Объявляем объект класса AES
                Aes aes = Aes.Create();
                //Генерируем соль
                aes.GenerateIV();

                //Присваиваем ключ. aeskey - переменная (массив байт), сгенерированная методом GenerateKey() класса AES
                aes.Key = aeskey;
                byte[] encrypted;
                ICryptoTransform crypt = aes.CreateEncryptor(aes.Key, aes.IV);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, crypt, CryptoStreamMode.Write))
                    {
                        using (StreamWriter sw = new StreamWriter(cs))
                        {
                            sw.Write(src);
                        }
                    }
                    //Записываем в переменную encrypted зашиврованный поток байтов
                    encrypted = ms.ToArray();
                }
                //Возвращаем поток байт + крепим соль

                return Convert.ToBase64String(encrypted.Concat(aes.IV).ToArray());

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return "";
            }

        }
    }
}
