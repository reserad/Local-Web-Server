using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Local_Web_Server.Models
{
    public class WeatherModel
    {
        public info current_observation { get; set; }
        public struct info 
        {
            public decimal temp_f { get; set; }
            public string windchill_f { get; set; }
            public decimal wind_mph { get; set; }
            public string icon_url { get; set; }
            public DateTime LastRefresh { get; set; }
        }

        public info GetLatestWeather()
        {
            info item = new info();
            using (var cn = new SqlConnection(@"Data Source=(LocalDB)\v11.0;AttachDbFilename" +
             "=|DataDirectory|\\Database.mdf; Integrated Security=True"))
            {
                string _sql = @"SELECT * FROM [dbo].[Weather]";
                var cmd = new SqlCommand(_sql, cn);
                cn.Open();
                var reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    try
                    {
                        while (reader.Read())
                        {
                            item.LastRefresh = Convert.ToDateTime(reader["LastRefresh"].ToString());
                            item.temp_f = Convert.ToDecimal(reader["Temperature"].ToString());
                            item.windchill_f = reader["Class"].ToString();
                            item.wind_mph = Convert.ToDecimal(reader["WindSpeed"].ToString());
                            item.icon_url = reader["Icon"].ToString();
                        }
                    }
                    catch (Exception e) {}
                }
                reader.Dispose();
                cmd.Dispose();
            }
            return item;
        }

        public void UpdateRefresh(info s)
        {
            using (var cn = new SqlConnection(@"Data Source=(LocalDB)\v11.0;AttachDbFilename" +
             "=|DataDirectory|\\Database.mdf; Integrated Security=True"))
            {
                string _insertSql = @"UPDATE [dbo].[Weather] 
                                    SET LastRefresh = @LastRefresh, Temperature = @Temperature, WindChill = @WindChill, Icon = @Icon, WindSpeed = @WindSpeed WHERE Id = 1";
                var insertCmd = new SqlCommand(_insertSql, cn);
                insertCmd.Parameters
                    .Add(new SqlParameter("@LastRefresh", SqlDbType.DateTime))
                    .Value = s.windchill_f;
                insertCmd.Parameters
                    .Add(new SqlParameter("@Temperature", SqlDbType.NVarChar))
                    .Value = s.LastRefresh;
                insertCmd.Parameters
                    .Add(new SqlParameter("@WindChill", SqlDbType.NVarChar))
                    .Value = s.icon_url;
                insertCmd.Parameters
                    .Add(new SqlParameter("@Icon", SqlDbType.NVarChar))
                    .Value = s.temp_f;
                insertCmd.Parameters
                    .Add(new SqlParameter("@WindSpeed", SqlDbType.NVarChar))
                    .Value = s.wind_mph;
                cn.Open();
                insertCmd.ExecuteNonQuery();
                cn.Close();
            }
        }

        public void CreateWeather(info s)
        {
            using (var cn = new SqlConnection(@"Data Source=(LocalDB)\v11.0;AttachDbFilename" +
             "=|DataDirectory|\\Database.mdf; Integrated Security=True"))
            {
                string _insertSql = @"INSERT INTO [dbo].[Weather] 
                                            ([LastRefresh], [Temperature], [WindChill], [Icon], [WindSpeed])
                                            VALUES (@LastRefresh, @Temperature, @WindChill, @Icon, @WindSpeed)";
                var insertCmd = new SqlCommand(_insertSql, cn);
                insertCmd.Parameters
                    .Add(new SqlParameter("@LastRefresh", SqlDbType.DateTime))
                    .Value = DateTime.Now;
                insertCmd.Parameters
                    .Add(new SqlParameter("@Temperature", SqlDbType.NVarChar))
                    .Value = s.temp_f;
                insertCmd.Parameters
                    .Add(new SqlParameter("@WindChill", SqlDbType.NVarChar))
                    .Value = s.windchill_f;
                insertCmd.Parameters
                    .Add(new SqlParameter("@Icon", SqlDbType.NVarChar))
                    .Value = s.icon_url;
                insertCmd.Parameters
                    .Add(new SqlParameter("@WindSpeed", SqlDbType.NVarChar))
                    .Value = s.wind_mph;
                cn.Open();
                insertCmd.ExecuteNonQuery();
                cn.Close();
            }
        }

        public void Truncate()
        {
            using (var cn = new SqlConnection(@"Data Source=(LocalDB)\v11.0;AttachDbFilename" +
             "=|DataDirectory|\\Database.mdf; Integrated Security=True"))
            {
                string _sql = @"TRUNCATE TABLE Weather";
                var cmd = new SqlCommand(_sql, cn);
                cn.Open();
                var reader = cmd.ExecuteReader();
                reader.Dispose();
                cmd.Dispose();
            }
        }
    }
}