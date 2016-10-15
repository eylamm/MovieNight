namespace MovieNight.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ititaial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Username = c.String(nullable: false, maxLength: 100),
                        Password = c.String(nullable: false),
                        FirstName = c.String(),
                        LastName = c.String(),
                        Gender = c.Int(nullable: false),
                        DateOfBirth = c.DateTime(nullable: false, storeType: "date"),
                        Origin = c.String(),
                        Role = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .Index(t => t.Username, unique: true, name: "IX_UserName");
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.Users", "IX_UserName");
            DropTable("dbo.Users");
        }
    }
}
