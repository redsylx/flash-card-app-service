CREATE TABLE Account (
    Id VARCHAR(36) PRIMARY KEY,
    Email NVARCHAR(320) NOT NULL UNIQUE,
    Username NVARCHAR(12) NULL UNIQUE,
    CreatedTime DATETIME NOT NULL,
    LastUpdatedTime DATETIME NOT NULL
);