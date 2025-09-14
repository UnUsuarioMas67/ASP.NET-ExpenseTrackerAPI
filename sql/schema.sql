USE master
DROP DATABASE ExpenseTracker
GO

CREATE DATABASE ExpenseTracker
GO
USE ExpenseTracker

CREATE TABLE Users (
	UserId INT PRIMARY KEY IDENTITY(1,1),
	Username VARCHAR(100) NOT NULL,
	Email VARCHAR(254) NOT NULL UNIQUE,
	HashedPassword VARCHAR(MAX) NOT NULL
)

CREATE TABLE ExpenseCategories (
	CategoryId VARCHAR(25) PRIMARY KEY,
	CategoryName VARCHAR(100) NOT NULL,
)

INSERT INTO ExpenseCategories (CategoryId, CategoryName)
VALUES
('groceries', 'Groceries'),
('leisure', 'Leisure'),
('electronics', 'Electronics'),
('utilities', 'Utilities'),
('clothing', 'Clothing'),
('health', 'Health'),
('other', 'Others')

CREATE TABLE Expenses (
	ExpenseId INT PRIMARY KEY IDENTITY(1,1),
	[Description] VARCHAR(200) NOT NULL,
	Amount DECIMAL NOT NULL,
	ExpenseDate DATE NOT NULL,
	CategoryId VARCHAR(25) NOT NULL FOREIGN KEY REFERENCES ExpenseCategories(CategoryId)
)


CREATE TABLE UserExpenses (
	UserId INT,
	ExpenseId INT

	PRIMARY KEY (UserId, ExpenseId)
)

SELECT * FROM ExpenseCategories