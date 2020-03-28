using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.App.EF.Migrations
{
    public partial class AddFamilyTreeToPerson : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Persons_FamilyTrees_FamilyTreeId",
                table: "Persons");

            migrationBuilder.AlterColumn<int>(
                name: "FamilyTreeId",
                table: "Persons",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Persons_FamilyTrees_FamilyTreeId",
                table: "Persons",
                column: "FamilyTreeId",
                principalTable: "FamilyTrees",
                principalColumn: "FamilyTreeId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Persons_FamilyTrees_FamilyTreeId",
                table: "Persons");

            migrationBuilder.AlterColumn<int>(
                name: "FamilyTreeId",
                table: "Persons",
                type: "int",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_Persons_FamilyTrees_FamilyTreeId",
                table: "Persons",
                column: "FamilyTreeId",
                principalTable: "FamilyTrees",
                principalColumn: "FamilyTreeId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
