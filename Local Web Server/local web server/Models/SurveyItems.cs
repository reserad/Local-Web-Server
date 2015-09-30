using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Local_Web_Server.Models
{
    public class SurveyItems
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string Option1 { get; set; }
        public string Option2 { get; set; }
        public string Option3 { get; set; }
        public string Option4 { get; set; }
        public int Score1 { get; set; }
        public int Score2 { get; set; }
        public int Score3 { get; set; }
        public int Score4 { get; set; }
        public DateTime EndDate { get; set; }
        public string Creator { get; set; }


        public void Vote(string _username, int ID, string choice)
        {
            using (var cn = new SqlConnection(@"Data Source=(LocalDB)\v11.0;AttachDbFilename" +
             "=|DataDirectory|\\Database.mdf; Integrated Security=True"))
            {
                string _sql = @"SELECT [Username] FROM [dbo].[SurveyVotes] " +
                       @"WHERE [Username] = @u AND [Id] = @i";
                var cmd = new SqlCommand(_sql, cn);
                cmd.Parameters
                    .Add(new SqlParameter("@u", SqlDbType.NVarChar))
                    .Value = _username;
                cmd.Parameters
                    .Add(new SqlParameter("@i", SqlDbType.NVarChar))
                    .Value = ID;
                cn.Open();
                var reader = cmd.ExecuteReader();

                if (!reader.HasRows)
                {
                    reader.Dispose();
                    cmd.Dispose();

                    if (_username != null && choice != null)
                    {
                        string _insertSql = @"INSERT INTO [dbo].[SurveyVotes] 
                                            ([Username], [SurveyID])
                                            VALUES (@u, @i)";
                        var insertCmd = new SqlCommand(_insertSql, cn);
                        insertCmd.Parameters
                            .Add(new SqlParameter("@u", SqlDbType.NVarChar))
                            .Value = _username;
                        insertCmd.Parameters
                            .Add(new SqlParameter("@i", SqlDbType.NVarChar))
                            .Value = ID;
                        insertCmd.ExecuteNonQuery();
                        insertCmd.Dispose();

                        string scoreChoice = "Score1";
                        if (choice == "2") { scoreChoice = "Score2"; }
                        if (choice == "3") { scoreChoice = "Score3"; }
                        if (choice == "4") { scoreChoice = "Score4"; }

                        string _updateSql = @"UPDATE [dbo].[Surveys] 
                                            SET " + scoreChoice + " = " + scoreChoice + " + 1 WHERE Id = " + ID;
                        var updateCmd = new SqlCommand(_updateSql, cn);
                        updateCmd.Parameters
                            .Add(new SqlParameter("@u", SqlDbType.NVarChar))
                            .Value = _username;
                        updateCmd.Parameters
                            .Add(new SqlParameter("@i", SqlDbType.NVarChar))
                            .Value = ID;

                        updateCmd.ExecuteNonQuery();
                        updateCmd.Dispose();
                    }
                }
            }
        }
        public bool hasAlreadyVoted(string _username, int ID)
        {
            using (var cn = new SqlConnection(@"Data Source=(LocalDB)\v11.0;AttachDbFilename" +
            "=|DataDirectory|Database.mdf; Integrated Security=True"))
            {
                string _sql = @"SELECT [Username] FROM [dbo].[SurveyVotes] " +
                       @"WHERE [Username] = @u AND [SurveyID] = @i";
                var cmd = new SqlCommand(_sql, cn);
                cmd.Parameters
                    .Add(new SqlParameter("@u", SqlDbType.NVarChar))
                    .Value = _username;
                cmd.Parameters
                    .Add(new SqlParameter("@i", SqlDbType.NVarChar))
                    .Value = ID;
                cn.Open();
                var reader = cmd.ExecuteReader();

                if (!reader.HasRows)
                {
                    reader.Dispose();
                    cmd.Dispose();
                    return false;
                }

            }
            return true;
        }
        public void CreateSurvey(SurveyItems s) 
        {
            using (var cn = new SqlConnection(@"Data Source=(LocalDB)\v11.0;AttachDbFileName=|DataDirectory|Database.mdf;Integrated Security=True"))
            {
                string _insertSql = @"INSERT INTO [dbo].[Surveys] 
                                            ([Title], [Option 1], [Option 2], [Option 3], [Option 4], [Score1], [Score2], [Score3], [Score4], [EndDate], [Creator])
                                            VALUES (@t, @o1, @o2, @o3, @o4, @s1, @s2, @s3, @s4, @e, @ow)";
                var insertCmd = new SqlCommand(_insertSql, cn);
                insertCmd.Parameters
                    .Add(new SqlParameter("@t", SqlDbType.NVarChar))
                    .Value = s.Title;
                insertCmd.Parameters
                    .Add(new SqlParameter("@o1", SqlDbType.NVarChar))
                    .Value = s.Option1;
                insertCmd.Parameters
                    .Add(new SqlParameter("@o2", SqlDbType.NVarChar))
                    .Value = s.Option2;
                insertCmd.Parameters
                    .Add(new SqlParameter("@o3", SqlDbType.NVarChar))
                    .Value = s.Option3;
                insertCmd.Parameters
                    .Add(new SqlParameter("@o4", SqlDbType.NVarChar))
                    .Value = s.Option4;
                insertCmd.Parameters
                    .Add(new SqlParameter("@s1", SqlDbType.NVarChar))
                    .Value = 0;
                insertCmd.Parameters
                    .Add(new SqlParameter("@s2", SqlDbType.NVarChar))
                    .Value = 0;
                insertCmd.Parameters
                    .Add(new SqlParameter("@s3", SqlDbType.NVarChar))
                    .Value = 0;
                insertCmd.Parameters
                    .Add(new SqlParameter("@s4", SqlDbType.NVarChar))
                    .Value = 0;
                insertCmd.Parameters
                    .Add(new SqlParameter("@e", SqlDbType.NVarChar))
                    .Value = s.EndDate;
                insertCmd.Parameters
                    .Add(new SqlParameter("@ow", SqlDbType.NVarChar))
                    .Value = s.Creator;
                cn.Open();
                insertCmd.ExecuteNonQuery();
                cn.Close();
            }
        }
        public List<SurveyItems> getSurveys() 
        {
            List<SurveyItems> items = new List<SurveyItems>();
            using (var cn = new SqlConnection(@"Data Source=(LocalDB)\v11.0;AttachDbFileName=|DataDirectory|Database.mdf;Integrated Security=True"))
            {
                string _sql = @"SELECT * FROM [dbo].[Surveys]";
                var cmd = new SqlCommand(_sql, cn);
                cn.Open();
                var reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    
                    try 
                    { 
                        while(reader.Read())
                        {
                            SurveyItems item = new SurveyItems();
                            item.ID = Convert.ToInt32(reader["Id"].ToString());
                            item.Title = reader["Title"].ToString();
                            item.Option1 = reader["Option 1"].ToString();
                            item.Option2 = reader["Option 2"].ToString();
                            item.Option3 = reader["Option 3"].ToString();
                            item.Option4 = reader["Option 4"].ToString();
                            item.Score1 = Convert.ToInt32(reader["Score1"].ToString());
                            item.Score2 = Convert.ToInt32(reader["Score2"].ToString());
                            item.Score3 = Convert.ToInt32(reader["Score3"].ToString());
                            item.Score4 = Convert.ToInt32(reader["Score4"].ToString());
                            item.EndDate = Convert.ToDateTime(reader["EndDate"].ToString());
                            item.Creator = reader["Creator"].ToString();
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
        public SurveyItems getSurveysByID(int id)
        {
            SurveyItems item = new SurveyItems();
            using (var cn = new SqlConnection(@"Data Source=(LocalDB)\v11.0;AttachDbFileName=|DataDirectory|Database.mdf;Integrated Security=True"))
            {
                string _sql = @"SELECT * FROM [dbo].[Surveys] WHERE [Id] = @i ";
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
                        item.ID = Convert.ToInt32(reader["Id"].ToString());
                        item.Title = reader["Title"].ToString();
                        item.Option1 = reader["Option 1"].ToString();
                        item.Option2 = reader["Option 2"].ToString();
                        item.Option3 = reader["Option 3"].ToString();
                        item.Option4 = reader["Option 4"].ToString();
                        item.Score1 = Convert.ToInt32(reader["Score1"].ToString());
                        item.Score2 = Convert.ToInt32(reader["Score2"].ToString());
                        item.Score3 = Convert.ToInt32(reader["Score3"].ToString());
                        item.Score4 = Convert.ToInt32(reader["Score4"].ToString());
                        item.EndDate = Convert.ToDateTime(reader["EndDate"].ToString());
                    }
                }
                reader.Dispose();
                cmd.Dispose();
            }
            return item;
        }
        public void deleteSurveysByID(int id)
        {
            using (var cn = new SqlConnection(@"Data Source=(LocalDB)\v11.0;AttachDbFileName=|DataDirectory|Database.mdf;Integrated Security=True"))
            {
                string _sql = @"DELETE FROM [dbo].[Surveys] WHERE [Id] = @i ";
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