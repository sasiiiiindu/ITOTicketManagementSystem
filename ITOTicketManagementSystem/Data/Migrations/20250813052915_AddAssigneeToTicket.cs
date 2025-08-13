using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ITOTicketManagementSystem.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAssigneeToTicket : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CurrentAssignee",
                table: "Tickets",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentAssignee",
                table: "Tickets");
        }
    }
}
