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
    /// Логика взаимодействия для LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {


        public static string Online_person;
        public LoginWindow()
        {
            InitializeComponent();
        }

        #region =Статусбар и кнопка перейти на регестрацию=
        private void BorderDrag_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)//для передвижения окна за статусбар
        {
            DragMove();
        }

        private void CloseLog_Click(object sender, RoutedEventArgs e)//закрыть
        {
            Application.Current.Shutdown();
        }

        private void Register_Click(object sender, RoutedEventArgs e)//регестрация окно показать
        {
            RegisterWindow registerWindow = new RegisterWindow();
            registerWindow.Show();

            Application.Current.MainWindow.Close();

        }
        #endregion

        private void LogIn_Click(object sender, RoutedEventArgs e)//кнопка логин для перехода на другие окна в заисимости от пользователя
        {
            SqlConnection sqlCon = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename='|DataDirectory|\DataBase\stm.mdf';Integrated Security=True;Connect Timeout=30"); 
      
            try
            {
                if (sqlCon.State == ConnectionState.Closed)
                    sqlCon.Open();
                if (txt_user.Text != "" && txt_password.Password != "")
                {
                    String query = "SELECT  [USER].role FROM [USER] WHERE ID=@ID AND password=@password ";
                    SqlCommand sqlCmd = new SqlCommand(query, sqlCon);
                    sqlCmd.CommandType = CommandType.Text;
                    sqlCmd.Parameters.AddWithValue("@ID", txt_user.Text);
                    sqlCmd.Parameters.AddWithValue("@password", txt_password.Password);
                    string role = Convert.ToString(sqlCmd.ExecuteScalar());

                    if (role == "moder")//если модер то окно ModerWindow
                    {

                        Online_person = txt_user.Text;
                        ModerWindow mo = new ModerWindow();
                        mo.Show();
                        this.Close();
                    }
                    else if (role == "admin")//если администратор то окно AdminWindow
                    {

                        Online_person = txt_user.Text;
                        AdminWindow ad = new AdminWindow();
                        ad.Show();

                        this.Close();
                    }
                    else if (role == "doctor")//если доктор то окно DoctorWindow
                    {
                        Online_person = txt_user.Text;
                        DoctorWindow doctorWindow = new DoctorWindow();
                        doctorWindow.Show();

                        this.Close();
                    }
                    else if (role == "user")//если пользователь то окно UserWindow
                    {

                        Online_person = txt_user.Text;
                        UserWindow us = new UserWindow();
                        us.Show();

                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Такой логин/пароль не существует!");
                    }
                }
                else
                {
                    MessageBox.Show("Поля не могут быть пустые!");
                }
            }
            catch(Exception ex)
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
