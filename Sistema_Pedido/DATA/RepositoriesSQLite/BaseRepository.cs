using DATA.Entities;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Text;

namespace DATA.RepositoriesSQLite
{
    public class BaseRepository<T> where T : BaseEntity
    {
        public static void Save(T Entity)
        {
            using (var conection = Banco.DbConnection())
            {
                StringBuilder sb = new StringBuilder();
                sb.Append($"insert into { Entity.GetType().Name } ( ");
                var properties = Entity.GetType().GetProperties();

                object instanceEntity = Activator.CreateInstance(Entity.GetType());

                foreach (var item in properties)
                {
                    if (item.Name.ToUpper() != "ID")
                        sb.Append($"{item.Name},");
                }

                sb.Remove(sb.Length - 1, 1);
                sb.Append(" ) values ( ");

                foreach (var item in properties)
                {
                    switch (item.PropertyType.Name)
                    {
                        case "String":
                            sb.Append($"'{item.GetValue(Entity, null)}',");
                            break;

                        case "Decimal":
                            sb.Append($"{item.GetValue(Entity, null)},");
                            break;

                        case "Int32":
                            if (item.Name.ToUpper() != "ID")
                                sb.Append($"{item.GetValue(Entity, null)},");
                            break;

                        case "DateTime":
                            sb.Append($"'{((DateTime)item.GetValue(Entity, null)).ToString("yyyy-MM-dd")}',");
                            break;
                    }
                }

                sb.Remove(sb.Length - 1, 1);
                sb.Append(" )");

                using (var command = conection.CreateCommand())
                {
                    command.CommandText = sb.ToString();
                    command.ExecuteNonQuery();
                }
            }
        }

        public void Delete(T Entity)
        {
            using (var conection = Banco.DbConnection())
            {
                object instanceEntity = Activator.CreateInstance(Entity.GetType());
                var properties = Entity.GetType().GetProperty("Id");

                string sql = $" DELETE FROM {Entity.GetType().Name} where Id = {properties.GetValue(instanceEntity, null)}";

                using (var command = conection.CreateCommand())
                {
                    command.CommandText = sql;
                    command.ExecuteNonQuery();
                }
            }
        }

        public static IList<T> GetAll(Type Entity)
        {
            using (var conection = Banco.DbConnection())
            {
                SQLiteCommand sQLiteCommand = new SQLiteCommand($"SELECT * FROM {Entity.Name} ", conection);

                using (var read = sQLiteCommand.ExecuteReader())
                {
                    while (read.Read())
                    {

                    }


                    if (read.Read())
                    {
                        DateTime valor = Convert.ToDateTime(read["DataNascimento"].ToString());
                        DateTime dataCadastro = Convert.ToDateTime(read["DataCadastro"].ToString());
                    }
                }
            }
            return null;
        }

        public static void Update(T Entity)
        {
            using (var conection = Banco.DbConnection())
            {
                object instanceEntity = Activator.CreateInstance(Entity.GetType());
                StringBuilder sb = new StringBuilder();
                sb.Append($"UPDATE { Entity.GetType().Name } SET  ");
                var properties = Entity.GetType().GetProperties();

                foreach (var item in properties)
                {
                    switch (item.PropertyType.Name)
                    {
                        case "String":
                            sb.Append($"{item.Name} = '{item.GetValue(instanceEntity, null)}',");
                            break;

                        case "Decimal":
                            sb.Append($" {item.Name} = {item.GetValue(instanceEntity, null)},");
                            break;

                        case "Int32":
                            if (item.Name.ToUpper() != "ID")
                                sb.Append($" {item.Name} = {item.GetValue(instanceEntity, null)},");
                            break;

                        case "DateTime":
                            sb.Append($" {item.Name} = '{((DateTime)item.GetValue(instanceEntity, null)).ToString("yyyy-MM-dd")}',");
                            break;
                    }
                }

                sb.Remove(sb.Length - 1, 1);
                sb.Append(" )");

                using (var command = conection.CreateCommand())
                {
                    command.CommandText = sb.ToString();
                    command.ExecuteNonQuery();
                }
            }
        }


        //public T GetEntity(Type typeEntity, int id)
        //{
        //    return "";
        //    //object instanceEntity = Activator.CreateInstance(T.GetType());
        //    //return null;
        //}

    }
}