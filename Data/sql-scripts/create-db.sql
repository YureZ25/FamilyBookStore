use [master]; -- используем главную таблицу MS SQL

if exists (select * from master.sys.databases where name = 'adonet-fbs') -- запрос к системным таблицам
begin
	print 'База данных с названием adonet-fbs уже существует' -- вывод в консоль
	return -- Выход из скрипта
end

create database [adonet-fbs]; -- указать имя из переменной не получится
go -- нужно выполнить создание, чтобы последующие команды было где исполнять

use [adonet-fbs]; -- переключаемся на созданную, чтобы не указывать ее во всех запросах

-- Запрос к унифицированным (у разных баз данных) системным таблицам
if exists (select * from INFORMATION_SCHEMA.TABLES where TABLE_SCHEMA = 'adonet-fbs' and TABLE_NAME = 'Books')
begin
	print 'Таблица с назнванием Books уже существует в базе данных adonet-fbs' 
	return
end

create table Books
(
	Id int identity(1,1) primary key, -- для авто инкремента используется модификатор identity (начальное значение, дельта)
	Title nvarchar(1024) -- строка с длиной до 128
);

---- Для предотвращения insert одинаковых данных несколько раз
--merge Books as target
--using (values
--('Идиот'),
--('Преступление и наказание'),
--('Солярис'),
--('Мартин Иден'),
--('Вино из одуванчиков'),
--('Странники'),
--('О дивный новый мир'),
--('Мечтают ли андроиды об электроовцах')) as source(Title)
--on target.Title = source.Title -- условие совпадения
--when not matched 
--then insert (Title) values (source.Title); -- если не совпало - вставляем данные
