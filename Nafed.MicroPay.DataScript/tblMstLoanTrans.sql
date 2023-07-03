alter table [dbo].[tblLoanTrans]
alter column [EmployeeCode] varchar(6) not null


alter table [dbo].[tblLoanTrans]
alter column BranchCode varchar(10) not null 

alter table [dbo].[tblLoanTrans]
add EmployeeID int ,
BranchID int 


UPDATE A    
SET EmployeeId = RA.EmployeeId
FROM [tblLoanTrans] A
INNER JOIN tblMstEmployee RA
ON A.EmployeeCode = RA.EmployeeCode

UPDATE A    
SET BranchID = RA.BranchID
FROM [tblLoanTrans] A
INNER JOIN Branch RA
ON substring(A.BranchCode, patindex('%[^0]%',A.BranchCode), 10) = RA.BranchCode

alter table [tblLoanTrans]
add CreatedBy int not null default(1) ,
 CreatedOn datetime not null default(getdate()),
 UpdatedBy int,
 UpdatedOn datetime

ALTER TABLE [tblLoanTrans]
ADD FOREIGN KEY (EmployeeID) REFERENCES [tblmstEmployee](EmployeeID);

ALTER TABLE [tblLoanTrans]
ADD FOREIGN KEY (BranchID) REFERENCES [Branch](BranchID);

alter table tblLoanTrans
add LoanTypeID int  null

UPDATE A    
SET LoanTypeId = RA.LoanTypeId
FROM tblLoanTrans A
INNER JOIN tblMstLoanType RA
ON A.LoanType = RA.LoanType

alter table tblLoanTrans
alter column LoanTypeID int not  null

ALTER TABLE [tblLoanTrans]
ADD FOREIGN KEY (LoanTypeID) REFERENCES [tblMstLoanType](LoanTypeID);



-----=== Dated on - 05-May-2020 c============

insert into tblLoanTrans([BranchCode]
      ,[EmployeeCode]
      ,[PriorityNo]
      ,[LoanType]
      ,[SancAmt]
      ,[CurrentROI]
      ,[CurrentPInstNoPaid]
      ,[CurrentPInstAmtPaid]
      ,[RemainingPAmt]
      ,[CurrentInterestAmt]
      ,[CurrentIInstNoPaid]
      ,[CurrentIInstAmtPaid]
      ,[RemainingIAmt]
      ,[PeriodOfPayment]
      ,[SkippedInstNo]
      ,[TDSRebetAmt]
      ,[IsNewLoanAfterDevelop]
      ,[SerialNo]
      ,[CLOSINGBALANCE]
      ,[PDebit]
      ,[PDinterest]
      ,[PCredit]
      ,[PCinterest]
      ,[EntryDate],BranchID,CreatedBy,CreatedOn,LoanTypeId)
SELECT  e.[BranchCode]
      ,[EmployeeCode]
      ,[PriorityNo]
      ,e.[LoanType]
      ,[SancAmt]
      ,[CurrentROI]
      ,[CurrentPInstNoPaid]
      ,[CurrentPInstAmtPaid]
      ,[RemainingPAmt]
      ,[CurrentInterestAmt]
      ,[CurrentIInstNoPaid]
      ,[CurrentIInstAmtPaid]
      ,[RemainingIAmt]
      ,[PeriodOfPayment]
      ,[SkippedInstNo]
      ,[TDSRebetAmt]
      ,[IsNewLoanAfterDevelop]
      ,[SerialNo]
      ,[CLOSINGBALANCE]
      ,[PDebit]
      ,[PDinterest]
      ,[PCredit]
      ,[PCinterest]
      ,[EntryDate],Branch.BranchID,1,getdate(), tblMstLoanType.LoanTypeId
  FROM [MicroPayB4].[dbo].[tblLoanTrans] e inner join Branch on e.BranchCode = Branch.BranchCode  
  inner join tblMstLoanType on e.LoanType = tblMstLoanType.LoanType
  where e.PeriodOfPayment='202003'


  select * from tblLoanTrans where EmployeeCode = '0663' and LoanType = 'D_15' and SerialNo=4087
select * from MicroPayB4.dbo.tblLoanTrans where EmployeeCode = '0663' and LoanType = 'D_15' and SerialNo=4087
select * from MicroPayAfter.dbo.tblLoanTrans where EmployeeCode = '0663' and LoanType = 'D_15' and SerialNo=4087
 


Loanpriority - interestInstAmt , balanceIAmt, LastPaidInstDeduDt

LoanTrans - CurrentInterestAmt, CurrentInstAmtPaid, RemainingIamt, Periodofpayment, 

update TblMstEmployeeSalary set 

E_Basic = o.E_Basic,
BankCode =  o.Bankcode, BankAcNo = o.BankAcNo, IsSalgenrated = o.IsSalgenrated , IsRateVPF = o.IsRateVPF, VPFValueRA = o.VPFValueRA
, NoofChildren = o.NoofChildren, ModePay = o.ModePay, E_02 = case when o.HRA = 1 then 1 else 0 end , D_07=  case when o.HRA = 1 then 0 else 1 end, 
D_14=  case when o.UnionFee = 1 then 1 else 0 end, 
D_02 =  case when o.ProfTax = 1 then 1 else 0 end, D_06 =  case when o.SportClub = 1 then 1 else 0 end , D_VPF =  case when o.D_VPF > 0 then o.D_VPF else 0 end
, E_05 =  case when o.CCA = 1 then 1 else 0 end  , E_07 =  case when o.WASHING = 1 then 1 else 0 end
from TblMstEmployeeSalary l   left join  MicroPayAfter.dbo.TblMstEmployeeSalary o
on l.EmployeeCode = o.EmployeeCode 