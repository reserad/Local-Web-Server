using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Local_Web_Server.Models
{
    public class CalendarEvents
    {
        public int ID { get; set; }
        public string Assignment { get; set; }
        public string Class { get; set; }
        public DateTime Due_Date { get; set; }
        public string Creator { get; set; }
        public string SelectedRecurring { get; set; }

        public void CreateEvent(CalendarEvents s)
        {
            using (var cn = new SqlConnection(@"Data Source=(LocalDB)\v11.0;AttachDbFilename" +
             "=|DataDirectory|\\Database.mdf; Integrated Security=True"))
            {
                string _insertSql = @"INSERT INTO [dbo].[CalendarEvents] 
                                            ([Assignment], [Class], [Due_Date], [Creator], [Recurring])
                                            VALUES (@Assignment, @Class, @Due_Date, @Creator, @Recurring)";
                var insertCmd = new SqlCommand(_insertSql, cn);
                insertCmd.Parameters
                    .Add(new SqlParameter("@Assignment", SqlDbType.NVarChar))
                    .Value = s.Assignment;
                insertCmd.Parameters
                    .Add(new SqlParameter("@Class", SqlDbType.NVarChar))
                    .Value = s.Class;
                insertCmd.Parameters
                    .Add(new SqlParameter("@Due_Date", SqlDbType.DateTime))
                    .Value = s.Due_Date;
                insertCmd.Parameters
                    .Add(new SqlParameter("@Creator", SqlDbType.NVarChar))
                    .Value = s.Creator;
                insertCmd.Parameters
                    .Add(new SqlParameter("@Recurring", SqlDbType.NVarChar))
                    .Value = s.SelectedRecurring;
                cn.Open();
                insertCmd.ExecuteNonQuery();
                cn.Close();
            }
        }
        public List<CalendarEvents> getEvents()
        {
            List<CalendarEvents> items = new List<CalendarEvents>();
            using (var cn = new SqlConnection(@"Data Source=(LocalDB)\v11.0;AttachDbFilename" +
             "=|DataDirectory|\\Database.mdf; Integrated Security=True"))
            {
                string _sql = @"SELECT * FROM [dbo].[CalendarEvents]";
                var cmd = new SqlCommand(_sql, cn);
                cn.Open();
                var reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    try
                    {
                        while (reader.Read())
                        {
                            CalendarEvents item = new CalendarEvents();
                            item.ID = Convert.ToInt32(reader["ID"].ToString());
                            item.Assignment = reader["Assignment"].ToString();
                            item.Class = reader["Class"].ToString();
                            item.Due_Date = Convert.ToDateTime(reader["Due_Date"].ToString());
                            item.Creator = reader["Creator"].ToString();
                            item.SelectedRecurring = reader["Recurring"].ToString();
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
        public CalendarEvents getEventByID(int id)
        {
            CalendarEvents item = new CalendarEvents();
            using (var cn = new SqlConnection(@"Data Source=(LocalDB)\v11.0;AttachDbFilename" +
             "=|DataDirectory|\\Database.mdf; Integrated Security=True"))
            {
                string _sql = @"SELECT * FROM [dbo].[CalendarEvents] WHERE [Id] = @i ";
                var cmd = new SqlCommand(_sql, cn);
                cmd.Parameters
                    .Add(new SqlParameter("@i", SqlDbType.NVarChar))
                    .Value = id;
                cn.Open();

                var reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        item.ID = Convert.ToInt32(reader["ID"].ToString());
                        item.Assignment = reader["Assignment"].ToString();
                        item.Class = reader["Class"].ToString();
                        item.Due_Date = Convert.ToDateTime(reader["Due_Date"].ToString());
                        item.Creator = reader["Creator"].ToString();
                        item.SelectedRecurring = reader["Recurring"].ToString();
                    }
                }
                reader.Dispose();
                cmd.Dispose();
            }
            return item;
        }
        public void deleteEventByID(int id)
        {
            using (var cn = new SqlConnection(@"Data Source=(LocalDB)\v11.0;AttachDbFilename" +
             "=|DataDirectory|\\Database.mdf; Integrated Security=True"))
            {
                string _sql = @"DELETE FROM [dbo].[CalendarEvents] WHERE [Id] = @i ";
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
        public void UpdateEvent(CalendarEvents s)
        {
            using (var cn = new SqlConnection(@"Data Source=(LocalDB)\v11.0;AttachDbFilename" +
             "=|DataDirectory|\\Database.mdf; Integrated Security=True"))
            {
                string _insertSql = @"UPDATE [dbo].[CalendarEvents] 
                                    SET Assignment = @Assignment, Class = @Class, Due_Date = @Due_Date, Recurring = @Recurring WHERE Id = @Id";
                var insertCmd = new SqlCommand(_insertSql, cn);
                insertCmd.Parameters
                    .Add(new SqlParameter("@Assignment", SqlDbType.NVarChar))
                    .Value = s.Assignment;
                insertCmd.Parameters
                    .Add(new SqlParameter("@Class", SqlDbType.NVarChar))
                    .Value = s.Class;
                insertCmd.Parameters
                    .Add(new SqlParameter("@Due_Date", SqlDbType.DateTime))
                    .Value = s.Due_Date;
                insertCmd.Parameters
                    .Add(new SqlParameter("@Id", SqlDbType.Int))
                    .Value = s.ID;
                insertCmd.Parameters
                    .Add(new SqlParameter("@Recurring", SqlDbType.NVarChar))
                    .Value = s.SelectedRecurring;
                cn.Open();
                insertCmd.ExecuteNonQuery();
                cn.Close();
            }
        }
        public void UpdateDueDate(DateTime Due_Date, int ID)
        {
            using (var cn = new SqlConnection(@"Data Source=(LocalDB)\v11.0;AttachDbFilename" +
             "=|DataDirectory|\\Database.mdf; Integrated Security=True"))
            {
                string _insertSql = @"UPDATE [dbo].[CalendarEvents] 
                                    SET Due_Date = @Due_Date WHERE Id = @Id";
                var insertCmd = new SqlCommand(_insertSql, cn);
                insertCmd.Parameters
                    .Add(new SqlParameter("@Due_Date", SqlDbType.DateTime))
                    .Value = Due_Date;
                insertCmd.Parameters
                    .Add(new SqlParameter("@Id", SqlDbType.Int))
                    .Value = ID;
                cn.Open();
                insertCmd.ExecuteNonQuery();
                cn.Close();
            }
        }
    }
}