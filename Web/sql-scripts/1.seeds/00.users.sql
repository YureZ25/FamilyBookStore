merge Users as target
using 
(
	values
	('yadmin', 'YADMIN', 'AQAAAAIAAYagAAAAEC5Bm5LC0nG/tEECFeybKzUJbTzUReundaTYzXHL+narlJMJS1NxohqMntUGdBE5xA==')
) as source(UserName, NormalizedUserName, PasswordHash)
on target.UserName = source.UserName
when not matched
then insert (UserName, NormalizedUserName, PasswordHash) values (source.UserName, source.NormalizedUserName, source.PasswordHash);