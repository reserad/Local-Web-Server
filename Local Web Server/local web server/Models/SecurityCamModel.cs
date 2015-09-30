using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Local_Web_Server.Models
{
    public class SecurityCam
    {
        public string Owner { get; set; }
        public string Address { get; set; }
        public string Location { get; set; }
        public int Id { get; set; }

        public void CreateCamera(SecurityCam s)
        {
            using (var cn = new SqlConnection(@"Data Source=(LocalDB)\v11.0;AttachDbFilename" +
             "=|DataDirectory|\\Database.mdf; Integrated Security=True"))
            {
                string _insertSql = @"INSERT INTO [dbo].[RegisteredCameras] 
                                            ([Owner], [Address], [Location])
                                            VALUES (@Owner, @Address, @Location)";
                var insertCmd = new SqlCommand(_insertSql, cn);
                insertCmd.Parameters
                    .Add(new SqlParameter("@Owner", SqlDbType.NVarChar))
                    .Value = s.Owner;
                insertCmd.Parameters
                    .Add(new SqlParameter("@Address", SqlDbType.NVarChar))
                    .Value = s.Address;
                insertCmd.Parameters
                    .Add(new SqlParameter("@Location", SqlDbType.NVarChar))
                    .Value = s.Location;
                cn.Open();
                insertCmd.ExecuteNonQuery();
                cn.Close();
            }
        }
        public List<SecurityCam> getCameras()
        {
            List<SecurityCam> items = new List<SecurityCam>();
            using (var cn = new SqlConnection(@"Data Source=(LocalDB)\v11.0;AttachDbFilename" +
             "=|DataDirectory|\\Database.mdf; Integrated Security=True"))
            {
                string _sql = @"SELECT * FROM [dbo].[RegisteredCameras]";
                var cmd = new SqlCommand(_sql, cn);
                cn.Open();
                var reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    try
                    {
                        while (reader.Read())
                        {
                            SecurityCam item = new SecurityCam();
                            item.Id = Convert.ToInt32(reader["Id"].ToString());
                            item.Owner = reader["Owner"].ToString();
                            item.Address = reader["Address"].ToString();
                            item.Location = reader["Location"].ToString();
                            items.Add(item);
                        }
                    }
                    catch (Exception e) { return items; }

                }
                reader.Dispose();
                cmd.Dispose();
            }
            return items;
        }
        public void deleteCameraById(int id)
        {
            using (var cn = new SqlConnection(@"Data Source=(LocalDB)\v11.0;AttachDbFilename" +
             "=|DataDirectory|\\Database.mdf; Integrated Security=True"))
            {
                string _sql = @"DELETE FROM [dbo].[RegisteredCameras] WHERE [Id] = @i ";
                var cmd = new SqlCommand(_sql, cn);
                cmd.Parameters
                    .Add(new SqlParameter("@i", SqlDbType.NVarChar))
                    .Value = id;
                cn.Open();
                var reader = cmd.ExecuteReader();
                reader.Dispose();
                cmd.Dispose();
            }
        }
    }
}