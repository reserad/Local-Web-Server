using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;

namespace Local_Web_Server.Models
{
    public class User
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "LastAtivity")]
        public DateTime LastActivity { get; set; }

        public bool IsValid(string _username, string _password)
        {
            using (var cn = new SqlConnection(@"Data Source=(LocalDB)\v11.0;AttachDbFilename" +
             "=|DataDirectory|\\Database.mdf; Integrated Security=True"))
            {
                string _sql = @"SELECT [Username] FROM [dbo].[System_Users] " +
                       @"WHERE [Username] = @u AND [Password] = @p";
                var cmd = new SqlCommand(_sql, cn);
                cmd.Parameters
                    .Add(new SqlParameter("@u", SqlDbType.NVarChar))
                    .Value = _username;
                cmd.Parameters
                    .Add(new SqlParameter("@p", SqlDbType.NVarChar))
                    .Value = Helpers.SHA1.Encode(_password);
                cn.Open();
                var reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    reader.Dispose();
                    cmd.Dispose();
                    return true;
                }
                else
                {
                    reader.Dispose();
                    cmd.Dispose();
                    return false;
                }
            }
        }
        public void CreateAccount(string _username, string _password, string _email) 
        {
            Random randomGen = new Random();
            KnownColor[] names = (KnownColor[])Enum.GetValues(typeof(KnownColor));
            KnownColor randomColorName = names[randomGen.Next(names.Length)];
            Color randomColor = Color.FromKnownColor(randomColorName);
            string color = "#" + randomColor.R.ToString("X2") + randomColor.G.ToString("X2") + randomColor.B.ToString("X2");

            using (var cn = new SqlConnection(@"Data Source=(LocalDB)\v11.0;AttachDbFilename" +
            "=|DataDirectory|\\Database.mdf; Integrated Security=True"))
            {
                string _sql = @"SELECT [Username] FROM [dbo].[System_Users] " +
                       @"WHERE [Username] = @u";
                var cmd = new SqlCommand(_sql, cn);
                cmd.Parameters
                    .Add(new SqlParameter("@u", SqlDbType.NVarChar))
                    .Value = _username;
                cn.Open();
                var reader = cmd.ExecuteReader();
                if (!reader.HasRows)
                {
                    reader.Dispose();
                    cmd.Dispose();

                    if(_username != null &&_password != null)
                    {
                        string _insertSql = @"INSERT INTO [dbo].[System_Users] 
                                            ([Username], [Password], [Email], [LastActivity], [ColorIdentification])
                                            VALUES (@u, @p, @e, @l, @color)";
                        var insertCmd = new SqlCommand(_insertSql, cn);
                        insertCmd.Parameters
                            .Add(new SqlParameter("@u", SqlDbType.NVarChar))
                            .Value = _username;
                        insertCmd.Parameters
                            .Add(new SqlParameter("@p", SqlDbType.NVarChar))
                            .Value = Helpers.SHA1.Encode(_password);
                        insertCmd.Parameters
                            .Add(new SqlParameter("@e", SqlDbType.NVarChar))
                            .Value = _email;
                        insertCmd.Parameters
                            .Add(new SqlParameter("@l", SqlDbType.DateTime))
                            .Value = System.DateTime.Now;
                        insertCmd.Parameters
                            .Add(new SqlParameter("@color", SqlDbType.NVarChar))
                            .Value = color;
                        insertCmd.ExecuteNonQuery();
                    }
                }
            }
        }
        public void UpdateActivityTime(string _username) 
        {
            using (var cn = new SqlConnection(@"Data Source=(LocalDB)\v11.0;AttachDbFilename" +
           "=|DataDirectory|\\Database.mdf; Integrated Security=True"))
            {
                string _updateSql = @"UPDATE [dbo].[System_Users] 
                                        SET LastActivity = @l WHERE Username = @u";
                var updateCmd = new SqlCommand(_updateSql, cn);
                updateCmd.Parameters
                    .Add(new SqlParameter("@u", SqlDbType.NVarChar))
                    .Value = _username;
                updateCmd.Parameters
                    .Add(new SqlParameter("@l", SqlDbType.NVarChar))
                    .Value = System.DateTime.Now;
                cn.Open();
                updateCmd.ExecuteNonQuery();
                updateCmd.Dispose();
                cn.Close();
            } 
        }
        public List<User> AllUsers() 
        {
            List<User> Users = new List<User>();
            using (var cn = new SqlConnection(@"Data Source=(LocalDB)\v11.0;AttachDbFilename" +
             "=|DataDirectory|\\Database.mdf; Integrated Security=True"))
            {
                string _sql = @"SELECT [Username], [LastActivity] FROM [dbo].[System_Users]";
                var cmd = new SqlCommand(_sql, cn);
                cn.Open();
                var reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while(reader.Read())
                    {
                        User item = new User();
                        item.UserName = reader["Username"].ToString();
                        item.LastActivity = Convert.ToDateTime(reader["LastActivity"].ToString());
                        Users.Add(item);
                    }
                    reader.Dispose();
                    cmd.Dispose();
                }
            }
            return Users;
        }
        public string GetColorByUsername(string username) 
        {
            string color = "";
            using (var cn = new SqlConnection(@"Data Source=(LocalDB)\v11.0;AttachDbFilename" +
             "=|DataDirectory|\\Database.mdf; Integrated Security=True"))
            {
                string _sql = @"SELECT [ColorIdentification] FROM [dbo].[System_Users] WHERE [Username] = @u";
                var cmd = new SqlCommand(_sql, cn);
                cmd.Parameters
                    .Add(new SqlParameter("@u", SqlDbType.NVarChar))
                    .Value = username;
                cn.Open();
                var reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        color = reader["ColorIdentification"].ToString();
                    }
                    reader.Dispose();
                    cmd.Dispose();
                }
            }
            return color;
        }
    }
}