using System.Data.Entity;

namespace EfMigrationTool.DemoApp
{
    public class DemoContext: DbContext
    {
        public DbSet<Person> Persons { get; set; }
        public DbSet<Job> Jobs { get; set; }
    }
}
