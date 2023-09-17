use [master]; -- ���������� ������� ������� MS SQL

if exists (select * from master.sys.databases where name = 'adonet-fbs') -- ������ � ��������� ��������
begin
	print '���� ������ � ��������� adonet-fbs ��� ����������' -- ����� � �������
	return -- ����� �� �������
end

create database [adonet-fbs]; -- ������� ��� �� ���������� �� ���������
go -- ����� ��������� ��������, ����� ����������� ������� ���� ��� ���������

use [adonet-fbs]; -- ������������� �� ���������, ����� �� ��������� �� �� ���� ��������

-- ������ � ��������������� (� ������ ��� ������) ��������� ��������
if exists (select * from INFORMATION_SCHEMA.TABLES where TABLE_SCHEMA = 'adonet-fbs' and TABLE_NAME = 'Books')
begin
	print '������� � ���������� Books ��� ���������� � ���� ������ adonet-fbs' 
	return
end

create table Books
(
	Id int identity(1,1) primary key, -- ��� ���� ���������� ������������ ����������� identity (��������� ��������, ������)
	Title nvarchar(1024) -- ������ � ������ �� 128
);

---- ��� �������������� insert ���������� ������ ��������� ���
--merge Books as target
--using (values
--('�����'),
--('������������ � ���������'),
--('�������'),
--('������ ����'),
--('���� �� �����������'),
--('���������'),
--('� ������ ����� ���'),
--('������� �� �������� �� ������������')) as source(Title)
--on target.Title = source.Title -- ������� ����������
--when not matched 
--then insert (Title) values (source.Title); -- ���� �� ������� - ��������� ������
