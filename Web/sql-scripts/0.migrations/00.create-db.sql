declare @dbName nvarchar(100) = '$(db-name)';

use [$(db-name)];

if not exists (select * from INFORMATION_SCHEMA.TABLES where TABLE_CATALOG = @dbName and TABLE_NAME = 'Books')
begin
	create table Books
	(
		Id int identity(1,1) primary key,
		Title nvarchar(1024) not null,
		Description nvarchar(max),
		IsbnStoreValue bigint null,
		PageCount smallint null,
		Price smallmoney null,
		ImageId int null,
		AuthorId int not null,
		GenreId int not null
	);
end

if not exists (select * from INFORMATION_SCHEMA.TABLES where TABLE_CATALOG = @dbName and TABLE_NAME = 'Authors')
begin
	create table Authors
	(
		Id int identity(1,1) primary key,
		FirstName nvarchar(1024) not null,
		LastName nvarchar(1024) not null
	);
end

if not exists (select * from INFORMATION_SCHEMA.TABLES where TABLE_CATALOG = @dbName and TABLE_NAME = 'Genres')
begin
	create table Genres
	(
		Id int identity(1,1) primary key,
		Name nvarchar(128) not null
	);
end

if not exists (select * from INFORMATION_SCHEMA.TABLES where TABLE_CATALOG = @dbName and TABLE_NAME = 'Stores')
begin
	create table Stores
	(
		Id int identity(1,1) primary key,
		Name nvarchar(128) not null,
		Address nvarchar(1024)
	);
end

if not exists (select * from INFORMATION_SCHEMA.TABLES where TABLE_CATALOG = @dbName and TABLE_NAME = 'Book2Stores')
begin
	create table Book2Stores
	(
		BookId int not null foreign key references Books(Id),
		StoreId int not null foreign key references Stores(Id)
	);
end

if not exists (select * from INFORMATION_SCHEMA.TABLES where TABLE_CATALOG = @dbName and TABLE_NAME = 'Users')
begin
	create table Users
	(
		Id int identity(1,1) primary key,
		UserName nvarchar(128) not null,
		NormalizedUserName nvarchar(128) not null,
		PasswordHash nvarchar(128)
	);
end

if not exists (select * from INFORMATION_SCHEMA.TABLES where TABLE_CATALOG = @dbName and TABLE_NAME = 'Users2Stores')
begin
	create table Users2Stores
	(
		UserId int not null foreign key references Users(Id),
		StoreId int not null foreign key references Stores(Id)
	);
end

if not exists (select * from INFORMATION_SCHEMA.TABLES where TABLE_CATALOG = @dbName and TABLE_NAME = 'UsersBooksStatuses')
begin
	create table UsersBooksStatuses
	(
		Id int identity(1,1) primary key,
		UserId int not null foreign key references Users(Id),
		BookId int not null foreign key references Books(Id),
		BookStatus tinyint not null,
		WishRead datetime2(7) null,
		StartRead datetime2(7) null,
		CurrentPage smallint null,
		EndRead datetime2(7) null
	);
end

if not exists (select * from INFORMATION_SCHEMA.TABLES where TABLE_CATALOG = @dbName and TABLE_NAME = 'BookImages')
begin
	create table BookImages
	(
		Id int identity(1,1) primary key,
		FileName nvarchar(128) not null,
		ContentType nvarchar(64) not null,
		Content varbinary(max) not null
	);
end

if not exists (select * from INFORMATION_SCHEMA.TABLES where TABLE_CATALOG = @dbName and TABLE_NAME = 'BookQuotes')
begin
	create table BookQuotes
	(
		Id int identity(1,1) primary key,
		BookId int not null foreign key references Books(Id),
		Page smallint not null,
		Text nvarchar(max) not null
	);
end

if not exists (select * from INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS where CONSTRAINT_NAME = 'FK_Books_Authors_AuthorId')
begin
	alter table Books
	add constraint FK_Books_Authors_AuthorId
	foreign key (AuthorId) references Authors(Id);
end

if not exists (select * from INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS where CONSTRAINT_NAME = 'FK_Books_Genres_GenreId')
begin
	alter table Books
	add constraint FK_Books_Genres_GenreId
	foreign key (GenreId) references Genres(Id);
end

if not exists (select * from INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS where CONSTRAINT_NAME = 'FK_Books_BookImages_ImageId')
begin
	alter table Books
	add constraint FK_Books_BookImages_ImageId
	foreign key (ImageId) references BookImages(Id);
end

declare @indexes table (IndexName nvarchar(max), IndexType nvarchar(max), TableName nvarchar(max), IsIndexUnique bit)

insert into @indexes
SELECT I.name, I.type_desc, T.name, I.is_unique
FROM sys.indexes AS I
INNER JOIN sys.tables AS T
    ON I.[object_id] = T.[object_id]
WHERE I.type_desc <> N'HEAP' and I.is_primary_key = 0;

if not exists (select * from @indexes where TableName = 'Book2Stores' and IndexName = 'IX_Book2Stores_BookId')
begin
	CREATE UNIQUE INDEX IX_Book2Stores_BookId
	ON Book2Stores(BookId);
end

