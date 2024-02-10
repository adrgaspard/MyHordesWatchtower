using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyHordesWatchtower.Infrastructure.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.CreateTable(
                name: "CitizenEntries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BatchId = table.Column<Guid>(type: "uuid", nullable: false),
                    BatchTimestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    HordesId = table.Column<int>(type: "integer", nullable: false),
                    Pseudo = table.Column<string>(type: "text", nullable: false),
                    Profession = table.Column<int>(type: "integer", nullable: false),
                    Chaman = table.Column<bool>(type: "boolean", nullable: false),
                    Stars = table.Column<long>(type: "bigint", nullable: false),
                    WellUses = table.Column<long>(type: "bigint", nullable: false),
                    LastConnectionDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    OutsideTown = table.Column<bool>(type: "boolean", nullable: false),
                    X = table.Column<int>(type: "integer", nullable: false),
                    Y = table.Column<int>(type: "integer", nullable: false),
                    DeathStatus = table.Column<int>(type: "integer", nullable: false),
                    Injured = table.Column<bool>(type: "boolean", nullable: false),
                    Infected = table.Column<bool>(type: "boolean", nullable: false),
                    Terrified = table.Column<bool>(type: "boolean", nullable: false),
                    DrugAddict = table.Column<bool>(type: "boolean", nullable: false),
                    Dehydrated = table.Column<bool>(type: "boolean", nullable: false),
                    Banned = table.Column<bool>(type: "boolean", nullable: false),
                    Charges = table.Column<long>(type: "bigint", nullable: false),
                    HomeLevel = table.Column<int>(type: "integer", nullable: false),
                    Constructions = table.Column<string>(type: "text", nullable: false),
                    Visible = table.Column<bool>(type: "boolean", nullable: false),
                    Items = table.Column<string>(type: "text", nullable: false),
                    Defense = table.Column<long>(type: "bigint", nullable: false),
                    Decoration = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    _ = table.PrimaryKey("PK_CitizenEntries", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.DropTable(
                name: "CitizenEntries");
        }
    }
}
