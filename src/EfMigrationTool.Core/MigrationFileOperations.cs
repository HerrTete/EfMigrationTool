using System;
using System.IO;

namespace EfMigrationTool.Core
{
    public class MigrationFileOperations
    {
        private MigrationDecoder _migrationDecoder;

        public MigrationFileOperations()
        {
            _migrationDecoder = new MigrationDecoder();
        }
        public string WriteMigrationToFile(MigrationInfo migration, string path = null)
        {
            var edmxContent = _migrationDecoder.GetEdmxContentForMigration(migration.ModelBlobb);
            var filename = migration.Source.ToString() + "_" + migration.MigrationId + ".edmx";
            var targetpath = Path.Combine(path ?? Environment.CurrentDirectory, filename);
            File.WriteAllText(targetpath, edmxContent);
            return targetpath;
        }
    }
}
