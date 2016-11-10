using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using EfMigrationTool.Core;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace EfMigrationTool.View
{
    public class MainWindowViewmodel : ViewModelBase
    {
        private MigrationScanner _migrationScanner;
        private MigrationDecoder _migrationDecoder;
        private MigrationComparer _migrationComparer;
        private MigrationFileOperations _migrationFileOperations;

        private SettingsStore _settingsStore;

        public string MigrationAssembly { get; set; }
        public string DbConnectionString { get; set; }
        public List<MigrationInfo> AssemblyMigrations { get; set; }
        public List<MigrationInfo> DbMigrations { get; set; }
        public string CompareResult { get; set; }
        public string MigrationFilePath { get; set; }

        public MigrationInfo SelectedAssemblyMigration { get; set; }
        public MigrationInfo SelectedDbMigration { get; set; }

        public RelayCommand ReadMigrationsFromAssemblyCommand { get; set; }
        public RelayCommand ReadMigrationsFromDbCommand { get; set; }
        public RelayCommand DumpAssemblyMigrationCommand { get; set; }
        public RelayCommand DumpDbMigrationCommand { get; set; }
        public RelayCommand QuickCompareCommand { get; set; }
        public RelayCommand ExternCompareCommand { get; set; }
        public RelayCommand WriteAssemblyMigrationToDbCommand { get; set; }
        public RelayCommand WriteMigrationFromFileCommand { get; set; }

        public MainWindowViewmodel()
        {

            _migrationScanner = new MigrationScanner();
            _migrationDecoder = new MigrationDecoder();
            _migrationComparer = new MigrationComparer();
            _migrationFileOperations = new MigrationFileOperations();

            _settingsStore = new SettingsStore();
#if DEBUG
            DbConnectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=EfMigrationTool.DemoApp.DemoContext;Integrated Security=True";
            MigrationAssembly = "EfMigrationTool.DemoApp.exe";
            _settingsStore.WriteDefaultSettings();
#endif
            ReadMigrationsFromAssemblyCommand = new RelayCommand(ReadMigrationsFromAssembly);
            ReadMigrationsFromDbCommand = new RelayCommand(ReadMigrationsFromDb);

            DumpAssemblyMigrationCommand = new RelayCommand(() => DumpMigrationInfo(SelectedAssemblyMigration));
            DumpDbMigrationCommand = new RelayCommand(() => DumpMigrationInfo(SelectedDbMigration));
            QuickCompareCommand = new RelayCommand(QuickCompare);
            ExternCompareCommand = new RelayCommand(
                () =>
                ExternCompare(
                    _settingsStore.GetSettingValue(SettingKeyEnum.DiffToolPath),
                    _settingsStore.GetSettingValue(SettingKeyEnum.DiffToolPattern)));

            WriteAssemblyMigrationToDbCommand = new RelayCommand(WriteAssemblyMigrationToDb);
            WriteMigrationFromFileCommand = new RelayCommand(WriteMigrationFromFile);
        }

        private void WriteMigrationFromFile()
        {
            if (
                MigrationFilePath != null &&
                File.Exists(MigrationFilePath) &&
                SelectedDbMigration != null)
            {
                var edmxContent = File.ReadAllText(MigrationFilePath);
                var changedMigration = new MigrationInfo
                {
                    MigrationId = SelectedDbMigration.MigrationId,
                    Source = SelectedDbMigration.Source,
                    ModelBlobb = _migrationDecoder.GetBytesFromEdmxContent(edmxContent)
                };

                _migrationScanner.UpdateMigration(changedMigration, DbConnectionString);
            }
        }

        private void WriteAssemblyMigrationToDb()
        {
            if (
                SelectedAssemblyMigration != null &&
                SelectedDbMigration != null)
            {
                var changedMigration = new MigrationInfo
                {
                    MigrationId = SelectedDbMigration.MigrationId,
                    Source = SelectedDbMigration.Source,
                    ModelBlobb = SelectedAssemblyMigration.ModelBlobb
                };

                _migrationScanner.UpdateMigration(changedMigration, DbConnectionString);
            }
        }

        private void ExternCompare(string diffToolPath, string diffToolPattern)
        {
            if (SelectedAssemblyMigration != null &&
                SelectedDbMigration != null)
            {
                _migrationComparer.CompareMigrationsInExternTool(
                    SelectedAssemblyMigration,
                    SelectedDbMigration,
                    diffToolPath,
                    diffToolPattern);
            }
        }

        private void QuickCompare()
        {
            var compareResult = string.Empty;

            if (DbMigrations.Count > 0 && AssemblyMigrations.Count > 0)
            {
                compareResult = _migrationComparer.CompareMigrationSets(DbMigrations, AssemblyMigrations);
            }
            else
            {
                compareResult = "No migrations found." + Environment.NewLine;
            }

            if (string.IsNullOrEmpty(compareResult))
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
                var targetFile = _migrationFileOperations.WriteMigrationToFile(migration);
                MigrationFilePath = targetFile;
                RaisePropertyChanged("MigrationFilePath");
            }
        }

        private void ReadMigrationsFromAssembly()
        {
            AssemblyMigrations = _migrationScanner.ScanAssemblyForMigrations(MigrationAssembly).
                OrderBy(x => x.MigrationId).ToList();

            if (AssemblyMigrations.Count > 0)
            {
                SelectedAssemblyMigration = AssemblyMigrations.First();
            }

            RaisePropertyChanged("AssemblyMigrations");
            RaisePropertyChanged("SelectedAssemblyMigration");
        }

        private void ReadMigrationsFromDb()
        {
            DbMigrations = _migrationScanner.ScanDataBaseForMigrations(DbConnectionString).
                OrderBy(x => x.MigrationId).ToList();

            if (DbMigrations.Count > 0)
            {
                SelectedDbMigration = DbMigrations.First();
            }

            RaisePropertyChanged("DbMigrations");
            RaisePropertyChanged("SelectedDbMigration");
        }
    }
}