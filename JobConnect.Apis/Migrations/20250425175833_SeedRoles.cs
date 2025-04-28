using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JobConnect.Apis.Migrations
{
    /// <inheritdoc />
    public partial class SeedRoles : Migration
    {
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.InsertData(
	table: "AspNetRoles",
	columns: new[] { "Id", "Name", "NormalizedName", "ConcurrencyStamp" },
	values: new object[] { Guid.NewGuid().ToString(), "employer", "EMPLOYER", Guid.NewGuid().ToString() }
);
			migrationBuilder.InsertData(
				table: "AspNetRoles",
				columns: new[] { "Id", "Name", "NormalizedName", "ConcurrencyStamp" },
				values: new object[] { Guid.NewGuid().ToString(), "jobseeker", "JOBSEEKER", Guid.NewGuid().ToString() }
			);
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DeleteData(
   table: "AspNetRoles",
   keyColumn: "Name",
   keyValue: "employer"
);
			migrationBuilder.DeleteData(
			   table: "AspNetRoles",
			   keyColumn: "Name",
			   keyValue: "jobseeker"
		   );
		}
	}
}
