using System;
using System.Data.Entity.Migrations.Infrastructure;
using System.IO;
using System.Text;
using EfMigrationTool.Core;
using EfMigrationTool.DemoApp.Migrations;

namespace EfMigrationTool.ConsoleHost
{
    class Program
    {
        static void Main(string[] args)
        {
            var migrationScanner = new MigrationScanner();
            var migrations = migrationScanner.ScanAssemblyForMigrationsTypes("EfMigrationTool.DemoApp.exe");
            var migrationDecoder = new MigrationDecoder();
            foreach (var migrationType in migrations)
            {
                var migration = Activator.CreateInstance(migrationType) as IMigrationMetadata;
                Console.WriteLine(migration.Id);
                var edmxContent = migrationDecoder.GetEdmxContentForMigration(migration);
                Console.WriteLine(edmxContent);

                File.WriteAllText("Assembly_" + migration.Id + ".edmx", edmxContent);
            }

            var connectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=EfMigrationTool.DemoApp.DemoContext;Integrated Security=True";

            var dbMigrations = migrationScanner.ScanDataBaseForMigrations(connectionString);

            foreach (var dbMigration in dbMigrations)
            {
                Console.WriteLine(dbMigration.MigrationId);
                var edmxContent = migrationDecoder.GetEdmxContentForMigration(dbMigration.ModelBlobb);
                Console.WriteLine(edmxContent);

                File.WriteAllText("DB_" + dbMigration.MigrationId + ".edmx", edmxContent);
            }
        }
    }
}
