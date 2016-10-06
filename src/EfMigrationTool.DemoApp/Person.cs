using System;

namespace EfMigrationTool.DemoApp
{
    public class Person
    {
        public Guid Id { get; set; }

        public string Firstname { get; set; }

        public string Lastname { get; set; }

        public int Age { get; set; }
    }
}