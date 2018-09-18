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
using System.Windows.Shapes;
using System.Data.SqlClient;
using System.Data;

namespace Stomatology.Wind
{
    /// <summary>
    /// Логика взаимодействия для AdminWindow.xaml
    /// </summary>
    public partial class AdminWindow : Window
    {
       public SqlConnection sqlCon = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename='|DataDirectory|\DataBase\stm.mdf';Integrated Security=True;Connect Timeout=30");

        TextBox namePr, durab, pricePr, descriptionPr;

        public AdminWindow()
        {
            InitializeComponent();

            namePr = (TextBox)FindName("txt_namePr");
            durab = (TextBox)FindName("txt_durability");
            pricePr = (TextBox)FindName("txt_pricePr");
            descriptionPr = (TextBox)FindName("txt_description");

            AdFrameNavigation.Content = new HomePage();

            Name.Text = Stomatology.Wind.LoginWindow.Online_person;
        }

        #region =Меню=
        private void DragAdminWin_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)//для перемещения окна
        {
            DragMove(); 
        }

        private void AdminClose_Click(object sender, RoutedEventArgs e)//закрыть окно
        {
            this.Close();
        }

        private void ChangeAcc_Click(object sender, RoutedEventArgs e)//поменять акк
        {
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();

            this.Close();
        }

        private void MinimizedWin_CLick(object sender, RoutedEventArgs e)//кнопка свернуть окно
        {
            this.WindowState = WindowState.Minimized;
        }

        #endregion

        #region=PopUp=

        private void HelpBut_Click(object sender, RoutedEventArgs e)//кнопка помощь в pop up
        {
            HelpWindow helpWindow = new HelpWindow();
            helpWindow.Show();
        }

        private void AboutBtn_Click(object sender, RoutedEventArgs e)
        {
            AboutWindow aboutWindow = new AboutWindow();
            aboutWindow.Show();
        }
        #endregion

        #region=MainButtons=
        private void Timetable_CLick(object sender, RoutedEventArgs e)//кнопка вызова расписания
        {
            AdFrameNavigation.Content = new TimetablePage();
        }

        private void Client_Click(object sender, RoutedEventArgs e)//кнопка перехода на клиенты страницу
        {
            AdFrameNavigation.Content = new PersonPage();
        }

        private void Service_Click(object sender, RoutedEventArgs e)//кнопка перехода на страницу с услугами
        {
            AdFrameNavigation.Content = new ServicePage();
        }

        private void HomePage_Click(object sender, RoutedEventArgs e)//кнопка перехода на главную страницу
        {
            AdFrameNavigation.Content = new HomePage();
        }
        #endregion

        TimeSpan Conver( string durab)
        {
            TimeSpan timedurab = new TimeSpan(0, 0, 0); 
            switch (durab) {
                case "10 мин":
                    timedurab = new TimeSpan(0, 10, 0);
                break;
                case "20 мин":
                    timedurab = new TimeSpan(0, 20, 0);
                    break;
                case "30 мин":
                    timedurab = new TimeSpan(0, 30, 0);
                    break;
                case "40 мин":
                    timedurab = new TimeSpan(0, 40, 0);
                    break;
                case "50 мин":
                    timedurab = new TimeSpan(0, 50, 0);
                    break;
                case "60 мин":
                    timedurab = new TimeSpan(1, 0, 0);
                    break;
                case "90 мин":
                    timedurab = new TimeSpan(1, 30, 0);
                    break;
                case "120 мин":
                    timedurab = new TimeSpan(2, 0, 0);
                    break;
                default:
                    timedurab = new TimeSpan(0, 30, 0);
                    break;
            }
            return timedurab;
        }

        private void RegestrPriem_Click(object sender, RoutedEventArgs e)//кнопка регестрации процедуры
        {
            try
            {
                if (sqlCon.State == ConnectionState.Closed)
                    sqlCon.Open();
                String quer = "INSERT INTO [PROCEDURE_INFO] ([Durability], [Procedure], [Price], [Description]) values (@durab, @proced, @price, @descrep)";
                SqlCommand cmd = new SqlCommand(quer, sqlCon);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@durab", Conver(durab.Text));
                cmd.Parameters.AddWithValue("@proced", namePr.Text);
                cmd.Parameters.AddWithValue("@price", pricePr.Text);
                cmd.Parameters.AddWithValue("@descrep", descriptionPr.Text);
                cmd.ExecuteNonQuery();

                MessageBox.Show("Занесено!");
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