merge Genres as target
using (values
('Классическая проза'),
('Современная проза'),
('Поэзия'),
('Фольклор'),
('Фантастика'),
('Фантастика'),
('Фэнтези'),
('Манга'),
('Комикс'),
('Ранобэ'),
('Детектив'),
('Приключения'),
('Философия'),
('IT')) as source(Name)
on target.Name = source.Name
when not matched 
then insert (Name) values (source.Name);