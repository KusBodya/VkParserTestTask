using System;
using Microsoft.EntityFrameworkCore.Migrations;

// ReSharper disable once CheckNamespace
namespace Infrastructure.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Authors
            migrationBuilder.CreateTable(
                name: "Authors",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    VkOwnerId = table.Column<long>(type: "bigint", nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ScreenName = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table => { table.PrimaryKey("PK_Authors", x => x.Id); }
            );

            migrationBuilder.CreateIndex(
                name: "IX_Authors_VkOwnerId",
                table: "Authors",
                column: "VkOwnerId",
                unique: true);

            // Posts
            migrationBuilder.CreateTable(
                name: "Posts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OwnerId = table.Column<long>(type: "bigint", nullable: false),
                    PostId = table.Column<long>(type: "bigint", nullable: false),
                    PostedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Text = table.Column<string>(type: "character varying(8000)", maxLength: 8000, nullable: true),
                    City = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    AuthorId = table.Column<Guid>(type: "uuid", nullable: false),
                    IngestionTag = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    RawJson = table.Column<string>(type: "jsonb", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Posts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Posts_Authors_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "Authors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                }
            );

            migrationBuilder.CreateIndex(
                name: "IX_Posts_OwnerId_PostId",
                table: "Posts",
                columns: new[] { "OwnerId", "PostId" },
                unique: true);

            // Analyses
            migrationBuilder.CreateTable(
                name: "Analyses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PostIdFk = table.Column<Guid>(type: "uuid", nullable: false),
                    IsRelevant = table.Column<bool>(type: "boolean", nullable: false),
                    RelevanceScore = table.Column<float>(type: "real", nullable: true),
                    Intent = table.Column<int>(type: "integer", nullable: false),
                    PropertyType = table.Column<int>(type: "integer", nullable: false),
                    PhonesRaw = table.Column<string>(type: "jsonb", nullable: false),
                    PhonesE164 = table.Column<string>(type: "jsonb", nullable: false),
                    ModelTrace = table.Column<string>(type: "text", nullable: true),
                    ModelName = table.Column<string>(type: "text", nullable: true),
                    Provider = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Analyses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Analyses_Posts_PostIdFk",
                        column: x => x.PostIdFk,
                        principalTable: "Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                }
            );

            migrationBuilder.CreateIndex(
                name: "IX_Analyses_PostIdFk",
                table: "Analyses",
                column: "PostIdFk");

            // Leads
            migrationBuilder.CreateTable(
                name: "Leads",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Source = table.Column<int>(type: "integer", nullable: false),
                    PostIdFk = table.Column<Guid>(type: "uuid", nullable: false),
                    AuthorId = table.Column<Guid>(type: "uuid", nullable: false),
                    PrimaryPhoneE164 = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    AllPhonesE164 = table.Column<string>(type: "jsonb", nullable: false),
                    Intent = table.Column<int>(type: "integer", nullable: false),
                    PropertyType = table.Column<int>(type: "integer", nullable: false),
                    City = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Notes = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Leads", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Leads_Authors_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "Authors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Leads_Posts_PostIdFk",
                        column: x => x.PostIdFk,
                        principalTable: "Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                }
            );

            migrationBuilder.CreateIndex(
                name: "IX_Leads_PrimaryPhoneE164",
                table: "Leads",
                column: "PrimaryPhoneE164");

            migrationBuilder.CreateIndex(
                name: "IX_Leads_Source_PostIdFk",
                table: "Leads",
                columns: new[] { "Source", "PostIdFk" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "Analyses");
            migrationBuilder.DropTable(name: "Leads");
            migrationBuilder.DropTable(name: "Posts");
            migrationBuilder.DropTable(name: "Authors");
        }
    }
}

