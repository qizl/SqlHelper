create table FileTerms
(
	ID uniqueidentifier primary key,
	Title nvarchar(500),
	Describe nvarchar(500),
	GenreID uniqueidentifier,
	CreateTime datetime,
	IsNotice bit,
	Amounts int 
)