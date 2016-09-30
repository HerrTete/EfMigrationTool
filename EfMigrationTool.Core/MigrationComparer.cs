using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfMigrationTool.Core
{
    public class MigrationComparer
    {
        private MigrationDecoder _migrationDecoder;

        public MigrationComparer()
        {
            _migrationDecoder = new MigrationDecoder();
        }

        public string CompareMigration(MigrationInfo migration1, MigrationInfo mirgation2)
        {
            var compareResult = string.Empty;

            var sourceEdmx = _migrationDecoder.GetEdmxContentForMigration(migration1.ModelBlobb);
            var targetEdmx = _migrationDecoder.GetEdmxContentForMigration(mirgation2.ModelBlobb);
            if (sourceEdmx != targetEdmx)
            {
                compareResult += "Found difference in " + migration1.MigrationId + "." + Environment.NewLine;
            }

            return compareResult;
        }

        public string CompareMigrationSets(List<MigrationInfo> source, List<MigrationInfo> target)
        {
            var compareResult = string.Empty;

            if(source.Count == target.Count)
            {
                foreach (var sourceMigration in source)
                {
                    var targetMigration = target.FirstOrDefault(m => m.MigrationId == sourceMigration.MigrationId);
                    if (targetMigration != null)
                    {
                        compareResult += CompareMigration(sourceMigration, targetMigration);
                    }
                    else
                    {
                        compareResult += "Didn't found matching pair for " + sourceMigration.MigrationId + "." + Environment.NewLine;
                    }
                }
            }
            else
            {
                compareResult += "MigrationsCount is different." + Environment.NewLine;
            }

            return compareResult;
        }
    }
}
