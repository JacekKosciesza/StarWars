using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace StarWars.Data.EntityFramework.Migrations
{
    public partial class Hero : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "HeroId",
                table: "Episodes",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Episodes_HeroId",
                table: "Episodes",
                column: "HeroId");

            migrationBuilder.AddForeignKey(
                name: "FK_Episodes_Characters_HeroId",
                table: "Episodes",
                column: "HeroId",
                principalTable: "Characters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Episodes_Characters_HeroId",
                table: "Episodes");

            migrationBuilder.DropIndex(
                name: "IX_Episodes_HeroId",
                table: "Episodes");

            migrationBuilder.DropColumn(
                name: "HeroId",
                table: "Episodes");
        }
    }
}
