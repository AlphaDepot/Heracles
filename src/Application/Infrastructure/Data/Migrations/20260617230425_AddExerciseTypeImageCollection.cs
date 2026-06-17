using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Application.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddExerciseTypeImageCollection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "ExerciseTypes");

            migrationBuilder.AddColumn<List<string>>(
                name: "Images",
                table: "ExerciseTypes",
                type: "text[]",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Images",
                table: "ExerciseTypes");

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "ExerciseTypes",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true);
        }
    }
}
