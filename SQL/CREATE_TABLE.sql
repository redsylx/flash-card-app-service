CREATE TABLE Account (
    Id VARCHAR(36) PRIMARY KEY,
    Email NVARCHAR(320) NOT NULL UNIQUE,
    Username NVARCHAR(12) NULL UNIQUE,
    CreatedTime DATETIME NOT NULL,
    LastUpdatedTime DATETIME NOT NULL
);

CREATE TABLE CardCategory (
    Id VARCHAR(36) PRIMARY KEY,
    Name NVARCHAR(12) NOT NULL,
    NCard INT NOT NULL CHECK (NCard >= 0),
    PctCorrect DECIMAL(3,2) NULL CHECK (PctCorrect >= 0 AND PctCorrect <= 1),
    AccountId VARCHAR(36) NOT NULL,
    CreatedTime DATETIME NOT NULL,
    LastUpdatedTime DATETIME NOT NULL,
    CONSTRAINT fk_account FOREIGN KEY (AccountId) REFERENCES Account(Id)
);