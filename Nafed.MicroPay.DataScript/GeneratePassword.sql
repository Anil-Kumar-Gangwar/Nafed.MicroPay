
update [user] set [Password]='7QGkupxcKY60SNRVwKsxdHiY+0lgJp0In1ecMPxE7LdaQxFv/1jx1/wBQ4dSJmjC6OkWNrWrmgCQJ6jOVzklHA==oFQPhDo/t9Ap6iQ/siOrcA==' 
where UserName='admin'

update U1 set u1.[Password] =  
case when res.DesignationID = 27 then 'ETofeZjL5W6E8FLH0muwXYRBlMKx5boSBj4BqyLHQgPSd9VnHzoCer9ID5a666Aa1mkhjX2Imlja+4HnFn1b2w==gqBLUlRpIMewLod27ETLtQ==' 
when res.DesignationID = 309 then 'knk35JlWjX0KGe1STCE7ulg9MXqmtzpMiTyKXAWJDh1PHz1QM8mA9Mr9rE5P4Y/BDSFB5N4jE1Fz83ZCD8brdA==dpOKXViAeV/ZT3JXVxwsmg==' 
when res.DesignationID = 330 then 'vwMACIXP2Tkw2QMEd9YgW7hJm83zIxETrR+WNdWmhxIq5O6AvHMtp6sNh/giZlZz8RRP27hiJrKIl8oO69ZVwQ==C56vC7lsozaM0B4NE+8p9A==' 

else u1.[Password] end 
from [user] U1
inner join
(
select e.DesignationID, u.* from [User] u left join tblmstEmployee e on  u.UserName = e.EmployeeCode 
where u.IsDeleted=0  and e.IsDeleted =0  and e.DesignationID in (27,309,330)) res on U1.UserName = res.UserName 
---// deepakkumarmukhiya@nafed-india.com


--Email Password-    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