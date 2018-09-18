using Stomatology.Class;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Stomatology.Wind
{
    /// <summary>
    /// Логика взаимодействия для TimetablePage.xaml
    /// </summary>
    public partial class TimetablePage : Page
    {
        public List<Doctor_> doctors { get; private set; }
        public List<Time> timetables { get; private set; }
        public List<User> users { get; private set; }
        public List<Procedure> procedures { get; private set; }

        public SqlConnection connection =  new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename='|DataDirectory|\DataBase\stm.mdf';Integrated Security=True;Connect Timeout=30");

        public TimeSpan timeschedule = new TimeSpan(0, 0, 0);

        public TimetablePage()
        {
            InitializeComponent();

            doctors = new List<Doctor_>();//заполняются при первом обр6ащении
            users = new List<User>();
            procedures = new List<Procedure>();


            StartWorking();

            Doctor.ItemsSource = doctors;
            txt_recedure.ItemsSource = procedures;
        }

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

                    User user = new User(Id, Surname + " " + Name + " " + Patronymic);//фио слияние в одну строку
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
            if(Date.Text != "" && Doctor.Text != "")
            {
                SrchSchedule();
                RegistBt.IsEnabled = true;
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
                    string procinfo = reader.GetString(3) + " " + reader.GetTimeSpan(4).ToString() + " ; " + reader.GetSqlMoney(5).ToString() + " руб.";
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

        #region Convert 
        int IdProc(string prsSrch) //срабатывает регистрации процедур, находит по строке нужны ай ди из списков
        {
            int idProc = 0;
            if (procedures != null && procedures.Count != 0)
            {
                for (int i = 0; i < procedures.Count; i++)
                {
                    if (procedures[i].Name.ToString() == prsSrch)
                        idProc = procedures[i].ID;
                }
            }        return idProc;
        }
        
        int UserId()
        {
            int dc = 0;
            for (int i = 0; i < users.Count; i++)
            {
                if (users[i].FIO == txt_usr.Text)
                    dc = users[i].Id;
            }
            return dc;
        }

        int DoctorId()
        {
            int dc = 0;
            for (int i = 0; i < doctors.Count; i++)
            {
                if (doctors[i].FIO == Doctor.Text)
                    dc = doctors[i].Id;
            }
            return dc;
        }

        TimeSpan TimeStartProc(string time)
        {
            TimeSpan timeStartProc = new TimeSpan(0, 0, 0);
           
            switch (time)
            {
                case "10:00":
                    timeStartProc = new TimeSpan(10, 0, 0);
                    break;
                case "11:00":
                    timeStartProc = new TimeSpan(11, 0, 0);
                    break;
                case "12:00":
                    timeStartProc = new TimeSpan(12, 0, 0);
                    break;
                case "13:00":
                    timeStartProc = new TimeSpan(13, 0, 0);
                    break;
                case "14:00":
                    timeStartProc = new TimeSpan(14, 0, 0);
                    break;
                case "15:00":
                    timeStartProc = new TimeSpan(15, 0, 0);
                    break;
                case "16:00":
                    timeStartProc = new TimeSpan(16, 0, 0);
                    break;
                case "17:00":
                    timeStartProc = new TimeSpan(17, 0, 0);
                    break;
                case "18:00":
                    timeStartProc = new TimeSpan(18, 0, 0);
                    break;
            }
            return timeStartProc;

        }
        #endregion

        private void RegestrPriem_Click(object sender, RoutedEventArgs e)//регестрация приема
        {
           IdProc(txt_recedure.Text);
           try
            {
                if (Date.SelectedDate.Value < DateTime.Now && Date.SelectedDate.Value > DateTime.Now.AddMonths(1)) //если дата уже прошла или еще слишком рано регестрироваться
                {
                    MessageBox.Show("Проверьте выбранную дату!");
                    return;
                }
                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                String query = "INSERT INTO [SCHEDULE] (UserId, DoctorId, Date, TimeStart, ProcedureId) values (@UserId, @DoctorId, @Date, @TimeStart, @ProcedureId)";
                SqlCommand sqlCmd = new SqlCommand(query, connection);
                sqlCmd.CommandType = CommandType.Text;
                sqlCmd.Parameters.AddWithValue("@UserId", UserId()); 
                sqlCmd.Parameters.AddWithValue("@DoctorId", DoctorId());
                sqlCmd.Parameters.AddWithValue("@Date", Date.SelectedDate.Value);
                sqlCmd.Parameters.AddWithValue("@TimeStart", TimeStartProc(txt_time.Text));
                sqlCmd.Parameters.AddWithValue("@ProcedureId", IdProc(txt_recedure.Text));
                sqlCmd.ExecuteNonQuery();//выполнеяет запрос query

                MessageBox.Show("готово!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                SrchSchedule();
                connection.Close();
            }
        }

        void Delete()//удаление
        {
            IdProc(txt_recedure.Text);
           try
            {

                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                String query = "DELETE FROM [SCHEDULE]  WHERE TimeStart LIKE @TimeStart"; 
                SqlCommand sqlCmd = new SqlCommand(query, connection);
                sqlCmd.CommandType = CommandType.Text;
                sqlCmd.Parameters.AddWithValue("@TimeStart", timeschedule);
                sqlCmd.ExecuteNonQuery();//выполнеяет запрос query

                MessageBox.Show("Удалено!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                SrchSchedule();
                connection.Close();
            }

        }

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
            Delete();
        }
    }
}