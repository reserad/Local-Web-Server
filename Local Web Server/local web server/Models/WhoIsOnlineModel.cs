using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
namespace Local_Web_Server.Models
{
    public class WhoIsOnlineModel
    {
        public int Id { get; set; }
        public string Address { get; set; }
        public string Owner { get; set; }
        public string DeviceType { get; set; }
        public string HostName { get; set; }

        public void CreateEntry(WhoIsOnlineModel s)
        {
            using (var cn = new SqlConnection(@"Data Source=(LocalDB)\v11.0;AttachDbFilename" +
             "=|DataDirectory|\\Database.mdf; Integrated Security=True"))
            {
                string _insertSql = @"INSERT INTO [dbo].[MacAddresses] 
                                            ([DeviceOwner], [MacAddress], [TypeOfDevice])
                                            VALUES (@DeviceOwner, @MacAddress, @TypeOfDevice)";
                var insertCmd = new SqlCommand(_insertSql, cn);
                insertCmd.Parameters
                    .Add(new SqlParameter("@DeviceOwner", SqlDbType.NVarChar))
                    .Value = s.Owner;
                insertCmd.Parameters
                    .Add(new SqlParameter("@MacAddress", SqlDbType.NVarChar))
                    .Value = s.Address;
                insertCmd.Parameters
                    .Add(new SqlParameter("@TypeOfDevice", SqlDbType.NVarChar))
                    .Value = s.DeviceType;
                cn.Open();
                insertCmd.ExecuteNonQuery();
                cn.Close();
            }
        }
        public List<WhoIsOnlineModel> getAddresses()
        {
            List<WhoIsOnlineModel> items = new List<WhoIsOnlineModel>();
            using (var cn = new SqlConnection(@"Data Source=(LocalDB)\v11.0;AttachDbFilename" +
             "=|DataDirectory|\\Database.mdf; Integrated Security=True"))
            {
                string _sql = @"SELECT * FROM [dbo].[MacAddresses]";
                var cmd = new SqlCommand(_sql, cn);
                cn.Open();
                var reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    try
                    {
                        while (reader.Read())
                        {
                            WhoIsOnlineModel item = new WhoIsOnlineModel();
                            item.Id = Convert.ToInt32(reader["Id"].ToString());
                            item.Owner = reader["DeviceOwner"].ToString();
                            item.Address = reader["MacAddress"].ToString();
                            item.DeviceType = reader["TypeOfDevice"].ToString();
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
        public void deleteAddressById(int id)
        {
            using (var cn = new SqlConnection(@"Data Source=(LocalDB)\v11.0;AttachDbFilename" +
             "=|DataDirectory|\\Database.mdf; Integrated Security=True"))
            {
                string _sql = @"DELETE FROM [dbo].[MacAddresses] WHERE [Id] = @i ";
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