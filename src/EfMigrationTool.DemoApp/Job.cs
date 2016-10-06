using System;

namespace EfMigrationTool.DemoApp
{
    public class Job
    {
        public Guid Id { get; set; }

        public string JobTitle { get; set; }

        public int Salary { get; set; }
    }
}