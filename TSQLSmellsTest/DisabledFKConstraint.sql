Create Table dbo.Parent
(
ParentId integer primary key,
Data char(10) not null
)
go
Create Table dbo.Child
(
ChildId integer primary key,
ParentId integer constraint fkParentChild foreign key references dbo.Parent(ParentId) 

)
go

ALTER TABLE Child NOCHECK CONSTRAINT fkParentChild
