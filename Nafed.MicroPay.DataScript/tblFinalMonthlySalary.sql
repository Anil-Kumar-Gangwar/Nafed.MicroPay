
alter table tblFinalMonthlySalary
alter column BANKACNO varchar(16) 

alter table [tblFinalMonthlySalary]
add CreatedBy int ,
CreatedOn datetime not null default(getdate()),
UpdatedBy int,
UpdatedOn datetime,
EmployeeID int ,
BranchID int

update [tblFinalMonthlySalary] set CreatedBy =1

alter table tblFinalMonthlySalary
alter column [EmployeeCode] varchar(6) not null


alter table  tblFinalMonthlySalary
add DesignationID int 


UPDATE A    
SET DesignationID = RA.DesignationID
FROM tblFinalMonthlySalary A
INNER JOIN Designation RA
ON A.DESIGNATIONCODE = RA.DesignationCode


ALTER TABLE tblFinalMonthlySalary
ADD FOREIGN KEY (DesignationID) REFERENCES Designation(DesignationID);
GO

alter table tblFinalMonthlySalary
alter column [BranchCode] varchar(10) not null

Alter Table tblFinalMonthlySalary Alter column SalMonth tinyint not null

ALTER TABLE tblFinalMonthlySalary
ADD FOREIGN KEY (UpdatedBy) REFERENCES [User](userid);
GO

ALTER TABLE tblFinalMonthlySalary
ADD FOREIGN KEY (EmployeeId) REFERENCES tblmstEmployee(EmployeeId);
GO

ALTER TABLE tblFinalMonthlySalary
ADD FOREIGN KEY (BranchId) REFERENCES Branch(BranchId);
GO

ALTER TABLE tblFinalMonthlySalary
ADD FOREIGN KEY (CreatedBy) REFERENCES [User](userid);
GO

UPDATE A    
SET EmployeeId = RA.EmployeeId
FROM tblFinalMonthlySalary A
INNER JOIN tblMstEmployee RA
ON A.EmployeeCode = RA.EmployeeCode

UPDATE A    
SET BranchID = RA.BranchID
FROM tblFinalMonthlySalary A
INNER JOIN Branch RA
ON substring(A.BranchCode, patindex('%[^0]%',A.BranchCode), 10) = RA.BranchCode


ALTER TABLE [dbo].[tblFinalMonthlySalary] ADD  CONSTRAINT [PK_tblFinalMonthlySalary] PRIMARY KEY CLUSTERED 
(
	[BranchCode] ASC,
	[SalMonth] ASC,
	[SalYear] ASC,
	[RecordType] ASC,
	[SeqNo] ASC,
	[EmployeeCode] ASC,
	[DateofGenerateSalary] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO


update tblGisDeduction set Category=LTRIM(RTRIM(Category))



-----
insert into tblFinalMonthlySalary([BranchCode],[SalMonth]
      ,[SalYear]
      ,[RecordType]
      ,[SeqNo]
      ,[EmployeeCode]
      ,[E_Basic]
      ,[E_SP]
      ,[E_FDA]
      ,[E_01]
      ,[E_02]
      ,[E_03]
      ,[E_04]
      ,[E_05]
      ,[E_06]
      ,[E_07]
      ,[E_08]
      ,[E_09]
      ,[E_10]
      ,[E_11]
      ,[E_12]
      ,[E_13]
      ,[E_14]
      ,[E_15]
      ,[E_16]
      ,[E_17]
      ,[E_18]
      ,[E_19]
      ,[E_20]
      ,[E_21]
      ,[E_22]
      ,[E_23]
      ,[E_24]
      ,[E_25]
      ,[E_26]
      ,[E_27]
      ,[E_28]
      ,[E_29]
      ,[E_30]
      ,[D_PF]
      ,[D_VPF]
      ,[D_01]
      ,[D_02]
      ,[D_03]
      ,[D_04]
      ,[D_05]
      ,[D_06]
      ,[D_07]
      ,[D_08]
      ,[D_09]
      ,[D_10]
      ,[D_11]
      ,[D_12]
      ,[D_13]
      ,[D_14]
      ,[D_15]
      ,[D_16]
      ,[D_17]
      ,[D_18]
      ,[D_19]
      ,[D_20]
      ,[D_21]
      ,[D_22]
      ,[D_23]
      ,[D_24]
      ,[D_25]
      ,[D_26]
      ,[D_27]
      ,[D_28]
      ,[D_29]
      ,[D_30]
      ,[E_Basic_A]
      ,[E_SP_A]
      ,[E_FDA_A]
      ,[E_01_A]
      ,[E_02_A]
      ,[E_03_A]
      ,[E_04_A]
      ,[E_05_A]
      ,[E_06_A]
      ,[E_07_A]
      ,[E_08_A]
      ,[E_09_A]
      ,[E_10_A]
      ,[E_11_A]
      ,[E_12_A]
      ,[E_13_A]
      ,[E_14_A]
      ,[E_15_A]
      ,[E_16_A]
      ,[E_17_A]
      ,[E_18_A]
      ,[E_19_A]
      ,[E_20_A]
      ,[E_21_A]
      ,[E_22_A]
      ,[E_23_A]
      ,[E_24_A]
      ,[E_25_A]
      ,[E_26_A]
      ,[E_27_A]
      ,[E_28_A]
      ,[E_29_A]
      ,[E_30_A]
      ,[D_PF_A]
      ,[D_VPF_A]
      ,[D_01_A]
      ,[D_02_A]
      ,[D_03_A]
      ,[D_04_A]
      ,[D_05_A]
      ,[D_06_A]
      ,[D_07_A]
      ,[D_08_A]
      ,[D_09_A]
      ,[D_10_A]
      ,[D_11_A]
      ,[D_12_A]
      ,[D_13_A]
      ,[D_14_A]
      ,[D_15_A]
      ,[D_16_A]
      ,[D_17_A]
      ,[D_18_A]
      ,[D_19_A]
      ,[D_20_A]
      ,[D_21_A]
      ,[D_22_A]
      ,[D_23_A]
      ,[D_24_A]
      ,[D_25_A]
      ,[D_26_A]
      ,[D_27_A]
      ,[D_28_A]
      ,[D_29_A]
      ,[D_30_A]
      ,[C_TotEarn]
      ,[C_TotDedu]
      ,[C_NetSal]
      ,[C_Pension]
      ,[C_GrossSalary]
      ,[SalaryLock]
      ,[Attendance]
      ,[LWP]
      ,[WorkingDays]
      ,[OTHrs]
      ,[AOTHrs]
      ,[DeductPFLoan]
      ,[DeductNB]
      ,[DeductTCS]
      ,[ELInstallNo]
      ,[GLInstallNo]
      ,[PaidInPeriod]
      ,[DeductHouseLoan]
      ,[DeductFestivalLoan]
      ,[DeductCarLoan]
      ,[DeductScooterLoan]
      ,[DeductSundryAdv]
      ,[chkNegative]
      ,[chkAlwaysNegative]
      ,[PFLoanInstNo]
      ,[HBLoanInstNo]
      ,[FestivalLoanInstNo]
      ,[CarLoanInstNo]
      ,[ScooterLoanInstNo]
      ,[SundryAdvInstNo]
      ,[DateofGenerateSalary]
      ,[PaySlipNo]
      ,[E_BASIC_INC]
      ,[RateADAA]
      ,[RateHRAA]
      ,[RatePFA]
      ,[RateVPFA]
      ,[ArrearType]
      ,[DeductLIC]
      ,[BANKCODE]
      ,[BANKACNO]
      ,[DESIGNATIONCODE]
      ,[E_31]
      ,[E_31_A]
      ,[E_32]
      ,[E_32_A]
      ,[CreatedBy]
      ,[CreatedOn]
	  ,[EmployeeTypeID]
)
select *,1,getdate(),5 from   MicroPayAfter.dbo.tblFinalMonthlySalary where SalMonth=4 and SalYear =2020



----------------------------------------------------
select a.EmployeeCode,b.EmployeeCode from
(select * from TBLMONTHLYINPUT 
where SalMonth =5 and SalYear =2020 and BranchCode ='99'
and EmployeeCode in (select EmployeeCode from tblMstEmployee
where BranchID=44 and IsDeleted =0 and EmployeeTypeID =5
and (DOLeaveOrg is null or  (Month(DOLeaveOrg)=5  and YEAR(DOLeaveOrg)=2020)) and IsSalgenrated =0)
) a
right join 
(
select * from TblMstEmployeeSalary where 
EmployeeCode in(select EmployeeCode from tblMstEmployee
where BranchID=44 and IsDeleted =0 and EmployeeTypeID =5
and (DOLeaveOrg is null or  (Month(DOLeaveOrg)=5  and YEAR(DOLeaveOrg)=2020)) and IsSalgenrated =0
)
) b on a.EmployeeCode=b.EmployeeCode
