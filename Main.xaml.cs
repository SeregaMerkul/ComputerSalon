using Microsoft.Win32;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Data.SqlTypes;
using System.IO;
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
using Xceed.Wpf.Toolkit;

namespace WpfApp1
{
    /// <summary>
    /// Логика взаимодействия для Main.xaml
    /// </summary>
    public partial class Main : Window
    {
        public Main()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
            RefreshComboBox();
            Get_list_Customers();
            Get_list_ComboBoxCustomers();
            Get_List_Sales();
            Get_list_ComboBoxComputers();
            Get_list_ComputerDataGrid();
            Get_List_ComboBoxComputers();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }

        void RefreshComboBox()
        {
            List<string> items = new List<string> { "CPU", "RAM", "GPU", "HDD", "SSD", "PSU", "Motherboard" };
            TypeComponentComboBox.ItemsSource = items;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Get_list();
        }

        void Get_List_Sales()
        {
            string connectionString = Settings.SQLiteConnecting;
            DataSet ds = new DataSet();
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    SQLiteCommand command = new SQLiteCommand("SELECT * FROM Sales_RU", connection);
                    SQLiteDataAdapter adapter = new SQLiteDataAdapter(command);
                    DataTable table = new DataTable();
                    adapter.Fill(ds, "Sales_RU");
                    DataGridView3.ItemsSource = ds.Tables["Sales_RU"].DefaultView;
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show("Ошибка регистрации: " + ex.Message);
                }
            }
        }

        void Get_List_ComboBoxComputers()
        {
            string connectionString = Settings.SQLiteConnecting;


            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                try
                {
                    SQLiteDataAdapter da = new SQLiteDataAdapter("select * from Computers", connection);
                    DataSet ds = new DataSet();
                    connection.Open();
                    {
                        SQLiteCommand command = new SQLiteCommand("SELECT * FROM Computers", connection);
                        SQLiteDataAdapter adapter = new SQLiteDataAdapter(command);
                        DataTable table = new DataTable();
                        adapter.Fill(ds, "Computers");
                        ComputerNumberCombobox.ItemsSource = null;
                        ComputerNumberCombobox.ItemsSource = ds.Tables["Computers"].DefaultView;
                        ComputerNumberCombobox.DisplayMemberPath = "NameComputer";
                        ComputerNumberCombobox.SelectedValuePath = "IDComupter";
                    }
                    connection.Close();
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show("Ошибка обновления: " + ex.Message);
                }
            }
        }

        void Get_list()
        {
            string connectionString = Settings.SQLiteConnecting;
            DataSet ds = new DataSet();
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    SQLiteCommand command = new SQLiteCommand("SELECT * FROM Components", connection);
                    SQLiteDataAdapter adapter = new SQLiteDataAdapter(command);
                    DataTable table = new DataTable();
                    adapter.Fill(ds, "Components");
                    DataGridView1.ItemsSource = ds.Tables["Components"].DefaultView;

                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show("Ошибка регистрации: " + ex.Message);
                }
            }

        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            Get_list();
        }

        private void ChangeButton_Click(object sender, RoutedEventArgs e)
        {
            string connectionString = Settings.SQLiteConnecting;

            string query = "UPDATE [Components] SET ComponentType = @ComponentType, Model = @Model, Manufacturer = @Manufacturer, Cost = @Cost WHERE ComponentId=@Id";
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    DataRowView selectedRow = (DataRowView)DataGridView1.SelectedItem;
                    string selectedId = selectedRow["ComponentId"].ToString();
                    using (SQLiteCommand command = new SQLiteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Id", selectedId);
                        command.Parameters.AddWithValue("@ComponentType", TypeComponentComboBox.Text);
                        command.Parameters.AddWithValue("@Model", ModelComponentTextBox.Text);
                        command.Parameters.AddWithValue("@Manufacturer", ManufacturerTextBox.Text);
                        command.Parameters.AddWithValue("@Cost", Convert.ToInt32(CostTextBox.Text));
                        command.ExecuteNonQuery();
                    }
                    connection.Close();
                    System.Windows.MessageBox.Show("Компонент успешно изменён");
                    Get_list();
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show("Ошибка добавления: " + ex.Message);
                }
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)      
        {
            string connectionString = Settings.SQLiteConnecting;

            string query = "INSERT INTO Components (ComponentType, Model, Manufacturer, Cost) VALUES (@ComponentType, @Model, @Manufacturer, @Cost)";
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    using (SQLiteCommand command = new SQLiteCommand(query, connection))
                    {                    
                        command.Parameters.AddWithValue("@ComponentType", TypeComponentComboBox.Text);
                        command.Parameters.AddWithValue("@Model", ModelComponentTextBox.Text);
                        command.Parameters.AddWithValue("@Manufacturer", ManufacturerTextBox.Text);
                        command.Parameters.AddWithValue("@Cost", Convert.ToInt32(CostTextBox.Text));

                        command.ExecuteNonQuery();
                    }
                    connection.Close();
                    System.Windows.MessageBox.Show("Компонент успешно добавлен");
                    Get_list();
                    
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show("Ошибка добавления: " + ex.Message);
                }
            }

        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string connectionString = Settings.SQLiteConnecting;
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                  
                    if (DataGridView1.SelectedItem != null)
                    {
                      
                        DataRowView selectedRow = (DataRowView)DataGridView1.SelectedItem;
                        string selectedId = selectedRow["ComponentId"].ToString();

                      
                        string query = "DELETE FROM [Components] WHERE ComponentId = @Id";
                        using (SQLiteCommand command = new SQLiteCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@Id", selectedId);
                            command.ExecuteNonQuery();
                        }

                       
                        Get_list();
                    }
                    else
                    {
                        System.Windows.MessageBox.Show("Выберите строку для удаления.");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Произошла ошибка: " + ex.Message);
            }

        }

        void Get_list_Customers()
        {
            string connectionString = Settings.SQLiteConnecting;
            DataSet ds = new DataSet();
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    SQLiteCommand command = new SQLiteCommand("SELECT * FROM Customers", connection);
                    SQLiteDataAdapter adapter = new SQLiteDataAdapter(command);
                    DataTable table = new DataTable();
                    adapter.Fill(ds, "Customers");
                    DataGridView2.ItemsSource = ds.Tables["Customers"].DefaultView;

                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show("Ошибка обновления: " + ex.Message);
                }
            }

        }

        private void AddButtonCustomers_Click(object sender, RoutedEventArgs e)
        {
            string testPhone = PhoneCustomersPhone.Text;
            string formattedPhone = FormatPhoneNumber(testPhone);
            System.Windows.MessageBox.Show(formattedPhone);
 
            string FormatPhoneNumber(string phone)
            { 
                string digitsOnly = new string(phone.Where(char.IsDigit).ToArray());
                if (digitsOnly.Length == 11)
                { 
                    return string.Format("+{0:#(###) ###-##-##}", long.Parse(digitsOnly));
                }
                else if (digitsOnly.Length == 10)
                {
                    return string.Format("+7{0:(###) ###-##-##}", long.Parse(digitsOnly));
                }
                else 
                {
                    return "Неправильный формат номера телефона";
                }
            }

            if(!(string.IsNullOrWhiteSpace(FIOCustomersTextBox.Text) || string.IsNullOrWhiteSpace(AddressCustomersTextBox.Text) || string.IsNullOrWhiteSpace(PhoneCustomersPhone.Text)))
            {    

            string connectionString = Settings.SQLiteConnecting;
            string query = "INSERT INTO Customers (FullName, Address, Phone) VALUES (@FullName, @Address, @Phone)";
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    using (SQLiteCommand command = new SQLiteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@FullName", FIOCustomersTextBox.Text);
                        command.Parameters.AddWithValue("@Address", AddressCustomersTextBox.Text);
                        command.Parameters.AddWithValue("@Phone", formattedPhone);

                        command.ExecuteNonQuery();
                    }
                    connection.Close();
                    System.Windows.MessageBox.Show("Покупатель успешно добавлен");
                    Get_list_Customers();
                    Get_list_ComboBoxCustomers();
                    }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show("Ошибка добавления: " + ex.Message);
                }
                }
            }
        }

        private void DeleteButtonCustomers_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string connectionString = Settings.SQLiteConnecting;
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();


                    if (DataGridView2.SelectedItem != null)
                    {

                        DataRowView selectedRow = (DataRowView)DataGridView2.SelectedItem;
                        string selectedId = selectedRow["CustomerId"].ToString();


                        string query = "DELETE FROM [Customers] WHERE CustomerId = @Id";
                        using (SQLiteCommand command = new SQLiteCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@Id", selectedId);
                            command.ExecuteNonQuery();
                        }


                        Get_list_Customers();
                    }
                    else
                    {
                        System.Windows.MessageBox.Show("Выберите строку для удаления.");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Произошла ошибка: " + ex.Message);
            }
        }

        private void ChangeButtonCustomers_Click(object sender, RoutedEventArgs e)
        {

            string testPhone = PhoneCustomersPhone.Text;
            string formattedPhone = FormatPhoneNumber(testPhone);
            System.Windows.MessageBox.Show(formattedPhone);

            string FormatPhoneNumber(string phone)
            {
                string digitsOnly = new string(phone.Where(char.IsDigit).ToArray());
                if (digitsOnly.Length == 11)
                {
                    return string.Format("+{0:#(###) ###-##-##}", long.Parse(digitsOnly));
                }
                else if (digitsOnly.Length == 10)
                {
                    return string.Format("+7{0:(###) ###-##-##}", long.Parse(digitsOnly));
                }
                else
                {
                    return "Неправильный формат номера телефона";
                }
            }
            if (!(string.IsNullOrWhiteSpace(FIOCustomersTextBox.Text) || string.IsNullOrWhiteSpace(AddressCustomersTextBox.Text) || string.IsNullOrWhiteSpace(PhoneCustomersPhone.Text)))
            {
                string connectionString = Settings.SQLiteConnecting;

                string query = "UPDATE [Customers] SET FullName = @FullName, Address = @Address, Phone = @Phone WHERE CustomerID=@Id";
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    try
                    {
                        connection.Open();
                        DataRowView selectedRow = (DataRowView)DataGridView2.SelectedItem;
                        string selectedId = selectedRow["CustomerID"].ToString();
                        using (SQLiteCommand command = new SQLiteCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@Id", selectedId);
                            command.Parameters.AddWithValue("@FullName", FIOCustomersTextBox.Text);
                            command.Parameters.AddWithValue("@Address", AddressCustomersTextBox.Text);
                            command.Parameters.AddWithValue("@Phone", formattedPhone);
                            command.ExecuteNonQuery();
                        }
                        connection.Close();
                        System.Windows.MessageBox.Show("Анкета покупателя успешно изменена");
                        Get_list_Customers();
                    }
                    catch (Exception ex)
                    {
                        System.Windows.MessageBox.Show("Ошибка изменения: " + ex.Message);
                    }
                }
            }
        }

        private void RefreshButtonCustomers_Click(object sender, RoutedEventArgs e)
        {
            Get_list_Customers();
        }

        private void RefreshButtonSells_Click(object sender, RoutedEventArgs e)
        {
            Get_list_ComboBoxCustomers();
        }

        void Get_list_ComboBoxCustomers()
        {
            string connectionString = Settings.SQLiteConnecting;


            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                try
                {
                    SQLiteDataAdapter da = new SQLiteDataAdapter("select * from Customers", connection);
                    DataSet ds = new DataSet();
                    connection.Open();
                    {
                        SQLiteCommand command = new SQLiteCommand("SELECT * FROM Customers", connection);
                        SQLiteDataAdapter adapter = new SQLiteDataAdapter(command);
                        DataTable table = new DataTable();
                        adapter.Fill(ds, "Customers");
                        FIOCustomerCombobox.ItemsSource = null;
                        FIOCustomerCombobox.ItemsSource = ds.Tables["Customers"].DefaultView;
                        FIOCustomerCombobox.DisplayMemberPath = "FullName";
                        FIOCustomerCombobox.SelectedValuePath = "CustomerID";

                    }
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show("Ошибка обновления: " + ex.Message);
                }
            }
        }

        void Get_list_ComboBoxComputers()
        {
            string connectionString = Settings.SQLiteConnecting;


            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                try
                {
                    SQLiteDataAdapter da = new SQLiteDataAdapter("select * from Components WHERE ComponentType = 'Motherboard'", connection);
                    DataSet ds = new DataSet();
                    connection.Open();
                    {
                        SQLiteCommand command = new SQLiteCommand("SELECT * FROM Components WHERE ComponentType = 'Motherboard'", connection);
                        SQLiteDataAdapter adapter = new SQLiteDataAdapter(command);
                        DataTable table = new DataTable();
                        adapter.Fill(ds, "Components");
                        MotherBoardComboBox.ItemsSource = null;
                        MotherBoardComboBox.ItemsSource = ds.Tables["Components"].DefaultView;
                        MotherBoardComboBox.DisplayMemberPath = "Model";
                        MotherBoardComboBox.SelectedValuePath = "ComponentID";

                    }
                    connection.Close();

                    da = new SQLiteDataAdapter("select * from Components WHERE ComponentType = 'CPU'", connection);
                    ds = new DataSet();
                    connection.Open();
                    {
                        SQLiteCommand command = new SQLiteCommand("SELECT * FROM Components WHERE ComponentType = 'CPU'", connection);
                        SQLiteDataAdapter adapter = new SQLiteDataAdapter(command);
                        DataTable table = new DataTable();
                        adapter.Fill(ds, "Components");
                        CPUComboBox.ItemsSource = null;
                        CPUComboBox.ItemsSource = ds.Tables["Components"].DefaultView;
                        CPUComboBox.DisplayMemberPath = "Model";
                        CPUComboBox.SelectedValuePath = "ComponentID";

                    }
                    connection.Close();

                    da = new SQLiteDataAdapter("select * from Components WHERE ComponentType = 'GPU'", connection);
                    ds = new DataSet();
                    connection.Open();
                    {
                        SQLiteCommand command = new SQLiteCommand("SELECT * FROM Components WHERE ComponentType = 'GPU'", connection);
                        SQLiteDataAdapter adapter = new SQLiteDataAdapter(command);
                        DataTable table = new DataTable();
                        adapter.Fill(ds, "Components");
                        GPUComboBox.ItemsSource = null;
                        GPUComboBox.ItemsSource = ds.Tables["Components"].DefaultView;
                        GPUComboBox.DisplayMemberPath = "Model";
                        GPUComboBox.SelectedValuePath = "ComponentID";

                    }
                    connection.Close();

                    da = new SQLiteDataAdapter("select * from Components WHERE ComponentType = 'PSU'", connection);
                    ds = new DataSet();
                    connection.Open();
                    {
                        SQLiteCommand command = new SQLiteCommand("SELECT * FROM Components WHERE ComponentType = 'PSU'", connection);
                        SQLiteDataAdapter adapter = new SQLiteDataAdapter(command);
                        DataTable table = new DataTable();
                        adapter.Fill(ds, "Components");
                        PSUComboBox.ItemsSource = null;
                        PSUComboBox.ItemsSource = ds.Tables["Components"].DefaultView;
                        PSUComboBox.DisplayMemberPath = "Model";
                        PSUComboBox.SelectedValuePath = "ComponentID";

                    }
                    connection.Close();

                    da = new SQLiteDataAdapter("select * from Components WHERE ComponentType = 'RAM'", connection);
                    ds = new DataSet();
                    connection.Open();
                    {
                        SQLiteCommand command = new SQLiteCommand("SELECT * FROM Components WHERE ComponentType = 'RAM'", connection);
                        SQLiteDataAdapter adapter = new SQLiteDataAdapter(command);
                        DataTable table = new DataTable();
                        adapter.Fill(ds, "Components");
                        RAMComboBox.ItemsSource = null;
                        RAMComboBox.ItemsSource = ds.Tables["Components"].DefaultView;
                        RAMComboBox.DisplayMemberPath = "Model";
                        RAMComboBox.SelectedValuePath = "ComponentID";

                    }
                    connection.Close();


                    da = new SQLiteDataAdapter("select * from Components WHERE ComponentType = 'HDD'", connection);
                    ds = new DataSet();
                    connection.Open();
                    {
                        SQLiteCommand command = new SQLiteCommand("SELECT * FROM Components WHERE ComponentType = 'HDD'", connection);
                        SQLiteDataAdapter adapter = new SQLiteDataAdapter(command);
                        DataTable table = new DataTable();
                        adapter.Fill(ds, "Components");
                        HDDComboBox.ItemsSource = null;
                        HDDComboBox.ItemsSource = ds.Tables["Components"].DefaultView;
                        HDDComboBox.DisplayMemberPath = "Model";
                        HDDComboBox.SelectedValuePath = "ComponentID";

                    }
                    connection.Close();

                    da = new SQLiteDataAdapter("select * from Components WHERE ComponentType = 'SSD'", connection);
                    ds = new DataSet();
                    connection.Open();
                    {
                        SQLiteCommand command = new SQLiteCommand("SELECT * FROM Components WHERE ComponentType = 'SSD'", connection);
                        SQLiteDataAdapter adapter = new SQLiteDataAdapter(command);
                        DataTable table = new DataTable();
                        adapter.Fill(ds, "Components");
                        SSDComboBox.ItemsSource = null;
                        SSDComboBox.ItemsSource = ds.Tables["Components"].DefaultView;
                        SSDComboBox.DisplayMemberPath = "Model";
                        SSDComboBox.SelectedValuePath = "ComponentID";

                    }
                    connection.Close();
                    
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show("Ошибка обновления: " + ex.Message);
                }
            }
        }
        
        private void AddButtonComputers_Click(object sender, RoutedEventArgs e)
        {
            if (!(string.IsNullOrWhiteSpace(NameComputerTextBox.Text) || string.IsNullOrWhiteSpace(MotherBoardComboBox.Text) || string.IsNullOrWhiteSpace(CPUComboBox.Text) || string.IsNullOrWhiteSpace(GPUComboBox.Text) || string.IsNullOrWhiteSpace(RAMComboBox.Text) || string.IsNullOrWhiteSpace(PSUComboBox.Text) || (string.IsNullOrWhiteSpace(HDDComboBox.Text) || string.IsNullOrWhiteSpace(SSDComboBox.Text))))
            {

                string connectionString = Settings.SQLiteConnecting;
                string query = "INSERT INTO Computers (NameComputer, Motherboard, CPU, GPU, PSU, RAM, HDD, SSD, Cost) VALUES (@NameComputer, @Motherboard, @CPU, @GPU,@PSU, @RAM, @HDD, @SSD, @Cost)";
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    try
                    {
                        connection.Open();

                        int cost = 0;

                        // Получаем стоимость каждого компонента и суммируем их
                        using (SQLiteCommand costCommand = new SQLiteCommand(connection))
                        {
                            costCommand.CommandText = "SELECT Cost FROM Components WHERE ComponentID = @ID";

                            // Параметры команды
                            costCommand.Parameters.Add("@ID", DbType.Int32);

                            // Получение и суммирование стоимости каждого компонента
                            costCommand.Parameters["@ID"].Value = MotherBoardComboBox.SelectedValue;
                            cost += Convert.ToInt32(costCommand.ExecuteScalar());

                            costCommand.Parameters["@ID"].Value = CPUComboBox.SelectedValue;
                            cost += Convert.ToInt32(costCommand.ExecuteScalar());

                            costCommand.Parameters["@ID"].Value = GPUComboBox.SelectedValue;
                            cost += Convert.ToInt32(costCommand.ExecuteScalar());

                            costCommand.Parameters["@ID"].Value = RAMComboBox.SelectedValue;
                            cost += Convert.ToInt32(costCommand.ExecuteScalar());

                            costCommand.Parameters["@ID"].Value = HDDComboBox.SelectedValue;
                            cost += Convert.ToInt32(costCommand.ExecuteScalar());

                            costCommand.Parameters["@ID"].Value = SSDComboBox.SelectedValue;
                            cost += Convert.ToInt32(costCommand.ExecuteScalar());
                        }

                   
                        using (SQLiteCommand command = new SQLiteCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@NameComputer", NameComputerTextBox.Text);
                            command.Parameters.AddWithValue("@Motherboard", MotherBoardComboBox.SelectedValue);
                            command.Parameters.AddWithValue("@CPU", CPUComboBox.SelectedValue);
                            command.Parameters.AddWithValue("@GPU", GPUComboBox.SelectedValue);
                            command.Parameters.AddWithValue("@PSU", PSUComboBox.SelectedValue);
                            command.Parameters.AddWithValue("@RAM", RAMComboBox.SelectedValue);
                            command.Parameters.AddWithValue("@HDD", HDDComboBox.SelectedValue);
                            command.Parameters.AddWithValue("@SSD", SSDComboBox.SelectedValue);
                            command.Parameters.AddWithValue("@Cost", cost); // Записываем общую стоимость
                            command.ExecuteNonQuery();
                        }

                        connection.Close();
                        System.Windows.MessageBox.Show("Компьютер успешно собран");
                        Get_list_ComputerDataGrid();
                    }
                    catch (Exception ex)
                    {
                        System.Windows.MessageBox.Show("Ошибка добавления: " + ex.Message);
                    }
                }

            }

            else
            {
                System.Windows.MessageBox.Show("Заполните все поля");
            }
        }
        
        void Get_list_ComputerDataGrid()
        {
            string connectionString = Settings.SQLiteConnecting;
            DataSet ds = new DataSet();
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    SQLiteCommand command = new SQLiteCommand("SELECT * FROM ComputersView", connection);
                    SQLiteDataAdapter adapter = new SQLiteDataAdapter(command);
                    DataTable table = new DataTable();
                    adapter.Fill(ds, "ComputersView");
                    DataGridView4.ItemsSource = ds.Tables["ComputersView"].DefaultView;

                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show("Ошибка регистрации: " + ex.Message);
                }
            }

        }
        
        private void ChangeButtonComputers_Click(object sender, RoutedEventArgs e)
        {
            if (!(string.IsNullOrWhiteSpace(NameComputerTextBox.Text) || string.IsNullOrWhiteSpace(MotherBoardComboBox.Text) || string.IsNullOrWhiteSpace(CPUComboBox.Text) || string.IsNullOrWhiteSpace(GPUComboBox.Text) || string.IsNullOrWhiteSpace(RAMComboBox.Text) || string.IsNullOrWhiteSpace(PSUComboBox.Text) || (string.IsNullOrWhiteSpace(HDDComboBox.Text) || string.IsNullOrWhiteSpace(SSDComboBox.Text))))
            {

                string connectionString = Settings.SQLiteConnecting;

                string query = "UPDATE Computers SET NameComputer = @NameComputer, Motherboard = @Motherboard, CPU = @CPU, GPU = @GPU, PSU = @PSU, RAM = @RAM, HDD = @HDD, SSD = @SSD, Cost = @Cost WHERE IDComupter = @ID";
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    try
                    {
                        connection.Open();

                        int cost = 0;

                        // Получаем стоимость каждого компонента и суммируем их
                        using (SQLiteCommand costCommand = new SQLiteCommand(connection))
                        {
                            costCommand.CommandText = "SELECT Cost FROM Components WHERE ComponentID = @ID";

                            // Параметры команды
                            costCommand.Parameters.Add("@ID", DbType.Int32);

                            // Получение и суммирование стоимости каждого компонента
                            costCommand.Parameters["@ID"].Value = MotherBoardComboBox.SelectedValue;
                            cost += Convert.ToInt32(costCommand.ExecuteScalar());

                            costCommand.Parameters["@ID"].Value = CPUComboBox.SelectedValue;
                            cost += Convert.ToInt32(costCommand.ExecuteScalar());

                            costCommand.Parameters["@ID"].Value = GPUComboBox.SelectedValue;
                            cost += Convert.ToInt32(costCommand.ExecuteScalar());

                            costCommand.Parameters["@ID"].Value = RAMComboBox.SelectedValue;
                            cost += Convert.ToInt32(costCommand.ExecuteScalar());

                            costCommand.Parameters["@ID"].Value = HDDComboBox.SelectedValue;
                            cost += Convert.ToInt32(costCommand.ExecuteScalar());

                            costCommand.Parameters["@ID"].Value = SSDComboBox.SelectedValue;
                            cost += Convert.ToInt32(costCommand.ExecuteScalar());
                        }
                        DataRowView selectedRow = (DataRowView)DataGridView4.SelectedItem;
                        string selectedId = selectedRow["Номер компьютера"].ToString();
                        using (SQLiteCommand command = new SQLiteCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@ID", selectedId);
                            command.Parameters.AddWithValue("@NameComputer", NameComputerTextBox.Text);
                            command.Parameters.AddWithValue("@Motherboard", MotherBoardComboBox.SelectedValue);
                            command.Parameters.AddWithValue("@CPU", CPUComboBox.SelectedValue);
                            command.Parameters.AddWithValue("@GPU", GPUComboBox.SelectedValue);
                            command.Parameters.AddWithValue("@PSU", PSUComboBox.SelectedValue);
                            command.Parameters.AddWithValue("@RAM", RAMComboBox.SelectedValue);
                            command.Parameters.AddWithValue("@HDD", HDDComboBox.SelectedValue);
                            command.Parameters.AddWithValue("@SSD", SSDComboBox.SelectedValue);
                            command.Parameters.AddWithValue("@Cost", cost); // Записываем общую стоимость
                            command.ExecuteNonQuery();
                        }

                        connection.Close();
                        System.Windows.MessageBox.Show("Компьютер успешно изменен");
                        Get_list_ComputerDataGrid();
                    }
                    catch (Exception ex)
                    {
                        System.Windows.MessageBox.Show("Ошибка изменения: " + ex.Message);
                    }
                }
            }
        }
        
        private void DeleteButtonComputers_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                string connectionString = Settings.SQLiteConnecting;
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();


                    if (DataGridView4.SelectedItem != null)
                    {

                        DataRowView selectedRow = (DataRowView)DataGridView4.SelectedItem;
                        string selectedId = selectedRow["Номер компьютера"].ToString();


                        string query = "DELETE FROM [Computers] WHERE IDComupter = @Id";
                        using (SQLiteCommand command = new SQLiteCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@Id", selectedId);
                            command.ExecuteNonQuery();
                        }


                        Get_list_ComputerDataGrid();
                    }
                    else
                    {
                        System.Windows.MessageBox.Show("Выберите строку для удаления");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Ошибка удаления: " + ex.Message);
            }
        }

        private void RefreshButtonComputers_Click(object sender, RoutedEventArgs e)
        {
            Get_list_ComputerDataGrid();
        }


       public void TakePriceComputer()
        {
            string connectionString = Settings.SQLiteConnecting;
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    double cost = 0;


                    using (SQLiteCommand costCommand = new SQLiteCommand(connection))
                    {
                        costCommand.CommandText = "SELECT Cost FROM Computers WHERE IDComupter = @ID";

                        costCommand.Parameters.Add("@ID", DbType.Int32);


                        costCommand.Parameters["@ID"].Value = ComputerNumberCombobox.SelectedValue;
                        cost += Convert.ToInt32(costCommand.ExecuteScalar());
                        if (CardRadioButton.IsChecked == true)
                        {
                            cost += cost * 0.13;
                        }
                        else if (CashRadioButton.IsChecked == true)
                        {
                            cost += cost * 0.1;
                        }
                        else
                        {
                            System.Windows.MessageBox.Show("Выберите один из способов оплаты");
                        }

                    }

                    ItogLabel.Text = $"Итого: {cost}";
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show("Ошибка добавления: " + ex.Message);
                }
            }
        }
        private void ComputerNumberCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TakePriceComputer();            
        }

        private void CardRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            TakePriceComputer();
        }

        private void CashRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            TakePriceComputer();
        }

        private void AddButtonSells_Click(object sender, RoutedEventArgs e)
        {
            if (!(string.IsNullOrWhiteSpace(FIOCustomerCombobox.Text) || string.IsNullOrWhiteSpace(DataPickerSale.Text) || string.IsNullOrWhiteSpace(ComputerNumberCombobox.Text)))
            {
                double cost = 0;
                string choicePay = "";
                string connectionString = Settings.SQLiteConnecting;
                string query = "INSERT INTO Sales (SaleDate, CustomerID, ComputerID, ChoicePay, Cost) VALUES (@SaleDate, @CustomerID, @ComputerID, @ChoicePay, @Cost)";
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    try
                    {
                        connection.Open();
                        using (SQLiteCommand costCommand = new SQLiteCommand(connection))
                        {
                            costCommand.CommandText = "SELECT Cost FROM Computers WHERE IDComupter = @ID";

                            costCommand.Parameters.Add("@ID", DbType.Int32);


                            costCommand.Parameters["@ID"].Value = ComputerNumberCombobox.SelectedValue;
                            cost += Convert.ToInt32(costCommand.ExecuteScalar());
                            if (CardRadioButton.IsChecked == true)
                            {
                                cost += cost * 0.13;
                                choicePay = "Карта (13%)";
                            }
                            else if (CashRadioButton.IsChecked == true)
                            {
                                cost += cost * 0.1;
                                choicePay = "Наличные (10%)";
                            }
                            else
                            {
                                System.Windows.MessageBox.Show("Выберите один из способов оплаты");
                            }

                        }

                        ItogLabel.Text = $"Итого: {cost}";
                    }
                    catch (Exception ex)
                    {
                        System.Windows.MessageBox.Show("Ошибка добавления: " + ex.Message);
                    }
                    connection.Close();


                    connection.Open();
                    using (SQLiteCommand command = new SQLiteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@SaleDate", DataPickerSale.SelectedDate);
                        command.Parameters.AddWithValue("@CustomerID", FIOCustomerCombobox.SelectedValue);
                        command.Parameters.AddWithValue("@ComputerID", ComputerNumberCombobox.SelectedValue);
                        command.Parameters.AddWithValue("@ChoicePay", choicePay);
                        command.Parameters.AddWithValue("@Cost", Math.Round(cost));


                        command.ExecuteNonQuery();
                    }
                    connection.Close();
                    Get_List_Sales();
                    System.Windows.MessageBox.Show("Продажа успешно совершена");                                        
                }
            }
        }

        private void ChangeButtonSells_Click(object sender, RoutedEventArgs e)
        {
            if (!(string.IsNullOrWhiteSpace(FIOCustomerCombobox.Text) || string.IsNullOrWhiteSpace(DataPickerSale.Text) || string.IsNullOrWhiteSpace(ComputerNumberCombobox.Text)))
            {
                double cost = 0;
                string choicePay = "";
                string connectionString = Settings.SQLiteConnecting;
                string query = "UPDATE Sales SET SaleDate = @SaleDate, CustomerID = @CustomerID, ComputerID = @ComputerID, ChoicePay = @ChoicePay, Cost = @Cost WHERE SaleID = @Id";
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    try
                    {
                        connection.Open();
                        using (SQLiteCommand costCommand = new SQLiteCommand(connection))
                        {
                            costCommand.CommandText = "SELECT Cost FROM Computers WHERE IDComupter = @ID";

                            costCommand.Parameters.Add("@ID", DbType.Int32);


                            costCommand.Parameters["@ID"].Value = ComputerNumberCombobox.SelectedValue;
                            cost += Convert.ToInt32(costCommand.ExecuteScalar());
                            if (CardRadioButton.IsChecked == true)
                            {
                                cost += cost * 0.13;
                                choicePay = "Карта (13%)";
                            }
                            else if (CashRadioButton.IsChecked == true)
                            {
                                cost += cost * 0.1;
                                choicePay = "Наличные (10%)";
                            }
                            else
                            {
                                System.Windows.MessageBox.Show("Выберите один из способов оплаты");
                            }

                        }

                        ItogLabel.Text = $"Итого: {cost}";
                    }
                    catch (Exception ex)
                    {
                        System.Windows.MessageBox.Show("Ошибка добавления: " + ex.Message);
                    }
                    connection.Close();


                    connection.Open();
                    DataRowView selectedRow = (DataRowView)DataGridView3.SelectedItem;
                    string selectedId = selectedRow["Идентификатор Продажи"].ToString();
                    using (SQLiteCommand command = new SQLiteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Id", selectedId);
                        command.Parameters.AddWithValue("@SaleDate", DataPickerSale.SelectedDate);
                        command.Parameters.AddWithValue("@CustomerID", FIOCustomerCombobox.SelectedValue);
                        command.Parameters.AddWithValue("@ComputerID", ComputerNumberCombobox.SelectedValue);
                        command.Parameters.AddWithValue("@ChoicePay", choicePay);
                        command.Parameters.AddWithValue("@Cost", Math.Round(cost));


                        command.ExecuteNonQuery();
                    }
                    connection.Close();
                    Get_List_Sales();
                    System.Windows.MessageBox.Show("Продажа успешно изменена");
                }
            }
        }

        private void DeleteButtonSells_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string connectionString = Settings.SQLiteConnecting;
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();


                    if (DataGridView3.SelectedItem != null)
                    {

                        DataRowView selectedRow = (DataRowView)DataGridView3.SelectedItem;
                        string selectedId = selectedRow["Идентификатор Продажи"].ToString();


                        string query = "DELETE FROM [Sales] WHERE SaleID = @Id";
                        using (SQLiteCommand command = new SQLiteCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@Id", selectedId);
                            command.ExecuteNonQuery();
                        }


                        Get_List_Sales();
                    }
                    else
                    {
                        System.Windows.MessageBox.Show("Выберите строку для удаления");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Ошибка удаления: " + ex.Message);
            }
        }

        private void ReportButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*";
            saveFileDialog.FilterIndex = 1;
            saveFileDialog.FileName = "SalesReport.xlsx";

            bool? result = saveFileDialog.ShowDialog();

            if (result == true)
            {

                using (ExcelPackage excelPackage = new ExcelPackage())
                {
                    ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("Sales Report");

                    DataTable dataTable = GetDataFromDatabase();

                    int row = 1;
                    int col = 1;

                    foreach (DataColumn column in dataTable.Columns)
                    {
                        worksheet.Cells[row, col].Value = column.ColumnName;
                        col++;
                    }

                    
                    worksheet.Cells[row, 1, row, col - 1].Style.Font.Bold = true;

                    
                    row++;

                    
                    foreach (DataRow dataRow in dataTable.Rows)
                    {
                        col = 1;
                        foreach (var item in dataRow.ItemArray)
                        {
                            if (item is DateTime)
                            {
                                worksheet.Cells[row, col].Value = (DateTime)item;
                                worksheet.Cells[row, col].Style.Numberformat.Format = "yyyy-mm-dd";
                            }
                            else
                            {
                                worksheet.Cells[row, col].Value = item;
                            }
                            col++;
                        }
                        row++;
                    }

                    // Подсчет суммы значений в столбце "Оплата"
                    double totalPayment = dataTable.AsEnumerable()
                                                    .Sum(r => Convert.ToDouble(r["Оплата"]));

                    // Добавление итоговой строки
                    worksheet.Cells[row, 5].Value = "Итого:";
                    worksheet.Cells[row, 6].Value = totalPayment;

                    // Полужирное выделение строки с итогами
                    worksheet.Cells[row, 1, row, col - 1].Style.Font.Bold = true;

                    worksheet.Cells.AutoFitColumns();
                    
                    FileInfo excelFile = new FileInfo(saveFileDialog.FileName);
                    excelPackage.SaveAs(excelFile);

                    System.Windows.MessageBox.Show("Отчет успешно экспортирован в Excel!");
                }
            }
        }


        private DataTable GetDataFromDatabase()
        {

            string connectionString = Settings.SQLiteConnecting;
            string query = "SELECT * FROM Sales_RU WHERE [Дата Продажи] BETWEEN @SelectedDate and @SelectedDate2";

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {

                    command.Parameters.AddWithValue("@SelectedDate", StartDatePicker.SelectedDate);
                    command.Parameters.AddWithValue("@SelectedDate2", FinishDatePicker.SelectedDate);

                    using (SQLiteDataAdapter adapter = new SQLiteDataAdapter(command))
                    {
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);
                        return dataTable;
                    }
                }
            }
        
        }

    }
}
