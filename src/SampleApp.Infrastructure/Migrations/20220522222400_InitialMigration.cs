using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SampleApp.Infrastructure.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Username = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ZipCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    GivenName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Username);
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Username", "Address", "GivenName", "PasswordHash", "Role", "ZipCode" },
                values: new object[] { "admin-user", "5036 Tierra Locks Suite 158", "Admin User", "$2a$11$1W9N/zaUdmo.PXWAdU4L3Ov3E8suY75ZMVh19Do/fPjwT/mnaohNO", 0, "980395900" });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Username", "Address", "GivenName", "PasswordHash", "Role", "ZipCode" },
                values: new object[] { "customer-user", "570 Hackett Bridge", "Customer User", "$2a$11$/UdRM0TQbDd8rOITPM7UQ.SVjtgkABxfhHNFE1QsVTnzv.s4mLbCy", 1, "948019535" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
