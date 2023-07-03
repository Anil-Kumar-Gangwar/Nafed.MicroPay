update  t1
set 
t1.OtaCode = t2.OtaCode 
from tblMstEmployee t1 
inner join Micropay.dbo.tblMstEmployee t2 on t1.EmployeeCode = t2.EmployeeCode


update T

set  t.EmpCatID = resultSet.EmplCatID
from tblMstEmployee t
inner join 

(
select  EmployeeCode, c.EmplCatID,c.EmplCatName from Micropay.dbo.tblMstEmployee e
inner join EmployeeCategory c on e.Category=  c.EmplCatName ) as resultSet  
on resultSet.EmployeeCode = t.EmployeeCode 


alter table tblgisdeduction
alter column [Rate] money


alter table tblmstSlab
alter column IsDeleted bit not null


update  e1
set e1.PFNO = e2.PFNO
from tblMstEmployee e1
inner join ( select * from MicroPayAfter.dbo.tblMstEmployee  )  e2 on e1.EmployeeCode = e2.EmployeeCode
where e1.PFNO is null --and e1.IsDeleted =0  and e2.DeletedEmployee =0 
