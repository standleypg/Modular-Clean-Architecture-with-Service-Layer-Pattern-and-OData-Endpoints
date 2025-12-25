using System;
using Microsoft.EntityFrameworkCore.Migrations;
using RetailPortal.Data.Db.Sql;

#nullable disable

namespace RetailPortal.Data.Db.Migrations
{
    /// <inheritdoc />
    public partial class Update_Db_Structure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Step 1: Drop all foreign keys that will be affected
            migrationBuilder.DropForeignKey(
                name: "FK_Addresses_Users_UserId",
                table: "Addresses");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_Categories_CategoryId",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_Sellers_SellerId",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_UserRoles_Roles_RoleId",
                table: "UserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_UserRoles_Users_UserId",
                table: "UserRoles");

            // Step 2: Drop tables that are no longer needed (keep Sellers for now - needed for data migration)
            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_Products_CategoryId",
                table: "Products");

            // Step 3: Drop columns that are no longer needed
            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "Products");

            // Step 4: Add temporary columns for new numeric IDs
            migrationBuilder.AddColumn<decimal>(
                name: "NewId",
                table: "Users",
                type: "numeric(20,0)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "NewId",
                table: "Roles",
                type: "numeric(20,0)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "NewId",
                table: "Products",
                type: "numeric(20,0)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "NewId",
                table: "Addresses",
                type: "numeric(20,0)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "NewUserId",
                table: "Addresses",
                type: "numeric(20,0)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "NewUserId",
                table: "UserRoles",
                type: "numeric(20,0)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "NewRoleId",
                table: "UserRoles",
                type: "numeric(20,0)",
                nullable: false,
                defaultValue: 0m);

            // Step 5: Populate new IDs with sequential numbers and update references
            migrationBuilder.Sql(@"
                WITH numbered_users AS (
                    SELECT ""Id"", ROW_NUMBER() OVER (ORDER BY ""CreatedAt"") as new_id
                    FROM ""Users""
                )
                UPDATE ""Users"" u
                SET ""NewId"" = nu.new_id
                FROM numbered_users nu
                WHERE u.""Id"" = nu.""Id"";
            ");

            migrationBuilder.Sql(@"
                WITH numbered_roles AS (
                    SELECT ""Id"", ROW_NUMBER() OVER (ORDER BY ""CreatedAt"") as new_id
                    FROM ""Roles""
                )
                UPDATE ""Roles"" r
                SET ""NewId"" = nr.new_id
                FROM numbered_roles nr
                WHERE r.""Id"" = nr.""Id"";
            ");

            migrationBuilder.Sql(@"
                WITH numbered_products AS (
                    SELECT ""Id"", ROW_NUMBER() OVER (ORDER BY ""CreatedAt"") as new_id
                    FROM ""Products""
                )
                UPDATE ""Products"" p
                SET ""NewId"" = np.new_id
                FROM numbered_products np
                WHERE p.""Id"" = np.""Id"";
            ");

            migrationBuilder.Sql(@"
                WITH numbered_addresses AS (
                    SELECT ""Id"", ROW_NUMBER() OVER (ORDER BY ""CreatedAt"") as new_id
                    FROM ""Addresses""
                )
                UPDATE ""Addresses"" a
                SET ""NewId"" = na.new_id
                FROM numbered_addresses na
                WHERE a.""Id"" = na.""Id"";
            ");

            // Update foreign key references
            migrationBuilder.Sql(@"
                UPDATE ""Addresses"" a
                SET ""NewUserId"" = u.""NewId""
                FROM ""Users"" u
                WHERE a.""UserId"" = u.""Id"";
            ");

            migrationBuilder.Sql(@"
                UPDATE ""UserRoles"" ur
                SET ""NewUserId"" = u.""NewId""
                FROM ""Users"" u
                WHERE ur.""UserId"" = u.""Id"";
            ");

            migrationBuilder.Sql(@"
                UPDATE ""UserRoles"" ur
                SET ""NewRoleId"" = r.""NewId""
                FROM ""Roles"" r
                WHERE ur.""RoleId"" = r.""Id"";
            ");

            // Step 6: Drop indexes on FK columns that might block PK drops
            migrationBuilder.DropIndex(
                name: "IX_Addresses_UserId",
                table: "Addresses");

            migrationBuilder.DropIndex(
                name: "IX_UserRoles_RoleId",
                table: "UserRoles");

            // Step 6a: Drop old primary keys using CASCADE to handle dependencies
            migrationBuilder.Sql(@"ALTER TABLE ""UserRoles"" DROP CONSTRAINT ""PK_UserRoles"";");
            migrationBuilder.Sql(@"ALTER TABLE ""Users"" DROP CONSTRAINT ""PK_Users"" CASCADE;");
            migrationBuilder.Sql(@"ALTER TABLE ""Roles"" DROP CONSTRAINT ""PK_Roles"" CASCADE;");
            migrationBuilder.Sql(@"ALTER TABLE ""Products"" DROP CONSTRAINT ""PK_Products"" CASCADE;");
            migrationBuilder.Sql(@"ALTER TABLE ""Addresses"" DROP CONSTRAINT ""PK_Addresses"" CASCADE;");

            // Step 7: Rename old Id columns to Guid and new columns to Id
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Users",
                newName: "Guid");

            migrationBuilder.RenameColumn(
                name: "NewId",
                table: "Users",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Roles",
                newName: "Guid");

            migrationBuilder.RenameColumn(
                name: "NewId",
                table: "Roles",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Products",
                newName: "Guid");

            migrationBuilder.RenameColumn(
                name: "NewId",
                table: "Products",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Addresses",
                newName: "Guid");

            migrationBuilder.RenameColumn(
                name: "NewId",
                table: "Addresses",
                newName: "Id");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Addresses");

            migrationBuilder.RenameColumn(
                name: "NewUserId",
                table: "Addresses",
                newName: "UserId");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "UserRoles");

            migrationBuilder.RenameColumn(
                name: "NewUserId",
                table: "UserRoles",
                newName: "UserId");

            migrationBuilder.DropColumn(
                name: "RoleId",
                table: "UserRoles");

            migrationBuilder.RenameColumn(
                name: "NewRoleId",
                table: "UserRoles",
                newName: "RoleId");

            // Step 8: Add new columns
            migrationBuilder.AddColumn<int>(
                name: "Category",
                table: "Products",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "UserId",
                table: "Products",
                type: "numeric(20,0)",
                nullable: false,
                defaultValue: 0m);

            // Populate Products.UserId from Sellers table
            migrationBuilder.Sql(@"
                UPDATE ""Products"" p
                SET ""UserId"" = u.""Id""
                FROM ""Sellers"" s
                INNER JOIN ""Users"" u ON s.""UserId"" = u.""Guid""
                WHERE p.""SellerId"" = s.""Id"";
            ");

            // Delete Products that couldn't be assigned a valid UserId (orphaned products)
            migrationBuilder.Sql(@"
                DELETE FROM ""Products""
                WHERE ""UserId"" = 0;
            ");

            // Now safe to drop Sellers table (FK was already dropped by CASCADE earlier)
            migrationBuilder.DropTable(
                name: "Sellers");

            migrationBuilder.DropIndex(
                name: "IX_Products_SellerId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "SellerId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "SellerId",
                table: "Products");

            // Step 9: Recreate primary keys
            migrationBuilder.AddPrimaryKey(
                name: "PK_Users",
                table: "Users",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Roles",
                table: "Roles",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Products",
                table: "Products",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Addresses",
                table: "Addresses",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserRoles",
                table: "UserRoles",
                columns: new[] { "UserId", "RoleId" });

            // Step 10: Create indexes
            migrationBuilder.CreateIndex(
                name: "IX_Products_UserId",
                table: "Products",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_UserId",
                table: "Addresses",
                column: "UserId");

            // Step 11: Recreate foreign keys
            migrationBuilder.AddForeignKey(
                name: "FK_Addresses_Users_UserId",
                table: "Addresses",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Users_UserId",
                table: "Products",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserRoles_Roles_RoleId",
                table: "UserRoles",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserRoles_Users_UserId",
                table: "UserRoles",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // This is a destructive migration - down migration would lose data
            throw new NotSupportedException("Cannot rollback this migration as it would result in data loss. Please restore from backup if needed.");
        }
    }
}
