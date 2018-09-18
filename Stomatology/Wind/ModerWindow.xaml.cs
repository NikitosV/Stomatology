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
using System.Data.Common;

namespace Stomatology.Wind
{
    /// <summary>
    /// Логика взаимодействия для ModerWindow.xaml
    /// </summary>
    public partial class ModerWindow : Window
    {
        TextBox txID, txSp, txEx, txN, txSur, txPat, txMob, txEm;
        DatePicker txBirth;
        PasswordBox txPass;

        //SqlDataAdapter adap;
        //DataSet ds;

        public SqlConnection sqlCon = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename='|DataDirectory|\DataBase\stm.mdf';Integrated Security=True;Connect Timeout=30");//подключение бд
       
        public ModerWindow()
        {
            InitializeComponent();

            txID = (TextBox)FindName("txt_regID");
            txSp = (TextBox)FindName("txt_regSpec");
            txEx = (TextBox)FindName("txt_regExp");
            txN = (TextBox)FindName("txt_regName");
            txSur = (TextBox)FindName("txt_regSurname");
            txPat = (TextBox)FindName("txt_regPantronymic");
            txMob = (TextBox)FindName("txt_regMobile");
            txEm = (TextBox)FindName("txt_regEmail");

            txPass = (PasswordBox)FindName("txt_regPass");
            txBirth = (DatePicker)FindName("txt_regbirth");


            Name.Text = Stomatology.Wind.LoginWindow.Online_person;

            Display_Data();
        }

        #region =Menu=
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

        private void MinimizedWin_Click(object sender, RoutedEventArgs e)//свернуть
        {
            this.WindowState = WindowState.Minimized;
        }
        #endregion

        #region=PopUp=
        private void HelpBut_Click(object sender, RoutedEventArgs e)
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

        #region =Кнопки работы c DataBase=
        private void Insert_Click(object sender, RoutedEventArgs e)//подтвердить
        {
            try
            {
                if (sqlCon.State == ConnectionState.Closed)//проверка на подключение к бд
                {
                    if (txt_insertID.Text != "" && txt_insertPassword.Text != "" && txt_insertRole.Text != "")//проверка на пустые строки
                    {
                        if (txt_insertRole.Text == "user" || txt_insertRole.Text == "admin" || txt_insertRole.Text == "moder")//проверка на нужную роль
                        {
                            sqlCon.Open();
                            SqlCommand cmd = sqlCon.CreateCommand();
                            cmd.CommandType = CommandType.Text;
                            cmd.CommandText = "insert into [USER] (ID, password, role) values ('" + txt_insertID.Text + "', '" + txt_insertPassword.Text + "', '" + txt_insertRole.Text + "')";
                            cmd.ExecuteNonQuery();
                            sqlCon.Close();
                            Display_Data();

                            txt_insertID.Text = "";
                            txt_insertPassword.Text = "";
                            txt_insertRole.Text = "";

                            MessageBox.Show("Добавлено!");
                        }
                        else
                        {
                            MessageBox.Show("Роль может быть только:\nuser\nadmin\nmoder");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Поля не могут быть пустые!");
                    }
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

        public void Display_Data() //показ DataBase [USER]
        {
            try
            {
                if (sqlCon.State == ConnectionState.Closed)
                {
                    sqlCon.Open();
                    SqlCommand cmd = sqlCon.CreateCommand();
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "SELECT * FROM [USER]";
                    cmd.ExecuteNonQuery();
                    DataTable dta = new DataTable();
                    SqlDataAdapter dtaAdp = new SqlDataAdapter(cmd);
                    dtaAdp.Fill(dta);
                    dataGridViewUsers.ItemsSource = dta.DefaultView;
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

        private void Refuse_Click(object sender, RoutedEventArgs e)//обновить DB
        {
            Display_Data();
        }

        private void Delete_CLick(object sender, RoutedEventArgs e)//удалить запись
        {
            try
            {
                if (sqlCon.State == ConnectionState.Closed)
                {
                    if (txt_insertID.Text != "")//проверка на пустые строки
                    {
                        sqlCon.Open();
                        SqlCommand cmd = sqlCon.CreateCommand();
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = "DELETE FROM [USER] WHERE ID = '" + txt_insertID.Text + "'";
                        cmd.ExecuteNonQuery();
                        Display_Data();

                        txt_insertID.Text = "";
                        txt_insertPassword.Text = "";
                        txt_insertRole.Text = "";

                        MessageBox.Show("Удалено!");
                    }
                    else
                    {
                        MessageBox.Show("Поля не могут быть пустые!");
                    }
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

        private void Update_Click(object sender, RoutedEventArgs e)//изменить запись
        {
            try
            {
                if(sqlCon.State == ConnectionState.Closed)
                {
                    sqlCon.Open();
                    SqlCommand cmd = sqlCon.CreateCommand();
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "UPDATE [USER] SET [ID] = '" + this.txt_insertID.Text + "', [Password] = '" + this.txt_insertPassword.Text + "','" + this.txt_insertRole.Text + "' WHERE [ID] = '" + this.txt_insertID.Text + "'";
                    cmd.ExecuteNonQuery();
                    Display_Data();

                    txt_insertID.Text = "";
                    txt_insertPassword.Text = "";
                    txt_insertRole.Text = "";

                    MessageBox.Show("Изменено!");
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

        private void SearchLog_Click(object sender, RoutedEventArgs e)//поиск по логину
        {
            try
            {
                if (sqlCon.State == ConnectionState.Closed)
                {
                    sqlCon.Open();
                    SqlCommand cmd = sqlCon.CreateCommand();
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "SELECT * FROM [USER] WHERE ID = '" + txt_SearchID.Text + "'";
                    cmd.ExecuteNonQuery();
                    DataTable dt = new DataTable();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                    dataGridViewUsers.ItemsSource = dt.DefaultView;
                    sqlCon.Close();
                    txt_SearchID.Text = "";
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

     
        #endregion

        #region =Doctor=
        public string doctor = "doctor";
        private void InsertDoctor_Click(object sender, RoutedEventArgs e)//добавление доктора
        {
            try
            {
                if (sqlCon.State == ConnectionState.Closed)
                {
                    if(txt_regID.Text != "" && txt_regPass.Password != "" && txt_regName.Text != "" && txt_regSurname.Text != "" && txt_regPantronymic.Text != "" && txt_regSpec.Text != "" && txt_regEmail.Text != "" && txt_regExp.Text != "" && txt_regMobile.Text != "" && txt_regbirth.SelectedDate.HasValue)//проверка на пустые строки
                    {
                        sqlCon.Open();

                        String quer = "INSERT INTO [USER] (ID, password, role) values (@user, @pass, @role)";
                        SqlCommand cmd = new SqlCommand(quer, sqlCon);
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@user", txID.Text);
                        cmd.Parameters.AddWithValue("@pass", txPass.Password);
                        cmd.Parameters.AddWithValue("@role", doctor);
                        cmd.ExecuteNonQuery();

                        //cmd.CommandType = CommandType.Text;
                        //cmd.CommandText = "insert into [USER] (ID, password, role) values ('" + txt_regID.Text + "', '" + txt_regPass + "', '" + doctor + "')";

                        String quer2 = "INSERT INTO [DOCTOR_INFO] (DoctorId, Specialization, Experience, Name, Surname, Patronymic, Mobile, Email, DateBirth) values (@DoctorId, @specializ, @exp, @name, @surname, @patronymic, @mob, @email, @dateb)";
                        SqlCommand cmd2 = new SqlCommand(quer2, sqlCon);
                        cmd2.CommandType = CommandType.Text;
                        cmd2.Parameters.AddWithValue("@DoctorId", txID.Text);
                        cmd2.Parameters.AddWithValue("@specializ", txSp.Text);
                        cmd2.Parameters.AddWithValue("@exp", txEx.Text);
                        cmd2.Parameters.AddWithValue("@name", txN.Text);
                        cmd2.Parameters.AddWithValue("@surname", txSur.Text);
                        cmd2.Parameters.AddWithValue("@patronymic", txPat.Text);
                        cmd2.Parameters.AddWithValue("@mob", txMob.Text);
                        cmd2.Parameters.AddWithValue("@email", txEm.Text);
                        cmd2.Parameters.AddWithValue("@dateb", txBirth.SelectedDate);

                        //cmd2.CommandText = "insert into [DOCTOR_INFO] (UserId, Specialization, Experience, Name, Surname, Patronymic, Mobile, Email, DateBirth) values ('" + txt_regID + "', '" +
                        //    txt_regSpec + "', '"  + txt_regExp + "', '" + txt_regName + "', '" + txt_regSurname + "', '" + txt_regPantronymic + "', '" + txt_regMobile + "', '" + txt_regEmail + "', '" + txt_regbirth + "')";
                        cmd2.ExecuteNonQuery();

                        MessageBox.Show("Congrulation!");
                        sqlCon.Close();
                        Display_Data();

                        txt_regID.Text = "";
                        txt_regSpec.Text = "";
                        txt_regExp.Text = "";
                        txt_regName.Text = "";
                        txt_regSurname.Text = "";
                        txt_regPantronymic.Text = "";
                        txt_regMobile.Text = "";
                        txt_regEmail.Text = "";
                    }
                    else
                    {
                        MessageBox.Show("Все поля доджны быть заполнены!");
                    }
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
        #endregion

    }
}