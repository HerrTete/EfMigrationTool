using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Migrations.Infrastructure;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EfMigrationTool.Core
{
    public class MigrationScanner
    {
        public List<Type> ScanAssemblyForMigrationsTypes(string filename)
        {
            List<Type> migrationTypes = null;

            if (File.Exists(filename))
            {
                migrationTypes = new List<Type>();

                var fullpath = Path.GetFullPath(filename);
                var assembly = Assembly.LoadFile(fullpath);
                foreach (var type in assembly.GetTypes())
                {
                    if (type.GetInterfaces().ToList().Contains(typeof(IMigrationMetadata)))
                    {
                        migrationTypes.Add(type);
                    }
                }
            }

            return migrationTypes;
        }
        public List<MigrationInfo> ScanAssemblyForMigrations(string filename)
        {
            var migrations = new List<MigrationInfo>();

            var types = ScanAssemblyForMigrationsTypes(filename);

            foreach (var type in types)
            {
                var instance = Activator.CreateInstance(type) as IMigrationMetadata;
                var compressedBytes = Convert.FromBase64String(instance.Target);
                migrations.Add(new MigrationInfo { MigrationId = instance.Id, ModelBlobb = compressedBytes, Source = MigrationSource.Assembly});
            }

            return migrations;
        }

        public List<MigrationInfo> ScanDataBaseForMigrations(string connectionString)
        {
            List<MigrationInfo> dbMigrations = null;

            var sqlQuery = "select * from __MigrationHistory";
            var sqlConnection = new SqlConnection(connectionString);
            var sqlCommand = new SqlCommand(sqlQuery, sqlConnection);
            sqlConnection.Open();
            var reader = sqlCommand.ExecuteReader();
            if (reader.HasRows)
            {
                dbMigrations = new List<MigrationInfo>();

                while (reader.Read())
                {
                    var migrationId = reader.GetString(0);
                    var model = (byte[])reader[2];
                    dbMigrations.Add(new MigrationInfo { MigrationId = migrationId, ModelBlobb = model, Source = MigrationSource.Db });
                }
            }

            return dbMigrations;
        }

        public void UpdateMigration(MigrationInfo migrationInfo, string connectionString)
        {
            var sqlQuery = "Update __MigrationHistory set Model = @modelBinary where MigrationId = @migrationId";
            var sqlConnection = new SqlConnection(connectionString);
            var sqlCommand = new SqlCommand(sqlQuery, sqlConnection);
            sqlCommand.Parameters.Add("@modelBinary", SqlDbType.VarBinary, migrationInfo.ModelBlobb.Length).Value = migrationInfo.ModelBlobb;
            sqlCommand.Parameters.Add("@migrationId", SqlDbType.NVarChar).Value = migrationInfo.MigrationId;
            sqlConnection.Open();
            var affectedRows = sqlCommand.ExecuteNonQuery();
        }
    }
}
