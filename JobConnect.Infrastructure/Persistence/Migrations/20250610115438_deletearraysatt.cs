using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JobConnect.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class deletearraysatt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "JobSeekerWorkedAs");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "JobSeekerWorkedAs");

            migrationBuilder.DropColumn(
                name: "ProficiencyLevel",
                table: "JobSeekerSkills");

            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "JobSeekerCompanyWorkedAt");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "JobSeekerCompanyWorkedAt");

            migrationBuilder.DropColumn(
                name: "ExpiryDate",
                table: "JobSeekerCertifications");

            migrationBuilder.DropColumn(
                name: "IssueDate",
                table: "JobSeekerCertifications");

            migrationBuilder.DropColumn(
                name: "IssuingOrganization",
                table: "JobSeekerCertifications");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "JobSeekerWorkedAs",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "JobSeekerWorkedAs",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "ProficiencyLevel",
                table: "JobSeekerSkills",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "JobSeekerCompanyWorkedAt",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "JobSeekerCompanyWorkedAt",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpiryDate",
                table: "JobSeekerCertifications",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "IssueDate",
                table: "JobSeekerCertifications",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "IssuingOrganization",
                table: "JobSeekerCertifications",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
