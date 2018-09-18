using Stomatology.Class;
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
    /// Логика взаимодействия для UserWindow.xaml
    /// </summary>
    public partial class UserWindow : Window
    {
        public UserWindow()
        {
            InitializeComponent();
            doctors = new List<Doctor_>();
            users = new List<User>();
            procedures = new List<Procedure>();
            StartWorking();

            Doctor.ItemsSource = doctors;

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

        private void MinimizedWin_CLick(object sender, RoutedEventArgs e)
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

        public List<Doctor_> doctors { get; private set; }
        public List<Time> timetables { get; private set; }
        public List<User> users { get; private set; }
        public List<Procedure> procedures { get; private set; }

        public SqlConnection connection = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename='|DataDirectory|\DataBase\stm.mdf';Integrated Security=True;Connect Timeout=30");

        public TimeSpan timeschedule = new TimeSpan(0, 0, 0);

   

        void StartWorking()
        {

            #region bd подключение доктора

            //подключить

            try
            {

                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                String query = "SELECT   [DOCTOR_INFO].[ID],  [DOCTOR_INFO].[Name],  [DOCTOR_INFO].[Surname] FROM [DOCTOR_INFO]";
                SqlCommand command = new SqlCommand(query, connection);
                command.CommandType = CommandType.Text;
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    int Id = reader.GetInt32(0);
                    string Name = reader.GetString(1);
                    string Surname = reader.GetString(2);

                    Doctor_ doctor = new Doctor_(Id, Name + " " + Surname);
                    doctors.Add(doctor);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            finally
            {
                connection.Close();
            }
            #endregion

            #region bd подключенеи юзера

            try
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();
                String query = "SELECT   [USER_INFO].[ID],  [USER_INFO].[Name],  [USER_INFO].[Surname], [USER_INFO].[Patronymic] FROM [USER_INFO]";
                SqlCommand command = new SqlCommand(query, connection);
                command.CommandType = CommandType.Text;
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    int Id = reader.GetInt32(0);
                    string Name = reader.GetString(1);
                    string Surname = reader.GetString(2);
                    string Patronymic = reader.GetString(3);

                    User user = new User(Id, Surname + " " + Name + " " + Patronymic);//фио
                    users.Add(user);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                connection.Close();
            }

            #endregion  //доделать поиск юзеров

            #region bd подключение процедур

            try
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();
                String query = "SELECT [PROCEDURE_INFO].[Procedure], [PROCEDURE_INFO].[ID]  FROM [PROCEDURE_INFO]";
                SqlCommand command = new SqlCommand(query, connection);
                command.CommandType = CommandType.Text;
                SqlDataReader reader = command.ExecuteReader();


                while (reader.Read())
                {
                    string Name = reader.GetString(0);
                    int ID = reader.GetInt32(1);

                    Procedure procedure = new Procedure(Name, ID);
                    procedures.Add(procedure);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            finally
            {
                connection.Close();
            }
            #endregion

        }

        private void Bt_Click(object sender, RoutedEventArgs e) //поиск по бд есть ли у врача в этот день приемы
        {
            if (Date.Text != "" && Doctor.Text != "")
            {
                SrchSchedule();
            }
            else
            {
                MessageBox.Show("Выберите время и доктора!");
            }
        }

        void SrchSchedule()
        {
            List<Time> timetables = new List<Time>();
            #region bd

            try
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                String query = "SELECT [SCHEDULE].[TimeStart],  [USER_INFO].[Surname], [USER_INFO].[Name],  [PROCEDURE_INFO].[Procedure],  [PROCEDURE_INFO].[Durability], [PROCEDURE_INFO].[Price]" +
                    " FROM [SCHEDULE]  INNER JOIN  [USER_INFO] " +
                    "ON [SCHEDULE].[UserId] =  [USER_INFO].[ID] " +
                    " INNER JOIN  [PROCEDURE_INFO] ON [PROCEDURE_INFO].[ID] = [SCHEDULE].[ProcedureId] " +
                    "WHERE [SCHEDULE].[DoctorId] = @DoctorSearch AND [Date]=@DateSearch ";
                SqlCommand command = new SqlCommand(query, connection);
                command.CommandType = CommandType.Text;
                int dc = 0;
                for (int i = 0; i < doctors.Count; i++)
                {
                    if (doctors[i].FIO == Doctor.Text)
                        dc = doctors[i].Id;
                }
                command.Parameters.AddWithValue("@DoctorSearch", dc); // id
                command.Parameters.AddWithValue("@DateSearch", Date.SelectedDate.Value);
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    TimeSpan time = reader.GetTimeSpan(0);
                    string user = reader.GetString(1) + " " + reader.GetString(2); // FIO user ///////
                    string procinfo = reader.GetTimeSpan(4).ToString() ;
                    Time timetable = new Time(time, user, procinfo);
                    timetables.Add(timetable);
                }

                if (timetables != null)
                {
                    Show(timetables);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                connection.Close();
            }

            #endregion

        }

        #region Show
        void Show(List<Time> timetables)
        {

            #region Clean

            Time10.Text = "";
            Time10.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#C9C9C9"));


            Time11.Text = "";
            Time11.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#C9C9C9"));


            Time12.Text = "";
            Time12.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#C9C9C9"));

            Time13.Text = "";
            Time13.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#C9C9C9"));

            Time14.Text = "";
            Time14.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#C9C9C9"));

            Time15.Text = "";
            Time15.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#C9C9C9"));

            Time16.Text = "";
            Time16.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#C9C9C9"));

            Time17.Text = "";
            Time17.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#C9C9C9"));

            Time18.Text = "";
            Time18.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#C9C9C9"));


            #endregion

            if (timetables != null && timetables.Count != 0)
            {
                for (int i = 0; i < timetables.Count; i++)
                {
                    if (timetables[i].time.ToString() == "10:00:00") //11:00:00,  11.11.2011

                    //6/1/2009 (Short Date String)
                    {
                        Time10.Text = timetables[i].user + " " + timetables[i].ProcInfo;
                        Time10.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFD3113D"));

                    }
                    if (timetables[i].time.ToString() == "11:00:00")
                    {
                        Time11.Text = timetables[i].user + " " + timetables[i].ProcInfo;
                        Time11.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFD3113D"));

                    }
                    if (timetables[i].time.ToString() == "12:00:00")
                    {
                        Time12.Text = timetables[i].user + " " + timetables[i].ProcInfo;
                        Time12.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFD3113D"));

                    }
                    if (timetables[i].time.ToString() == "13:00:00")
                    {
                        Time13.Text = timetables[i].user + " " + timetables[i].ProcInfo;
                        Time13.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFD3113D"));

                    }
                    if (timetables[i].time.ToString() == "14:00:00")
                    {
                        Time14.Text = timetables[i].user + " " + timetables[i].ProcInfo;
                        Time14.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFD3113D"));

                    }
                    if (timetables[i].time.ToString() == "15:00:00")
                    {
                        Time15.Text = timetables[i].user + " " + timetables[i].ProcInfo;
                        Time15.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFD3113D"));

                    }
                    if (timetables[i].time.ToString() == "16:00:00")
                    {
                        Time16.Text = timetables[i].user + " " + timetables[i].ProcInfo;
                        Time16.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFD3113D"));

                    }
                    if (timetables[i].time.ToString() == "17:00:00")
                    {
                        Time17.Text = timetables[i].user + " " + timetables[i].ProcInfo;
                        Time17.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFD3113D"));

                    }
                    if (timetables[i].time.ToString() == "18:00:00")
                    {
                        Time18.Text = timetables[i].user + " " + timetables[i].ProcInfo;
                        Time18.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFD3113D"));

                    }

                }
            }
        }
        #endregion

        private void TimeClick(object sender, RoutedEventArgs e)//кнопка добавить
        {
            string name = ((Button)sender).Name;
            switch (name)
            {
                case "Btn10":
                    timeschedule = new TimeSpan(10, 0, 0);
                    break;
                case "Btn11":
                    timeschedule = new TimeSpan(11, 0, 0);
                    break;
                case "Btn12":
                    timeschedule = new TimeSpan(12, 0, 0);
                    break;
                case "Btn13":
                    timeschedule = new TimeSpan(13, 0, 0);
                    break;
                case "Btn14":
                    timeschedule = new TimeSpan(14, 0, 0);
                    break;
                case "Btn15":
                    timeschedule = new TimeSpan(15, 0, 0);
                    break;
                case "Btn16":
                    timeschedule = new TimeSpan(16, 0, 0);
                    break;
                case "Btn17":
                    timeschedule = new TimeSpan(17, 0, 0);
                    break;
                case "Btn18":
                    timeschedule = new TimeSpan(18, 0, 0);
                    break;
            }
        }
    }
}
