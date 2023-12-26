using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Npgsql;


namespace Web_Project_WikiFandom_Server.Models
{
    public class MyDataContext 
    {
        private static NpgsqlConnection connection;

        private static string ConnectionString =>
            "Server=localhost; Port=5432; User Id=postgres; Password=postgres; Database=Wiki-fandom";

        public static bool Add<T>(T entity)
        {
            var type = entity?.GetType();
            var properties = type?.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                //.Where(prop => !(prop.Name.Equals("id", StringComparison.OrdinalIgnoreCase)))
                .ToList();

            var tableName = type?.Name.ToLower();

            var sb = new StringBuilder();
            var listOfArgs = new List<NpgsqlParameter>();

            sb.AppendFormat("insert into \"{0}\" (", tableName);

            foreach (var prop in properties)
            {
                sb.Append($"\"{prop.Name}\",");
            }

            sb.Length--;
            sb.Append(") values (");

            foreach (var prop in properties)
            {
                var paramName = $"@{prop.Name}";
                sb.Append($"{paramName},");
                listOfArgs.Add(new NpgsqlParameter(paramName, prop.GetValue(entity)));
            }

            sb.Length--;
            sb.Append(");");

            var command = new NpgsqlCommand(sb.ToString());
            command.Parameters.AddRange(listOfArgs.ToArray());

            return QueryToDatabase(command);
        }

        public static bool Update<T>(T entity)
        {
            var type = entity?.GetType();
            var tableName = type?.Name.ToLower();
            var id = type?.GetProperty("id");
            var props = type.GetProperties()
                .Where(x => !x.Name.Equals("id", StringComparison.OrdinalIgnoreCase))
                .ToList();

            var sqlExpression = $"SELECT * FROM \"{tableName}\" WHERE \"id\" = {id?.GetValue(entity)}";
            using (connection = new NpgsqlConnection(ConnectionString))
            {
                connection.Open();
                var adapter = new NpgsqlDataAdapter(sqlExpression, connection);
                var dataSet = new DataSet();
                adapter.Fill(dataSet);

                var entityFromDatabase = dataSet.Tables[0];
                var rowToUpdate = entityFromDatabase.Rows[0];

                foreach (var prop in props)
                {
                    var val = prop.GetValue(entity);
                    rowToUpdate[prop.Name] = val ?? DBNull.Value;
                }

                var commandBuilder = new NpgsqlCommandBuilder(adapter);
                adapter.UpdateCommand = commandBuilder.GetUpdateCommand();
                adapter.Update(dataSet);

                return true;
            }
        }

        public static bool Delete<T>(string id)
        {
            var type = typeof(T);
            var tableName = type.Name.ToLower();

            var sqlExpression = $"DELETE FROM \"{tableName}\" WHERE \"id\" = '{id}' ";
            using (connection = new NpgsqlConnection(ConnectionString))
            {
                connection.Open();
                var command = new NpgsqlCommand(sqlExpression, connection);
                var number = command.ExecuteNonQuery();
                Console.WriteLine("Delete {0} object by id: {1}", number, id);
                return true;
            }
        }

        public static List<T> Select<T>(T entity, string command ="") //
        {
            Console.WriteLine(command);
            var type = entity?.GetType();
            var tableName = type?.Name.ToLower();

            var sqlExpression = $"SELECT * FROM \"{tableName}\" {command}";
            using (connection = new NpgsqlConnection(ConnectionString))
            {
                connection.Open();
                var adapter = new NpgsqlDataAdapter(sqlExpression, connection);
                var dataSet = new DataSet();
                adapter.Fill(dataSet);
                var listOfTableItems = dataSet.Tables[0];
                var listOfEntities = new List<T>();

                foreach (DataRow row in listOfTableItems.Rows)
                {
                    var objOfEntity = Activator.CreateInstance<T>();
                    foreach (DataColumn column in listOfTableItems.Columns)
                    {
                        var prop = type.GetProperty(column.ColumnName);

                        if (prop != null && row[column] != DBNull.Value)
                        {
                            prop.SetValue(objOfEntity, row[column]);
                        }
                    }
                    listOfEntities.Add(objOfEntity);
                }

                return listOfEntities;
            }
        }

        public static T SelectById<T>(int id)
        {
            var tableName = typeof(T).Name.ToLower();
            var type = typeof(T);
            var sqlExpression = $"SELECT * FROM \"{tableName}\"" +
                                $"WHERE \"id\" = {id} ";
            using (connection = new NpgsqlConnection(ConnectionString))
            {
                connection.Open();
                var adapter = new NpgsqlDataAdapter(sqlExpression, connection);
                var dataSet = new DataSet();
                adapter.Fill(dataSet);
                var listOfArgs = dataSet.Tables[0];

                foreach (DataRow row in listOfArgs.Rows)
                {
                    var entity = Activator.CreateInstance<T>();
                    foreach (DataColumn column in listOfArgs.Columns)
                    {
                        var prop = type.GetProperty(column.ColumnName);
                        if (prop != null && row[column] != DBNull.Value)
                        {
                            prop.SetValue(entity, row[column]);
                        }
                    }

                    return entity;
                }
            }

            return default(T);
        }

        private static bool QueryToDatabase(NpgsqlCommand command)
        {
            using (connection = new NpgsqlConnection(ConnectionString))
            {
                connection.Open();
                command.Connection = connection;
                var number = command.ExecuteNonQuery();
                Console.WriteLine("Update {0} object", number);
                return true;
            }
        }


    }
}