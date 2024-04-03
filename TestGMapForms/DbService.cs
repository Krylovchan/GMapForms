using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using Microsoft.Extensions.Configuration;

namespace TestGMapForms
{
    /// <summary>
    /// Соединение и обработка даныых с сервера.
    /// </summary>
    class DbService
    {
        private readonly string connection;
        public DbService()
        {
            var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).Build();
            connection = configuration.GetSection("ConnectionStrings").GetValue<string>("DbService");
        }

        /// <summary>
        /// Запрос данных из БД
        /// </summary>
        /// <returns></returns>
        public List<Unit> GetAll()
        {
            var units = new List<Unit>();
            using (var conn = new SqlConnection(connection))
            {
                try
                {
                    conn.Open();
                    var command = new SqlCommand("SELECT Id, PositionX, PositionY FROM unit", conn);
                    using (var reader = command.ExecuteReader())
                    {

                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                var id = reader.GetInt32("Id");
                                var positionx = reader.GetDouble("PositionX");
                                var positiony = reader.GetDouble("PositionY");
                                units.Add(new Unit { Id = id, PositionX = positionx, PositionY = positiony });
                            }
                        }
                        else
                        {
                            MessageBox.Show("Не было найдено данных в базе данных.", "Внимание!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show("Ошибка подключения к БД", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }     
            }
            return units;
        }

        /// <summary>
        /// Запрос на изменение данных в БД
        /// </summary>
        /// <param name="id"></param>
        /// <param name="positionx"></param>
        /// <param name="positiony"></param>
        public void ChangePosition(int id, double positionx, double positiony)
        {
            using (var conn = new SqlConnection(connection))
            {
                try
                {
                    conn.Open();
                    var query = "UPDATE unit SET PositionX = @positionx, Positiony = @positiony WHERE Id = @id";
                    using (var command = new SqlCommand(query, conn))
                    {
                        command.Parameters.Add("@positionx", SqlDbType.Float).Value = positionx;
                        command.Parameters.Add("@positiony", SqlDbType.Float).Value = positiony;
                        command.Parameters.Add("@id", SqlDbType.Int).Value = id;
                        var rowsNum = command.ExecuteNonQuery();
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show("Ошибка подключения к БД", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        public void NewPoint(double positionx, double positiony)
        {
            using (var conn = new SqlConnection(connection))
            {
                try
                {
                    conn.Open();
                    var query = "INSERT INTO [unit] ([PositionX], [PositionY]) Values (@positionx, @positiony)";
                    using (var command = new SqlCommand(query, conn))
                    {
                        command.Parameters.Add("@positionx", SqlDbType.Float).Value = positionx;
                        command.Parameters.Add("@positiony", SqlDbType.Float).Value = positiony;
                        var rowsNum = command.ExecuteNonQuery();
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show("Ошибка подключения к БД", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        public void DeleteUnit(int id)
        {
            try
            {
                using (var conn = new SqlConnection(connection))
                {
                    conn.Open();

                    var command = new SqlCommand("DELETE FROM unit WHERE Id = @id", conn);
                    command.Parameters.AddWithValue("@id", id);

                    var rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Данные успешно удалены.", "Успех!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Не удалось найти и удалить данные.", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                 }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка подключения к БД: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
