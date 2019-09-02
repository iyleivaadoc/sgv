namespace web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class accesoss : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Accesos",
                c => new
                    {
                        id_acceso = c.Int(nullable: false, identity: true),
                        Nombre = c.String(nullable: false, maxLength: 50),
                        Control = c.String(nullable: false, maxLength: 50),
                        Metodo = c.String(nullable: false, maxLength: 50),
                        Tipo = c.Boolean(nullable: false),
                        AccesoPredecesor = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.id_acceso);
            
            CreateTable(
                "dbo.Permisos",
                c => new
                    {
                        id_acceso = c.Int(nullable: false),
                        Id = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.id_acceso, t.Id })
                .ForeignKey("dbo.Accesos", t => t.id_acceso, cascadeDelete: false)
                .ForeignKey("dbo.AspNetRoles", t => t.Id, cascadeDelete: false)
                .Index(t => t.id_acceso)
                .Index(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Permisos", "Id", "dbo.AspNetRoles");
            DropForeignKey("dbo.Permisos", "id_acceso", "dbo.Accesos");
            DropIndex("dbo.Permisos", new[] { "Id" });
            DropIndex("dbo.Permisos", new[] { "id_acceso" });
            DropTable("dbo.Permisos");
            DropTable("dbo.Accesos");
        }
    }
}
