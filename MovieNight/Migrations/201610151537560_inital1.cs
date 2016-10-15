namespace MovieNight.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class inital1 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Users", "FirstName");
            DropColumn("dbo.Users", "LastName");
            DropColumn("dbo.Users", "Gender");
            DropColumn("dbo.Users", "DateOfBirth");
            DropColumn("dbo.Users", "Origin");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Users", "Origin", c => c.String());
            AddColumn("dbo.Users", "DateOfBirth", c => c.DateTime(nullable: false, storeType: "date"));
            AddColumn("dbo.Users", "Gender", c => c.Int(nullable: false));
            AddColumn("dbo.Users", "LastName", c => c.String());
            AddColumn("dbo.Users", "FirstName", c => c.String());
        }
    }
}
