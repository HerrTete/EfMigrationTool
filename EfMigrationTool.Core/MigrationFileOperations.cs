using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfMigrationTool.Core
{
    public class MigrationFileOperations
    {
        private MigrationDecoder _migrationDecoder;

        public MigrationFileOperations()
        {
            _migrationDecoder = new MigrationDecoder();
        }
        public void WriteMigrationToFile(MigrationInfo migration)
        {
            var edmxContent = _migrationDecoder.GetEdmxContentForMigration(migration.ModelBlobb);
            File.WriteAllText(migration.Source.ToString() + "_" + migration.MigrationId + ".edmx", edmxContent);
        }
    }
}
