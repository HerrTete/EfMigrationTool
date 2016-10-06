namespace EfMigrationTool.DemoApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Newproperties : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Jobs", "Salary", c => c.Int(nullable: false));
            AddColumn("dbo.People", "Age", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.People", "Age");
            DropColumn("dbo.Jobs", "Salary");
        }
    }
}
