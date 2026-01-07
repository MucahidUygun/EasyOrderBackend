using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class fitRefreshTokenUserIdMapping : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // UserId3 index varsa sil
            migrationBuilder.Sql(@"
        IF EXISTS (
            SELECT 1 FROM sys.indexes 
            WHERE name = 'IX_RefreshToken_UserId3'
        )
        DROP INDEX IX_RefreshToken_UserId3 ON RefreshToken
    ");

            // UserId3 kolonu varsa sil
            migrationBuilder.Sql(@"
        IF EXISTS (
            SELECT 1 FROM sys.columns 
            WHERE name = 'UserId3'
              AND object_id = OBJECT_ID('RefreshToken')
        )
        ALTER TABLE RefreshToken DROP COLUMN UserId3
    ");

            // FK varsa sil (çakışma ihtimali)
            migrationBuilder.Sql(@"
        IF EXISTS (
            SELECT 1 FROM sys.foreign_keys 
            WHERE parent_object_id = OBJECT_ID('RefreshToken')
              AND name LIKE 'FK_RefreshToken_%'
              AND name <> 'FK_RefreshToken_Users_UserId'
        )
        BEGIN
            DECLARE @sql NVARCHAR(MAX) = '';
            SELECT @sql += 'ALTER TABLE RefreshToken DROP CONSTRAINT ' + QUOTENAME(name) + ';'
            FROM sys.foreign_keys
            WHERE parent_object_id = OBJECT_ID('RefreshToken')
              AND name LIKE 'FK_RefreshToken_%'
              AND name <> 'FK_RefreshToken_Users_UserId';
            EXEC sp_executesql @sql;
        END
    ");

            // Doğru FK yoksa ekle
            migrationBuilder.Sql(@"
        IF NOT EXISTS (
            SELECT 1 FROM sys.foreign_keys 
            WHERE name = 'FK_RefreshToken_Users_UserId'
        )
        ALTER TABLE RefreshToken
        ADD CONSTRAINT FK_RefreshToken_Users_UserId
        FOREIGN KEY (UserId) REFERENCES Users(Id)
        ON DELETE NO ACTION
    ");

            // UserId index yoksa ekle
            migrationBuilder.Sql(@"
        IF NOT EXISTS (
            SELECT 1 FROM sys.indexes 
            WHERE name = 'IX_RefreshToken_UserId'
        )
        CREATE INDEX IX_RefreshToken_UserId
        ON RefreshToken(UserId)
    ");
        }


        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
        IF EXISTS (
            SELECT 1 FROM sys.foreign_keys 
            WHERE name = 'FK_RefreshToken_Users_UserId'
        )
        ALTER TABLE RefreshToken DROP CONSTRAINT FK_RefreshToken_Users_UserId
    ");

            migrationBuilder.Sql(@"
        IF EXISTS (
            SELECT 1 FROM sys.indexes 
            WHERE name = 'IX_RefreshToken_UserId'
        )
        DROP INDEX IX_RefreshToken_UserId ON RefreshToken
    ");
        }

    }
}
