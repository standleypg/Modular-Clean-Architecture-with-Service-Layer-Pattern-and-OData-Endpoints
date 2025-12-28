using Microsoft.EntityFrameworkCore.Migrations;
using RetailPortal.Data.Db.Sql;

#nullable disable

namespace RetailPortal.Data.Db.Migrations
{
    /// <inheritdoc />
    public partial class DataSeeding : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(SqlHelper.GetSqlFromFile("Common", "Seed", "v0.0"));
            migrationBuilder.Sql(SqlHelper.GetSqlFromFile("Common", "Seed", "v1.0"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
