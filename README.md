# TSQL-Smells

TSQL Smells Finder - SSDT

TSQL Code can smell, it may work just fine but there can be hidden dangers held within.  Code that works just fine one day can break if the database schema changes underneath.

This project extends the Static Code Analysis rules inside SSDT, and therefore msbuild, to test that code meets certain rules.  Each rule is individually selectable to be active and to have "Warnings as Errors".
There are presently 43 rules covering:

Potential SQL Injection Issue 
Avoid cross server joins
Use two part naming
Use of nolock / UNCOMMITTED READS
Use of Table / Query hints
Use of Select *
Explicit Conversion of Columnar data – Non Sargable predicates
Ordinal positions in ORDER BY Clauses
Change Of DateFormat
Change Of DateFirst
SET ROWCOUNT
Missing Column specifications on insert
SET OPTION usage
Use 2 part naming in EXECUTE statements
SET IDENTITY_INSERT
Use of RANGE windows in SQL Server 2012
Create table statements should specify schema
View created with ORDER
Writable cursors
SET NOCOUNT ON should be included inside stored procedures
COUNT(*) used when EXISTS/NOT EXISTS can be more performant
use of TOP(100) percent or TOP(>9999) in a derived table

 

Whitelisting of objects is supported with extended properties.  Add to you project a script which sets the name of WhiteList against an object to be ignored.  For example :

EXEC sys.sp_addextendedproperty @name=N’WhiteList’, @value=N’Ignore’ , @level0type=N’SCHEMA’,@level0name=N’dbo’, @level1type=N’PROCEDURE’,@level1name=N’WhiteListTestProc’
