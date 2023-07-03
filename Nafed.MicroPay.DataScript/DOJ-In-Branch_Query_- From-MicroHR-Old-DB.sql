select distinct emp.branchcode as 'Branch Code',emp.employeecode as 'Employee Code',
emp.name as 'Employee Name',emp.DesignationCode as 'Designation Code',
emp.emptype as 'Employee Type' ,REPLACE(CONVERT(varchar(50), 
Convert(datetime,CONVERT(datetime,RIGHT(t.bdate,4)+SUBSTRING(t.bdate,4,2)+LEFT(t.bdate,2))), 106), ' ', '-') as 'DOJ In Branch' 
from  tblMstEmployee emp left join tblMstBranch b 
on  emp.branchcode=b.branchcode left join tblmstloanpriority l 
on emp.employeecode=l.Empcode left join tblMstLoanType LT on l.loantype=LT.loantype
left join tblmstemployeesalary es  on emp.employeecode=es.employeecode 
left join tblLeaveBal tblLB  on emp.employeecode=tblLB.empcode 
left join (select employeecode,convert(varchar,max(fromdate),103) as bdate 
from tblmsttransfer group by employeecode) t on t.employeecode=emp.employeecode 
where emp.employeecode is not null and doleaveorg is null and emptype = 'R' and tblLB.LeaveYear ='2020' 
order by emp.branchcode,emp.employeecode

select * from tblmsttransfer


 declare @table [dbo].[udtGenericStringList]
  insert into @table(VALUE) values (1)
  insert into @table(VALUE) values (2)
  insert into @table(VALUE) values (3)
  insert into @table(VALUE) values (4)
  declare @defaultFieldID int =3

   declare @COLnAME [dbo].[udtSelectListModel]
insert into @COLnAME values (1,'BranchName')
  insert into @COLnAME values (2,'ACR_No')
  insert into @COLnAME values (3,'PFNO')
  insert into @COLnAME values (4,'TitleName')
  insert into @COLnAME values (5,'QAcademicID')
   insert into @COLnAME values (6,'QProfessionalID')

  declare @Payscale [dbo].[udtSelectListModel]

  declare @displayTable [dbo].[udtSelectListModel]
 insert into @displayTable values (1,'Branch Name')
  insert into @displayTable values (2,'ACR')
  insert into @displayTable values (3,'PF No.')
  insert into @displayTable values (4,'Title')
  insert into @displayTable values (5,'Academic')
  insert into @displayTable values (6,'Professional')


  exec GetAdvanceSearchResult @defaultFieldID,1,@table,@Payscale,@COLnAME,@displayTable
