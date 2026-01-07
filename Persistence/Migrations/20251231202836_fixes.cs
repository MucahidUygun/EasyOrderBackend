using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class fixes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1️⃣ UserId3 varsa indexini sil
            migrationBuilder.Sql(@"
        IF EXISTS (
            SELECT 1 FROM sys.indexes 
            WHERE name = 'IX_RefreshToken_UserId3'
        )
        DROP INDEX IX_RefreshToken_UserId3 ON RefreshToken
    ");

            // 2️⃣ UserId3 kolonu varsa sil
            migrationBuilder.Sql(@"
        IF EXISTS (
            SELECT 1 FROM sys.columns 
            WHERE Name = N'UserId3' 
            AND Object_ID = Object_ID(N'RefreshToken')
        )
        ALTER TABLE RefreshToken DROP COLUMN UserId3
    ");

            // 3️⃣ UserId indexi yoksa ekle
            migrationBuilder.Sql(@"
        IF NOT EXISTS (
            SELECT 1 FROM sys.indexes 
            WHERE name = 'IX_RefreshToken_UserId'
        )
        CREATE INDEX IX_RefreshToken_UserId 
        ON RefreshToken(UserId)
    ");

            // 4️⃣ ForeignKey yoksa ekle
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
