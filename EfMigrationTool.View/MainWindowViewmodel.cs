using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations.Infrastructure;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EfMigrationTool.Core;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace EfMigrationTool.View
{
    public class MainWindowViewmodel : ViewModelBase
    {
        private MigrationScanner _migrationScanner;
        private MigrationDecoder _migrationDecoder;

        public string MigrationAssembly { get; set; }
        public string DbConnectionString { get; set; }
        public List<MigrationInfo> AssemblyMigrations { get; set; }
        public List<MigrationInfo> DbMigrations { get; set; }
        public string CompareResult { get; set; }

        public MigrationInfo SelectedAssemblyMigration { get; set; }
        public MigrationInfo SelectedDbMigration { get; set; }

        public RelayCommand ReadMigrationsFromAssemblyCommand { get; set; }
        public RelayCommand ReadMigrationsFromDbCommand { get; set; }
        public RelayCommand DumpAssemblyMigrationCommand { get; set; }
        public RelayCommand DumpDbMigrationCommand { get; set; }

        public RelayCommand QuickCompareCommand { get; set; }

        public MainWindowViewmodel()
        {

            _migrationScanner = new MigrationScanner();
            _migrationDecoder = new MigrationDecoder();

            DbConnectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=EfMigrationTool.DemoApp.DemoContext;Integrated Security=True";
            MigrationAssembly = "EfMigrationTool.DemoApp.exe";

            ReadMigrationsFromAssemblyCommand = new RelayCommand(ReadMigrationsFromAssembly);
            ReadMigrationsFromDbCommand = new RelayCommand(ReadMigrationsFromDb);

            DumpAssemblyMigrationCommand = new RelayCommand(()=>DumpMigrationInfo(SelectedAssemblyMigration));
            DumpDbMigrationCommand = new RelayCommand(() => DumpMigrationInfo(SelectedDbMigration));
            QuickCompareCommand = new RelayCommand(QuickCompare);
        }

        private void QuickCompare()
        {
            var compareResult = string.Empty;

            if(DbMigrations.Count > 0 && AssemblyMigrations.Count > 0)
            {
                var source = DbMigrations;
                var target = AssemblyMigrations;

                foreach(var sourceMigration in source)
                {
                    var targetMigration = target.FirstOrDefault(m => m.MigrationId == sourceMigration.MigrationId);
                    if(targetMigration != null)
                    {
                        var sourceEdmx = _migrationDecoder.GetEdmxContentForMigration(sourceMigration.ModelBlobb);
                        var targetEdmx = _migrationDecoder.GetEdmxContentForMigration(targetMigration.ModelBlobb);
                        if(sourceEdmx != targetEdmx)
                        {
                            compareResult += "Found difference in " + sourceMigration.MigrationId + "." + Environment.NewLine;
                        }
                    }
                    else
                    {
                        compareResult += "Didn't found matching pair for " + sourceMigration.MigrationId + "." + Environment.NewLine;
                    }
                }
            }

            if(string.IsNullOrEmpty(compareResult))
            {
                compareResult = "No difference found.";
            }
            CompareResult = compareResult;
            RaisePropertyChanged("CompareResult");
        }

        private void DumpMigrationInfo(MigrationInfo migration)
        {
            if (migration != null)
            {
                var edmxContent = _migrationDecoder.GetEdmxContentForMigration(migration.ModelBlobb);
                File.WriteAllText(migration.Source.ToString() + "_" + migration.MigrationId + ".edmx", edmxContent);
            }
        }

        private void ReadMigrationsFromAssembly()
        {
            AssemblyMigrations = _migrationScanner.ScanAssemblyForMigrations(MigrationAssembly);

            if(AssemblyMigrations.Count > 0)
            {
                SelectedAssemblyMigration = AssemblyMigrations.First();
            }

            RaisePropertyChanged("AssemblyMigrations");
            RaisePropertyChanged("SelectedAssemblyMigration");
        }

        private void ReadMigrationsFromDb()
        {
            DbMigrations = _migrationScanner.ScanDataBaseForMigrations(DbConnectionString);

            if (DbMigrations.Count > 0)
            {
                SelectedDbMigration = DbMigrations.First();
            }

            RaisePropertyChanged("DbMigrations");
            RaisePropertyChanged("SelectedDbMigration");
        }
    }
}