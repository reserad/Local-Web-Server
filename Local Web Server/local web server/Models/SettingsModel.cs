using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Local_Web_Server.Models
{
    public class SettingsModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Service")]
        public bool Service { get; set; }


        public void SaveSettings(string _username, bool _service)
        {
            using (var cn = new SqlConnection(@"Data Source=(LocalDB)\v11.0;AttachDbFilename" +
             "=|DataDirectory|\\Database.mdf; Integrated Security=True"))
            {

                var _email = "";
                string _emailSql = @"SELECT Top 1 [Email] FROM [dbo].[System_Users] " +
                       @"WHERE [Username] = @u";
                var emailCmd = new SqlCommand(_emailSql, cn);
                emailCmd.Parameters
                    .Add(new SqlParameter("@u", SqlDbType.NVarChar))
                    .Value = _username;
                cn.Open();
                var emailReader = emailCmd.ExecuteReader();
                
                if (emailReader.HasRows)
                {
                    while (emailReader.Read())
                    {
                        _email = emailReader["Email"].ToString();
                    }
                }
                cn.Close();
                emailReader.Dispose();
                emailCmd.Dispose();

                string _sql = @"SELECT [Service] FROM [dbo].[Settings] " +
                       @"WHERE [Username] = @u";
                var cmd = new SqlCommand(_sql, cn);
                cmd.Parameters
                    .Add(new SqlParameter("@u", SqlDbType.NVarChar))
                    .Value = _username;
                cn.Open();
                var reader = cmd.ExecuteReader();
                
                if (!reader.HasRows)
                {
                    cn.Close();
                    reader.Dispose();
                    cmd.Dispose();

                    string _insertSql = @"INSERT INTO [dbo].[Settings] 
                                        ([Username], [Email], [Service])
                                        VALUES (@u, @e, @s)";
                    var insertCmd = new SqlCommand(_insertSql, cn);
                    insertCmd.Parameters
                        .Add(new SqlParameter("@u", SqlDbType.NVarChar))
                        .Value = _username;
                    insertCmd.Parameters
                        .Add(new SqlParameter("@e", SqlDbType.NVarChar))
                        .Value = _email;
                    insertCmd.Parameters
                        .Add(new SqlParameter("@s", SqlDbType.Bit))
                        .Value = _service;
                    cn.Open();
                    insertCmd.ExecuteNonQuery();
                    cn.Close();
                }
                else
                {
                    cn.Close();
                    reader.Dispose();
                    cmd.Dispose();

                    string _insertSql = @"UPDATE [dbo].[Settings] 
                                        SET Service = '" + _service + "' WHERE Username = '" + _username + "' AND Email = '" + _email + "'";
                    var insertCmd = new SqlCommand(_insertSql, cn);
                    insertCmd.Parameters
                        .Add(new SqlParameter("@u", SqlDbType.NVarChar))
                        .Value = _username;
                    insertCmd.Parameters
                        .Add(new SqlParameter("@e", SqlDbType.NVarChar))
                        .Value = _email;
                    insertCmd.Parameters
                        .Add(new SqlParameter("@s", SqlDbType.Bit))
                        .Value = _service;
                    cn.Open();
                    insertCmd.ExecuteNonQuery();
                    insertCmd.Dispose();
                    cn.Close();
                }
            }
        }
        public SettingsModel getServiceSetting(string _username) 
        {
            SettingsModel settings = new SettingsModel();
            using (var cn = new SqlConnection(@"Data Source=(LocalDB)\v11.0;AttachDbFilename" +
             "=|DataDirectory|\\Database.mdf; Integrated Security=True"))
            {
                string _sql = @"SELECT TOP 1 [Service], [Email] FROM [dbo].[Settings] " +
                       @"WHERE [Username] = @u";
                var cmd = new SqlCommand(_sql, cn);
                cmd.Parameters
                    .Add(new SqlParameter("@u", SqlDbType.NVarChar))
                    .Value = _username;
                cn.Open();
                var reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {

                    while (reader.Read())
                    {
                        settings.Email = reader["Email"].ToString();
                        settings.UserName = _username;
                        settings.Service = Convert.ToBoolean(reader["Service"]);
                    }
                }
                reader.Dispose();
                cmd.Dispose();
                cn.Close();
            }
            return settings;
        }
    }
}