using System;
using System.Collections.Generic;
using System.Data.SQLite;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp1
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

        void ClearError()
        {
            RegTextBoxLogin.ToolTip = "Введите логин";
            RegTextBoxLogin.Foreground = Brushes.Black;
            RegPasswordTextBox.ToolTip = "Введите пароль";
            RegPasswordTextBox.Foreground = Brushes.Black;
            RegApprovePasswordTextBox.ToolTip = "Повторите пароль";
            RegApprovePasswordTextBox.Foreground = Brushes.Black;
            RegEmailTextBox.ToolTip = "Введите E-mail";
            RegEmailTextBox.Foreground = Brushes.Black;
        }
        private void RegButton_Click(object sender, RoutedEventArgs e)
        {
            string login = RegTextBoxLogin.Text.Trim();
            string password = RegPasswordTextBox.Password.Trim();
            string approvePassword = RegApprovePasswordTextBox.Password.Trim();
            string email = RegEmailTextBox.Text.Trim();

            bool RegFlag = true;

            if (login.Length < 5) 
            {
                RegTextBoxLogin.ToolTip = "Это поле введено некорректно";
                RegTextBoxLogin.Foreground = Brushes.Red;
                RegFlag = false;
            }
            if (password.Length < 5)
            {
                RegPasswordTextBox.ToolTip = "Это поле введено некорректно";
                RegPasswordTextBox.Foreground = Brushes.Red;
                RegFlag = false;
            }
            if (approvePassword != password)
            {
                RegApprovePasswordTextBox.ToolTip = "Это поле введено некорректно";
                RegApprovePasswordTextBox.Foreground = Brushes.Red;
                RegFlag = false;
            }
            if (email.Length < 5 || !email.Contains('@') || !email.Contains('.'))
            {
                Console.WriteLine(email.Contains('@'));
                RegEmailTextBox.ToolTip = "Это поле введено некорректно";
                RegEmailTextBox.Foreground = Brushes.Red;
                RegFlag = false;
            }
            if (RegFlag)
            {
                ClearError();
                string connectionString = Settings.SQLiteConnecting;

                string query = "INSERT INTO Users (Login, Password, Email) VALUES (@Login, @Password, @email)";
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    try
                    {
                        connection.Open();
                        using (SQLiteCommand command = new SQLiteCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@Login", login);
                            command.Parameters.AddWithValue("@Password", password);
                            command.Parameters.AddWithValue("@email", email);

                            command.ExecuteNonQuery();
                        }
                        connection.Close();
                        MessageBox.Show("Регистрация выполнена успешно");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка регистрации: " + ex.Message);
                    }
                }
            }
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string login = LogTextBoxLogin.Text.Trim();
            string password = LogPasswordTextBox.Password.Trim();

            string connectionString = Settings.SQLiteConnecting;

                string query = "SELECT COUNT(*) FROM Users WHERE Login = @Login AND Password = @Password";

                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    try
                    {
                        connection.Open();
                        using (SQLiteCommand command = new SQLiteCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@Login", login);
                            command.Parameters.AddWithValue("@Password", password);

                            int count = Convert.ToInt32(command.ExecuteScalar());

                            if (count > 0)
                            {
                                MessageBox.Show("Вход выполнен успешно!");
                                this.Hide();
                                Main main = new Main();
                                main.Show();
                            }
                            else
                            {
                                MessageBox.Show("Неверные логин или пароль!");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка входа: " + ex.Message);
                    }
                }
        }

        private void toLoginButton_Click(object sender, RoutedEventArgs e)
        {
            TabControlRegister.SelectedItem = TabLogin;
        }

        private void toRegButton_Click(object sender, RoutedEventArgs e)
        {
            TabControlRegister.SelectedItem = TabRagister;
        }
    }
}
