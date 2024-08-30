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

CREATE TABLE Card (
    Id VARCHAR(36) PRIMARY KEY,
    ClueTxt NVARCHAR(120) NOT NULL,
    ClueImg NVARCHAR(256),
    DescriptionTxt NVARCHAR(256) NOT NULL,
    DescriptionImg NVARCHAR(256),
    NFrequency INT NOT NULL CHECK (NFrequency >= 0),
    NCorrect INT NULL,
    PctCorrect DECIMAL(3,2) NULL CHECK (PctCorrect >= 0 AND PctCorrect <= 1),
    CardCategoryId VARCHAR(36) NOT NULL,
    CurrentVersionId VARCHAR(36) NOT NULL;
    CreatedTime DATETIME NOT NULL,
    LastUpdatedTime DATETIME NOT NULL,
    CONSTRAINT fk_cardcategory FOREIGN KEY (CardCategoryId) REFERENCES CardCategory(Id),
);

CREATE TABLE CardVersion (
    Id VARCHAR(36) PRIMARY KEY,
    ClueTxt NVARCHAR(120) NOT NULL,
    ClueImg NVARCHAR(256),
    DescriptionTxt NVARCHAR(256) NOT NULL,
    DescriptionImg NVARCHAR(256),
    CardId VARCHAR(36) NOT NULL,
    CreatedTime DATETIME NOT NULL,
    LastUpdatedTime DATETIME NOT NULL,
    CONSTRAINT fk_card FOREIGN KEY (CardId) REFERENCES Card(Id),
);

CREATE TABLE Game (
    Id VARCHAR(36) PRIMARY KEY,
    CreatedTime DATETIME NOT NULL,
    LastUpdatedTime DATETIME NOT NULL,
    Status VARCHAR(50) NOT NULL,
    NCard INT NOT NULL CHECK (NCard BETWEEN 10 AND 30),
    PctCorrect DECIMAL(5,2) NULL CHECK (PctCorrect BETWEEN 0 AND 1),
    HideDurationInSecond INT NOT NULL CHECK (HideDurationInSecond BETWEEN 5 AND 120),
    AccountId VARCHAR(36) NOT NULL,
    CONSTRAINT fk_account_game FOREIGN KEY (AccountId) REFERENCES Account(Id),
);

CREATE TABLE GameDetail (
    Id VARCHAR(36) PRIMARY KEY,
    CreatedTime DATETIME NOT NULL,
    LastUpdatedTime DATETIME NOT NULL,
    IsCorrect BIT,
    IsAnswered BIT,
    IndexNumber INT,
    GameId VARCHAR(36) NOT NULL,
    CardVersionId VARCHAR(36) NOT NULL,
    CONSTRAINT fk_gamedetail_game FOREIGN KEY (GameId) REFERENCES Game(Id),
    CONSTRAINT fk_gamedetail_cardversion FOREIGN KEY (CardVersionId) REFERENCES CardVersion(Id),
);

CREATE TABLE GameDetailCategory (
    Id VARCHAR(36) PRIMARY KEY,
    CreatedTime DATETIME NOT NULL,
    LastUpdatedTime DATETIME NOT NULL,
    GameId VARCHAR(36) NOT NULL,
    CardCategoryId VARCHAR(36) NOT NULL,
    CONSTRAINT fk_gamedetailcategory_game FOREIGN KEY (GameId) REFERENCES Game(Id),
    CONSTRAINT fk_gamedetail_cardcategory FOREIGN KEY (CardCategoryId) REFERENCES CardCategory(Id),
);