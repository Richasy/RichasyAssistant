﻿//<auto-generated />
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RichasyAssistant.Models.Context.Migrations
{
    /// <inheritdoc />
    public partial class ChatDbAddAssistantModelDelploymentName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ModelDeploymentName",
                table: "Assistants",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ModelDeploymentName",
                table: "Assistants");
        }
    }
}
