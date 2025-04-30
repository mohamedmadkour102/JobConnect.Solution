using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JobConnect.Apis.Migrations
{
    /// <inheritdoc />
    public partial class EditJob01 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Responsibilities",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "Tags",
                table: "Jobs");

            migrationBuilder.CreateTable(
                name: "JobResponsibilities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    JobId = table.Column<int>(type: "int", nullable: false),
                    Responsibility = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobResponsibilities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobResponsibilities_Jobs_JobId",
                        column: x => x.JobId,
                        principalTable: "Jobs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JobTags",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    JobId = table.Column<int>(type: "int", nullable: false),
                    Tag = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobTags", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobTags_Jobs_JobId",
                        column: x => x.JobId,
                        principalTable: "Jobs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_JobResponsibilities_JobId",
                table: "JobResponsibilities",
                column: "JobId");

            migrationBuilder.CreateIndex(
                name: "IX_JobTags_JobId",
                table: "JobTags",
                column: "JobId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JobResponsibilities");

            migrationBuilder.DropTable(
                name: "JobTags");

            migrationBuilder.AddColumn<string>(
                name: "Responsibilities",
                table: "Jobs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Tags",
                table: "Jobs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
