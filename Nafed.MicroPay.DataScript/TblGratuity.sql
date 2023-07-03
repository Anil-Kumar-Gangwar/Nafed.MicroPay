
insert into hrms.dbo.TblGratuity([employeecode]
      ,[Year]
      ,[BranchCode]
      ,[Assurance_Num]
      ,[E_Basic]
      ,[E_SP]
      ,[E_FDA]
      ,[E_01]
      ,[Basic_Total]
      ,[HalfBasic]
      ,[DOJ]
      ,[GenDate]
      ,[CreatedOn]
      ,[CreatedBy])

	  select *,getdate(),1 from dbo.TblGratuity 