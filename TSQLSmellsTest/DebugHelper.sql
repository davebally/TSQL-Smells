CREATE TABLE [dbo].[Table1]
(
	[Id] INT NOT NULL PRIMARY KEY,
	ColA integer not null
)
GO
CREATE TABLE [dbo].[Table2]
(
	[Id] INT NOT NULL PRIMARY KEY,
	ColB integer not null
)
Go
CREATE PROCEDURE [dbo].[Procedure1]
	@param1 int = 0,
	@param2 int
AS
	Select * from 
	Table1 a1 join Table2 a2
	on a1.Id = a2.id
	where a2.ColB = 1

	Select * from 
	Table1 a1 join Table2 a2
	on a1.Id = a2.id
	and a2.ColB = 1


RETURN 0
