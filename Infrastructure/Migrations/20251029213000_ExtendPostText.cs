using Infrastructure;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

// ReSharper disable once CheckNamespace
namespace Infrastructure.Migrations;

[DbContext(typeof(LeadsDbContext))]
[Migration("20251029213000_ExtendPostText")]
public partial class ExtendPostText : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<string>(
            name: "Text",
            table: "Posts",
            type: "text",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "character varying(8000)",
            oldMaxLength: 8000,
            oldNullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<string>(
            name: "Text",
            table: "Posts",
            type: "character varying(8000)",
            maxLength: 8000,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "text",
            oldNullable: true);
    }
}
