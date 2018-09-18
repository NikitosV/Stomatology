using System;
using System.Collections.Generic;
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
using System.Data.SqlClient;
using System.Data;


namespace Stomatology.Wind
{
    /// <summary>
    /// Логика взаимодействия для PersonPage.xaml
    /// </summary>
    public partial class PersonPage : Page
    {
        TextBox a, b;

        public SqlConnection sqlCon = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename='|DataDirectory|\DataBase\stm.mdf';Integrated Security=True;Connect Timeout=30");//подключение бд

        public PersonPage()
        {
            InitializeComponent();
            Display_Data();
            a = (TextBox)FindName("txt_SearchLogin");
            b = (TextBox)FindName("txt_SearchSurname");
        }

        public void Display_Data() //показ DataBase [USER_INFO]
        {
            try
            {
                if (sqlCon.State == ConnectionState.Closed)
                {
                    sqlCon.Open();
                    SqlCommand cmd = sqlCon.CreateCommand();
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "SELECT [UserId] as 'Логин', [Surname] as 'Фамилия', [Name] as 'Имя', [Patronymic] as 'Отчество', [DateBirth] as 'Дата рождения', [Email] as 'Почта', [Mobile] as 'Телефон' FROM [USER_INFO]";
                    cmd.ExecuteNonQuery();
                    DataTable dta = new DataTable();
                    SqlDataAdapter dtaAdp = new SqlDataAdapter(cmd);
                    dtaAdp.Fill(dta);
                    dataGridViewUsers1.ItemsSource = dta.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                sqlCon.Close();
            }


        }

        private void Refuse_Click(object sender, RoutedEventArgs e)//обновить
        {
            Display_Data();
        }

        private void SearchLog_Click(object sender, RoutedEventArgs e)//поиск по логину
        {
            try
            {
                if (txt_SearchLogin.Text != "")
                {
                    if (sqlCon.State == ConnectionState.Closed)
                    {
                        sqlCon.Open();
                        SqlCommand cmd = sqlCon.CreateCommand();
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = "SELECT [UserId] as 'Логин', [Surname] as 'Фамилия', [Name] as 'Имя', [Patronymic] as 'Отчество', [DateBirth] as 'Дата рождения', [Email] as 'Почта', [Mobile] as 'Телефон' FROM [USER_INFO] WHERE [UserId] LIKE @logi";
                        cmd.Parameters.AddWithValue("@logi", a.Text);
                        cmd.ExecuteNonQuery();
                        DataTable dt = new DataTable();
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        da.Fill(dt);
                        dataGridViewUsers1.ItemsSource = dt.DefaultView;
                        sqlCon.Close();
                        txt_SearchLogin.Text = "";
                    }
                }
                else
                {
                    MessageBox.Show("Для поиска нужно ввести слово!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                sqlCon.Close();
            }
        }

        private void SearchSurname_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (txt_SearchSurname.Text != "")
                {
                    if (sqlCon.State == ConnectionState.Closed)
                    {
                        sqlCon.Open();
                        SqlCommand cmd = sqlCon.CreateCommand();
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = "SELECT [UserId] as 'Логин', [Surname] as 'Фамилия', [Name] as 'Имя', [Patronymic] as 'Отчество', [DateBirth] as 'Дата рождения', [Email] as 'Почта', [Mobile] as 'Телефон' FROM [USER_INFO] WHERE [Surname] LIKE @surn";
                        cmd.Parameters.AddWithValue("@surn", b.Text);
                        cmd.ExecuteNonQuery();
                        DataTable dt = new DataTable();
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        da.Fill(dt);
                        dataGridViewUsers1.ItemsSource = dt.DefaultView;
                        sqlCon.Close();
                        txt_SearchSurname.Text = "";
                    }
                }
                else
                {
                    MessageBox.Show("Для поиска нужно ввести слово!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                sqlCon.Close();
            }
        }
    }
}
