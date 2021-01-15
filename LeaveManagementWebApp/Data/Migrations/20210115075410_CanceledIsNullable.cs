using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LeaveManagementWebApp.Data.Migrations
{
    public partial class CanceledIsNullable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            

            migrationBuilder.AlterColumn<bool>(
                name: "Canceled",
                table: "LeaveRequests",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "Canceled",
                table: "LeaveRequests",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldNullable: true);

            
        }
    }
}
