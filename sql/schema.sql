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
GO;

CREATE OR ALTER PROC sp_AddExpense (@Description VARCHAR(200), @Amount DECIMAL, @ExpenseDate DATE, @CategoryId VARCHAR(25), @UserEmail VARCHAR(254))
AS
BEGIN
	INSERT INTO Expenses ([Description], Amount, ExpenseDate, CategoryId)
	VALUES (@Description, @Amount, @ExpenseDate, @CategoryId)

	DECLARE @ExpenseId INT, @UserId INT

	SET @ExpenseId = SCOPE_IDENTITY()
	SELECT @UserId = UserId FROM Users
	WHERE Email = @UserEmail

	INSERT INTO UserExpenses (UserId, ExpenseId)
	VALUES (@UserId, @ExpenseId)

	SELECT ExpenseId, [Description], Amount, ExpenseDate as [Date], c.CategoryId, c.CategoryName 
	FROM Expenses e
	JOIN ExpenseCategories c ON e.CategoryId = c.CategoryId
	WHERE e.ExpenseId = @ExpenseId
END
GO

CREATE OR ALTER PROC sp_GetExpensesFromUser (@UserEmail VARCHAR(254))
AS
BEGIN
	SELECT e.ExpenseId, e.[Description], e.Amount, e.ExpenseDate AS [Date], c.CategoryId, c.CategoryName
	FROM Expenses e
	JOIN ExpenseCategories c ON e.CategoryId = c.CategoryId
	JOIN UserExpenses ue ON e.ExpenseId = ue.ExpenseId
	JOIN Users u ON ue.UserId = u.UserId
	WHERE u.Email = @UserEmail
END
GO

CREATE OR ALTER PROC sp_DeleteExpense (@ExpenseId INT)
AS 
BEGIN
	DELETE FROM UserExpenses WHERE ExpenseId = @ExpenseId
	DELETE FROM Expenses WHERE ExpenseId = @ExpenseId
END
GO

CREATE OR ALTER PROC sp_CreateUserIfEmailDoesNotExists (@Username VARCHAR(100), @Email VARCHAR(254), @HashedPassword VARCHAR(MAX))
AS
BEGIN 
    IF @Email NOT IN (SELECT Email FROM Users) 
    BEGIN 
        INSERT INTO Users (Username, Email, HashedPassword)
        VALUES (@Username, @Email, @HashedPassword)
        
        SELECT u.UserId, u.Username, u.Email, u.HashedPassword FROM Users u WHERE Email = @Email
    END
END
GO