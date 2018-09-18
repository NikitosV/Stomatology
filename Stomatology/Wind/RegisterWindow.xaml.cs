using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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

namespace Stomatology.Wind
{
    /// <summary>
    /// Логика взаимодействия для RegisterWindow.xaml
    /// </summary>
    public partial class RegisterWindow : Window
    {
        TextBox a, c, d, e2, f, g;
        DatePicker h;
        PasswordBox b;
        public RegisterWindow()
        {
            InitializeComponent();
            a = (TextBox)FindName("txt_login");
            b = (PasswordBox)FindName("txt_pass");
            c = (TextBox)FindName("txt_name");
            d = (TextBox)FindName("txt_lastname");
            e2 = (TextBox)FindName("txt_patronymic");
            f = (TextBox)FindName("txt_mobile");
            g = (TextBox)FindName("txt_email");
            h = (DatePicker)FindName("txt_birth");
        }

        #region =Статусбар=
        private void BorderDrag_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)//для передвижения окна за статусбар
        {
            DragMove();
        }

        private void CloseLog_Click(object sender, RoutedEventArgs e)//закрыть
        {
            Application.Current.Shutdown();
        }

        private void Login_Click(object sender, RoutedEventArgs e)//переход на логин вход
        {
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }
        #endregion

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            SqlConnection sqlCon = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename='|DataDirectory|\DataBase\stm.mdf';Integrated Security=True;Connect Timeout=30");
            try
            {

                if (sqlCon.State == ConnectionState.Closed)
                    sqlCon.Open();
                if(txt_login.Text != "" && txt_pass.Password != "" && txt_name.Text != "" && txt_lastname.Text != "" && txt_patronymic.Text != "" && txt_mobile.Text != "" && txt_email.Text != "" && txt_birth.SelectedDate.HasValue)//проверка что все поля не пустые
                {
                String query = "INSERT INTO [USER] (ID, password) values (@user, @pass)";
                SqlCommand sqlCmd = new SqlCommand(query, sqlCon);
                sqlCmd.CommandType = CommandType.Text;
                sqlCmd.Parameters.AddWithValue("@user", a.Text);
                sqlCmd.Parameters.AddWithValue("@pass", b.Password);
                sqlCmd.ExecuteNonQuery();//выполнеяет запрос query

                String query2 = "INSERT INTO [USER_INFO] (Userid, Name, Surname, Patronymic, Mobile, Email, DateBirth) values (@userman, @name, @surname, @patro, @mob, @email, @dateb)";
                SqlCommand sqlCmd2 = new SqlCommand(query2, sqlCon);
                sqlCmd2.CommandType = CommandType.Text;
                sqlCmd2.Parameters.AddWithValue("@userman", a.Text);
                sqlCmd2.Parameters.AddWithValue("@name", c.Text);
                sqlCmd2.Parameters.AddWithValue("@surname", d.Text);
                sqlCmd2.Parameters.AddWithValue("@patro", e2.Text);
                sqlCmd2.Parameters.AddWithValue("@mob", f.Text);
                sqlCmd2.Parameters.AddWithValue("@email", g.Text);
                sqlCmd2.Parameters.AddWithValue("@dateb", h.SelectedDate);
                sqlCmd2.ExecuteNonQuery();//выполнеяет запрос query

                MessageBox.Show("Congrulation!");
                }
                else
                {
                    MessageBox.Show("Все поля должны быть заполнены!");
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
