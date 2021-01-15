using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LeaveManagementWebApp.Data.Migrations
{
    public partial class LeaveRequestHasCancelColumnNow : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Canceled",
                table: "LeaveRequests",
                nullable: false,
                defaultValue: false);

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            

            migrationBuilder.DropColumn(
                name: "Canceled",
                table: "LeaveRequests");
        }
    }
}
