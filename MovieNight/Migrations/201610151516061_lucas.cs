namespace MovieNight.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class lucas : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Directors",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        FirstName = c.String(nullable: false),
                        LastName = c.String(nullable: false),
                        Gender = c.Int(nullable: false),
                        DateOfBirth = c.DateTime(nullable: false, storeType: "date"),
                        Origin = c.String(nullable: false),
                        Picture = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Movies",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false),
                        ReleaseDate = c.DateTime(nullable: false, storeType: "date"),
                        Genre = c.String(nullable: false),
                        Plot = c.String(nullable: false),
                        DirectorID = c.Int(nullable: false),
                        Rating = c.Double(nullable: false),
                        Poster = c.String(),
                        Trailer = c.String(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Directors", t => t.DirectorID, cascadeDelete: true)
                .Index(t => t.DirectorID);
            
            CreateTable(
                "dbo.Reviews",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        CriticName = c.String(nullable: false),
                        Date = c.DateTime(nullable: false),
                        MovieID = c.Int(nullable: false),
                        Content = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Movies", t => t.MovieID, cascadeDelete: true)
                .Index(t => t.MovieID);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Username = c.String(nullable: false, maxLength: 100),
                        Password = c.String(nullable: false),
                        Role = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .Index(t => t.Username, unique: true, name: "IX_UserName");
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Reviews", "MovieID", "dbo.Movies");
            DropForeignKey("dbo.Movies", "DirectorID", "dbo.Directors");
            DropIndex("dbo.Users", "IX_UserName");
            DropIndex("dbo.Reviews", new[] { "MovieID" });
            DropIndex("dbo.Movies", new[] { "DirectorID" });
            DropTable("dbo.Users");
            DropTable("dbo.Reviews");
            DropTable("dbo.Movies");
            DropTable("dbo.Directors");
        }
    }
}
