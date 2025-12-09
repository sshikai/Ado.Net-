using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Windows.Forms; // для MessageBox

namespace AdoSqlite.DAL
{
    internal class SqliteHelper
    {
        internal static List<User> GetUsers()
        {
            List<User> users = new List<User>();

            try
            {
                string dbPath = "db.sqlite";

                using (var connection = new SQLiteConnection($@"Data Source={dbPath};Version=3;"))
                {
                    connection.Open();

                    // Вариант А: Получаем date_created как строку
                    string query = "SELECT id, user_name, name, CAST(date_created AS TEXT) as date_str FROM users";

                    using (var cmd = new SQLiteCommand(query, connection))
                    using (var rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            var user = new User
                            {
                                Id = Convert.ToInt32(rdr["id"]),
                                UserName = rdr["user_name"].ToString(),
                                Name = rdr["name"].ToString()
                            };

                            // Пытаемся распарсить дату
                            string dateStr = rdr["date_str"].ToString();
                            if (DateTime.TryParse(dateStr, out DateTime dateValue))
                            {
                                user.Date = dateValue;
                            }
                            else
                            {
                                user.Date = DateTime.Now; // или другое значение по умолчанию
                            }

                            users.Add(user);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }

            return users;
        }
    }
}
