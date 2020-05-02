using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MediaPack.Migrations
{
    public partial class NewMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppSettings",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SettingName = table.Column<string>(nullable: false),
                    Value = table.Column<string>(nullable: false),
                    IsEditable = table.Column<int>(nullable: false),
                    DefaultValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ChannelFavorites",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ChannelId = table.Column<long>(nullable: false),
                    AddedDate = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChannelFavorites", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Channels",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Country = table.Column<string>(nullable: true),
                    CategoryId = table.Column<long>(nullable: false),
                    ChannelType = table.Column<string>(unicode: false, nullable: false),
                    ChannelCode = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    M3U8Address = table.Column<string>(nullable: true),
                    ParserAddress = table.Column<string>(nullable: true),
                    ParserScript = table.Column<string>(nullable: true),
                    ParserRequestRegex = table.Column<string>(nullable: true),
                    ParserDocumentRegex = table.Column<string>(nullable: true),
                    ParseDocument = table.Column<int>(nullable: false),
                    QualityOptions = table.Column<string>(nullable: true),
                    QualityOptionRegex = table.Column<string>(nullable: true),
                    Logo = table.Column<byte[]>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Channels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SpendTime",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TvChannelId = table.Column<long>(nullable: false),
                    AddedDate = table.Column<string>(nullable: false),
                    WatchTime = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpendTime", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Channels_ChannelCode",
                table: "Channels",
                column: "ChannelCode",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppSettings");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "ChannelFavorites");

            migrationBuilder.DropTable(
                name: "Channels");

            migrationBuilder.DropTable(
                name: "SpendTime");
        }
    }
}
