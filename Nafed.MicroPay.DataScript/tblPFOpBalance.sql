

alter table tblPFOpBalance
add EmployeeID int ,
BranchID int ,
CreatedBy int not null default(1),
CreatedOn datetime not null default(getdate()),
UpdatedBy int ,
UpdatedOn datetime

ALTER TABLE tblPFOpBalance
ADD FOREIGN KEY (EmployeeID) REFERENCES [tblmstEmployee](EmployeeID);

ALTER TABLE tblPFOpBalance
ADD FOREIGN KEY (BranchID) REFERENCES [Branch](BranchID);

ALTER TABLE tblPFOpBalance
ADD FOREIGN KEY (CreatedBy) REFERENCES [User](UserID);

ALTER TABLE tblPFOpBalance
ADD FOREIGN KEY (UpdatedBy) REFERENCES [User](UserID);



UPDATE A    
SET EmployeeId = RA.EmployeeId
FROM tblPFOpBalance A
INNER JOIN tblMstEmployee RA
ON A.EmployeeCode = RA.EmployeeCode

UPDATE A    
SET BranchID = RA.BranchID
FROM tblPFOpBalance A
INNER JOIN Branch RA
ON substring(A.BranchCode, patindex('%[^0]%',A.BranchCode), 10) = RA.BranchCode


alter table tblPFOpBalance
alter column Employeecode varchar(6) not null

alter table tblPFOpBalance
alter column BranchCode varchar(10) 

alter table [tblPFOpBalance]
alter column [Salmonth] tinyint not null

alter table tblPFOpBalance
add 
 CONSTRAINT [PK_tblPFOpBalance] PRIMARY KEY CLUSTERED 
(
	[Employeecode] ASC,
	[PFAcNo] ASC,
	[Salmonth] ASC,
	[Salyear] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]



--- //=        =DatedOn - 05-may-2020 ================

--select * from tblPFOpBalance  where Salmonth =3 and Salyear =2020

insert into tblPFOpBalance([Employeecode]
      ,[PFAcNo]
      ,[Salmonth]
      ,[Salyear]
      ,[BRANCHCODE]
      ,[EmplOpBal]
      ,[EmplrOpBal]
      ,[EmployeePFCont]
      ,[VPF]
      ,[Pension]
      ,[EmployerPFCont]
      ,[EmployeeInterest]
      ,[EmployerInterest]
      ,[InterestNRPFLoan]
      ,[InterestRate]
      ,[NonRefundLoan]
      ,[TotalPFBalance]
      ,[InterestTotal]
      ,[TotalPFOpeningEmpl]
      ,[TotalPFOpeningEmplr]
      ,[WithdrawlEmployeeAc]
      ,[WithdrawlEmployerAc]
      ,[AdditionEmployeeAc]
      ,[AdditionEmployerAc]
      ,[IntWDempl]
      ,[IntWDemplr]
      ,[Reason]
      ,[PF_DAarear]
      ,[VPF_DAarear]
      ,[PF_payarear]
      ,[VPF_payarear]
      ,[PensionDeduct]
      ,[IsInterestCalculate]
      ,[additionEmpInt]
      ,[additionEmployerInt],BranchID,CreatedBy,CreatedOn)


SELECT  [Employeecode]
      ,[PFAcNo]
      ,[Salmonth]
      ,[Salyear]
      ,e.[BRANCHCODE]
      ,[EmplOpBal]
      ,[EmplrOpBal]
      ,[EmployeePFCont]
      ,[VPF]
      ,[Pension]
      ,[EmployerPFCont]
      ,[EmployeeInterest]
      ,[EmployerInterest]
      ,[InterestNRPFLoan]
      ,[InterestRate]
      ,[NonRefundLoan]
      ,[TotalPFBalance]
      ,[InterestTotal]
      ,[TotalPFOpeningEmpl]
      ,[TotalPFOpeningEmplr]
      ,[WithdrawlEmployeeAc]
      ,[WithdrawlEmployerAc]
      ,[AdditionEmployeeAc]
      ,[AdditionEmployerAc]
      ,[IntWDempl]
      ,[IntWDemplr]
      ,[Reason]
      ,[PF_DAarear]
      ,[VPF_DAarear]
      ,[PF_payarear]
      ,[VPF_payarear]
      ,[PensionDeduct]
      ,[IsInterestCalculate]
      ,[additionEmpInt]
      ,[additionEmployerInt],Branch.BranchID,1,getdate()
  FROM [MicroPayB4].[dbo].[tblPFOpBalance]  
  where e.Salmonth =3 and e.Salyear =2020

  --============ End =======


  Create Proc GetEmpPFOpBalance(@salMonth TINYINT , @salYear SMALLINT,@tblEmpCode [dbo].[udtGenericStringList]  READONLY)
AS
BEGIN
   
    Select * from tblPFOpBalance  where Salyear= @salYear AND Salmonth = @salMonth 
	AND EXISTS (SELECT 1 FROM @tblEmpCode EC WHERE tblPFOpBalance.Employeecode= EC.[VALUE])

END