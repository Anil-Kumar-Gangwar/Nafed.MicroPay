alter table SalaryHeads
add EmployeeTypeID int 

ALTER TABLE [dbo].[SalaryHeads]  WITH CHECK ADD  CONSTRAINT [FK_SalaryHeads_EmployeeType] FOREIGN KEY([EmployeeTypeID])
REFERENCES [dbo].[EmployeeType] ([EmployeeTypeID])
GO

ALTER TABLE [dbo].[SalaryHeads] CHECK CONSTRAINT [FK_SalaryHeads_EmployeeType]
GO

alter table salaryheadshistory
add EmployeeTypeID int 

ALTER TABLE [dbo].[salaryheadshistory]  WITH CHECK ADD  CONSTRAINT [FK_salaryheadshistory_EmployeeType] FOREIGN KEY([EmployeeTypeID])
REFERENCES [dbo].[EmployeeType] ([EmployeeTypeID])
GO

alter table salaryheadshistory
alter column [LookUpHead] varchar(600)

alter table salaryheadshistory
alter column [LookUpHeadName] varchar(600)


 alter table salaryheadshistory
add FixedValueFormula bit ,
FixedValue numeric(18,2),
Slab bit null,
LowerRange numeric(18,2),
UpperRange numeric(18,2)

insert into salaryheadshistory([FieldName]
      ,[FieldDesc],[Abbreviation],[FormulaColumn],[LookUpHead] ,[RoundingUpto],[AttendanceDep]
      ,[RoundToHigher] ,[MonthlyInput],[SeqNo],[LookUpHeadName] ,[ActiveField],[SpecialField]
      ,[SpecialFieldMaster],[FromMaster],[LoanHead],[Conditional],[MT],[C]
      ,[DW],[A] ,[CT],[DC],[CW] ,[Period],[CreatedOn],[CreatedBy])

select *,getdate(),1 from Micropay.dbo.salaryheadshistory



update s1
set s1.LookUpHead =s2.HeadValueName , s1.LookUpHeadName=  s2.HeadValue
from salaryheadshistory s1 
inner join Micropay.dbo.TblHeadLookUpHistory s2 on s1.FieldName = s2.FieldName and s1.[Period]= s2.[Period] 





  alter table [BranchSalaryHeadRules] 
  add EmployeeTypeID int 

  alter table SalaryHeads
alter column EmployeeTypeID int not null

-----//===== Query to set Salary Head Rules for non regular employee (Employee Type =7 ) Deputation Case === 27-may-2020
truncate table SalaryHeads

insert into SalaryHeads([FieldName],[FieldDesc],[Abbreviation],[FormulaColumn]
      ,[LookUpHead],[RoundingUpto],[AttendanceDep],[RoundToHigher],[MonthlyInput]
      ,[SeqNo],[LookUpHeadName],[ActiveField],[SpecialField],[SpecialFieldMaster]
      ,[FromMaster],[LoanHead],[Conditional],[MT],[C],[DW],[A],[CT],[DC],[CW],[FixedValueFormula]
      ,[FixedValue] ,[Slab],[CreatedOn] ,[CreatedBy],[UpdatedOn] ,[UpdatedBy],[IsDeleted],[LocationDependent]
      ,[LowerRange],[UpperRange]  ,[CheckHeadInEmpSalTable],EmployeeTypeID)

	  select [FieldName],[FieldDesc],[Abbreviation],[FormulaColumn],[LookUpHead],[RoundingUpto]
      ,[AttendanceDep],[RoundToHigher],[MonthlyInput],[SeqNo],[LookUpHeadName],[ActiveField]
      ,[SpecialField],[SpecialFieldMaster],[FromMaster],[LoanHead],[Conditional],[MT]
      ,[C],[DW] ,[A],[CT] ,[DC],[CW],[FixedValueFormula],[FixedValue],[Slab]
      ,[CreatedOn],[CreatedBy],[UpdatedOn],[UpdatedBy],[IsDeleted]
      ,[LocationDependent] ,[LowerRange],[UpperRange]     
      ,[CheckHeadInEmpSalTable],7 from Micropay.dbo.SalaryHeads 

update  s1
set s1.UpperRange = s2.UpperRange , s1.LowerRange =s2.LowerRange
from SalaryHeads s1
  inner join Micropay.dbo.TblHeadLookup s2 on s1.FieldName = s2.FieldName
  