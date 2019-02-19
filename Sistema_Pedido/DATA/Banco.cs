using System;
using System.Data.SQLite;
using System.IO;
using System.Reflection;
using System.Text;

namespace DATA
{
    public class Banco
    {
        private static SQLiteConnection sqliteConnection;

        private static string strConection = System.Configuration.ConfigurationManager.
                    ConnectionStrings["DBConnect"].ConnectionString;

        public Banco()
        {
            CriarBanco();
        }

        private static void CriarBanco()
        {
            try
            {
                string pathRoot = @"C:\SistemaPedidos";
                string pathDB = pathRoot + @"\DBPedidos.sqlite";
                if (!Directory.Exists(pathRoot))
                    Directory.CreateDirectory(pathRoot);

                if (!File.Exists(pathDB))
                {
                    SQLiteConnection.CreateFile(pathDB);

                    string teste1 = Directory.GetCurrentDirectory();

                    //string teste02 = teste1.Remove(teste1.Length - 27, 27);
                    string teste02 = teste1.Substring(0, teste1.LastIndexOf("Sistema_Pedido"));// Remove(teste1.Length - 27, 27);
                    string teste03 = teste02 + @"DATA\Entities";
                    string[] files01 = Directory.GetFiles(teste03);
                    CriarTabelaSQlite(files01);
                }

                //var teste = strConection;
            }
            catch (Exception ex)
            {

            }
        }


        public static SQLiteConnection DbConnection()
        {
            sqliteConnection = new SQLiteConnection(strConection);
            string pathRoot = @"C:\DBMecanica";
            string pathDB = pathRoot + @"\Cadastro.sqlite";
            if (!File.Exists(pathDB))
            {
                CriarBanco();
            }
            if (sqliteConnection.State == System.Data.ConnectionState.Open)
                sqliteConnection.Close();

            sqliteConnection.Open();
            return sqliteConnection;
        }

        public static void CriarTabelaSQlite(string[] files)
        {
            try
            {
                Type EntityType = null;
                using (var cmd = DbConnection().CreateCommand())
                {
                    foreach (var fileClass in files)
                    {
                        var index = fileClass.LastIndexOf("\\");
                        var result = fileClass.Remove(0, index + 1);
                        var rresult02 = result.Replace(".cs", "");

                        EntityType = Type.GetType("DATA.Entities." + rresult02);

                        StringBuilder sb = new StringBuilder();
                        sb.Append($"CREATE TABLE IF NOT EXISTS { EntityType.Name } ( ");

                        PropertyInfo[] propertyInfos = EntityType.GetProperties();
                        foreach (var item in propertyInfos)
                        {
                            switch (item.PropertyType.Name)
                            {
                                case "String":
                                    sb.Append($"{item.Name} varchar(50),");
                                    break;

                                case "Decimal":
                                    sb.Append($"{item.Name} numeric ,");
                                    break;

                                case "Int32":
                                    if (item.Name.ToUpper() == "ID")
                                        sb.Append($"{item.Name} INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT ,");
                                    else
                                        sb.Append($"{item.Name} INTEGER ,");
                                    break;

                                case "DateTime":
                                    sb.Append($"{item.Name} Date ,");
                                    break;
                            }
                        }
                        sb.Remove(sb.Length - 1, 1);
                        sb.Append(" )");

                        cmd.CommandText = sb.ToString();
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}