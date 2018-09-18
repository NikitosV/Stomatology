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
    /// Логика взаимодействия для ServicePage.xaml
    /// </summary>
    public partial class ServicePage : Page
    {
        TextBox a;

        public ServicePage()
        {
            InitializeComponent();

            Display_Data();
            a = (TextBox)FindName("txt_SearchProcName");
        }

        public SqlConnection sqlCon = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename='|DataDirectory|\DataBase\stm.mdf';Integrated Security=True;Connect Timeout=30");//подключение бд

        private void Refuse_Click(object sender, RoutedEventArgs e)//обновить
        {
            Display_Data();
        }

        public void Display_Data() //показ DataBase [PROCEDURE_INFO]
        {
            try
            {
                if (sqlCon.State == ConnectionState.Closed)
                {
                    sqlCon.Open();
                    SqlCommand cmd = sqlCon.CreateCommand();
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "SELECT [Procedure] as 'Название процедуры', [Durability] as 'Продолжительность', [Price] as 'Цена', [Description] as 'Описание' FROM [PROCEDURE_INFO]";
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

        private void SearchProcedure_Click(object sender, RoutedEventArgs e)//поиск по названию процедуры
        {
            try
            {
                if (txt_SearchProcName.Text != "")
                {
                    if (sqlCon.State == ConnectionState.Closed)
                    {
                        sqlCon.Open();
                        SqlCommand cmd = sqlCon.CreateCommand();
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = "SELECT [Procedure] as 'Название процедуры', [Durability] as 'Продолжительность', [Price] as 'Цена', [Description] as 'Описание' FROM [PROCEDURE_INFO] WHERE [Procedure] LIKE @proc";
                        cmd.Parameters.AddWithValue("@proc", a.Text);
                        cmd.ExecuteNonQuery();
                        DataTable dt = new DataTable();
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        da.Fill(dt);
                        dataGridViewUsers1.ItemsSource = dt.DefaultView;

                        txt_SearchProcName.Text = "";
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

        private void SearchPrice_Click(object sender, RoutedEventArgs e)//поиск процедуры по цене
        {
            try
            {
                if (txt_SearchRole.Text != "")
                {
                    if (sqlCon.State == ConnectionState.Closed)
                    {
                        sqlCon.Open();
                        SqlCommand cmd = sqlCon.CreateCommand();
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = "SELECT [Procedure] as 'Название процедуры', [Durability] as 'Продолжительность', [Price] as 'Цена', [Description] as 'Описание' FROM [PROCEDURE_INFO] WHERE [Price] = '" + txt_SearchRole.Text + "'";
                        cmd.ExecuteNonQuery();
                        DataTable dt = new DataTable();
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        da.Fill(dt);
                        dataGridViewUsers1.ItemsSource = dt.DefaultView;

                        txt_SearchRole.Text = "";
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
