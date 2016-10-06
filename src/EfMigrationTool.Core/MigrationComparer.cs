using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace EfMigrationTool.Core
{
    public class MigrationComparer
    {
        private MigrationDecoder _migrationDecoder;
        private MigrationFileOperations _migrationFileOperations;

        public MigrationComparer()
        {
            _migrationDecoder = new MigrationDecoder();
            _migrationFileOperations = new MigrationFileOperations();
        }

        public string CompareMigration(MigrationInfo migration1, MigrationInfo migration2)
        {
            var compareResult = string.Empty;

            var migrationEdmx1 = _migrationDecoder.GetEdmxContentForMigration(migration1.ModelBlobb);
            var migrationEdmx2 = _migrationDecoder.GetEdmxContentForMigration(migration2.ModelBlobb);
            if (migrationEdmx1 != migrationEdmx2)
            {
                compareResult += "Found difference in " + migration1.MigrationId + "." + Environment.NewLine;
            }

            return compareResult;
        }

        public void CompareMigrationsInExternTool(MigrationInfo migration1, MigrationInfo migration2, string externtoolPath, string toolStartPattern)
        {
            var file1 = _migrationFileOperations.WriteMigrationToFile(migration1);
            var file2 = _migrationFileOperations.WriteMigrationToFile(migration2);

            var command = toolStartPattern;
            command = command.Replace("{file1}", "\"" + file1 + "\"");
            command = command.Replace("{file2}", "\"" + file2 + "\"");

            Process.Start(externtoolPath, command);
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
