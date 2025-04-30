using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JobConnect.Apis.Migrations
{
    /// <inheritdoc />
    public partial class AddResumes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Resumes",
                table: "JobSeekers");

            migrationBuilder.CreateTable(
                name: "JobSeekerResumes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    JobSeekerId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ResumePath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ResumeName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UploadDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobSeekerResumes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobSeekerResumes_JobSeekers_JobSeekerId",
                        column: x => x.JobSeekerId,
                        principalTable: "JobSeekers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_JobSeekerResumes_JobSeekerId",
                table: "JobSeekerResumes",
                column: "JobSeekerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JobSeekerResumes");

            migrationBuilder.AddColumn<string>(
                name: "Resumes",
                table: "JobSeekers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
