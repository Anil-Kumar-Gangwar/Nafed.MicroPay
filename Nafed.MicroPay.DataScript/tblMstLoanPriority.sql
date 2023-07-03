--select   * from  MicroPayB4.dbo.tblMstLoanPriority 

insert into tblMstLoanPriority([SerialNo]
      ,[Branchcode]
      ,[PriorityNo]
      ,[LoanType]
      ,[EmpCode]
      ,[DateofApp]
      ,[DateRcptApp]
      ,[ReqAmt]
      ,[Surety]
      ,[LoanSanc]
      ,[DateofSanc]
      ,[SancAmt]
      ,[DateAvailLoan]
      ,[EffDate]
      ,[Reasonref]
      ,[Dateofref]
      ,[MaxLoanAmt]
      ,[ROI]
      ,[Asubmitted]
      ,[Bsubmitted]
      ,[Csubmitted]
      ,[Dsubmitted]
      ,[Esubmitted]
      ,[Fsubmitted]
      ,[Gsubmitted]
      ,[Hsubmitted]
      ,[Detail]
      ,[LoanMode]
      ,[IsFloatingRate]
      ,[IsInterestPayable]
      ,[OriginalPInstNo]
      ,[OriginalIInstNo]
      ,[OriginalPinstAmt]
      ,[InterestInstAmt]
      ,[BalancePAmt]
      ,[BalanceIAmt]
      ,[TotalBalanceAmt]
      ,[RemainingPInstNo]
      ,[RemainingIInstNo]
      ,[LastPaidInstDeduDt]
      ,[LastPaidPInstAmt]
      ,[LastPaidIInstAmt]
      ,[LastPaidPInstNo]
      ,[LastPaidIInstNo]
      ,[LastMonthInterest]
      ,[TotalSkippedInst]
      ,[CurrentROI]
      ,[IsNewLoanAfterDevelop]
      ,[Status]
      ,[LastInstAmt]
      ,[LastPAmt]
      ,[AdjustedSancAmt]
      ,[withdrawlAmt]
      ,[WithdrawlInterestAmt]
      ,[Wdate]
      ,[Rmonth]
      ,[Ryear]
      ,[PARTREFUND]
      ,[PARTREFUNDINT],BranchID,CreatedBy,CreatedOn,LoanTypeId)
/****** Script for SelectTopNRows command from SSMS  ******/
SELECT  [SerialNo]
      ,e.[Branchcode]
      ,[PriorityNo]
      ,e.[LoanType]
      ,[EmpCode]
      ,[DateofApp]
      ,[DateRcptApp]
      ,[ReqAmt]
      ,[Surety]
      , CASE WHEN [LoanSanc]='Y' THEN 1 ELSE 0 END  
      ,[DateofSanc]
      ,[SancAmt]
      ,[DateAvailLoan]
      ,[EffDate]
      ,[Reasonref]
      ,[Dateofref]
      ,[MaxLoanAmt]
      ,[ROI]
      , CASE WHEN Asubmitted='Y' THEN 1 ELSE 0 END   
      ,CASE WHEN Bsubmitted='Y' THEN 1 ELSE 0 END
      ,CASE WHEN Csubmitted='Y' THEN 1 ELSE 0 END
      ,CASE WHEN Dsubmitted='Y' THEN 1 ELSE 0 END
      ,CASE WHEN Esubmitted='Y' THEN 1 ELSE 0 END
      ,CASE WHEN Fsubmitted='Y' THEN 1 ELSE 0 END
      ,CASE WHEN Gsubmitted='Y' THEN 1 ELSE 0 END
      ,CASE WHEN Hsubmitted='Y' THEN 1 ELSE 0 END
      ,[Detail]
      ,[LoanMode]
      ,[IsFloatingRate]
      ,[IsInterestPayable]
      ,[OriginalPInstNo]
      ,[OriginalIInstNo]
      ,[OriginalPinstAmt]
      ,[InterestInstAmt]
      ,[BalancePAmt]
      ,[BalanceIAmt]
      ,[TotalBalanceAmt]
      ,[RemainingPInstNo]
      ,[RemainingIInstNo]
      ,[LastPaidInstDeduDt]
      ,[LastPaidPInstAmt]
      ,[LastPaidIInstAmt]
      ,[LastPaidPInstNo]
      ,[LastPaidIInstNo]
      ,[LastMonthInterest]
      ,[TotalSkippedInst]
      ,[CurrentROI]
      ,[IsNewLoanAfterDevelop]
      ,[Status]
      ,[LastInstAmt]
      ,[LastPAmt]
      ,[AdjustedSancAmt]
      ,[withdrawlAmt]
      ,[WithdrawlInterestAmt]
      ,[Wdate]
      ,[Rmonth]
      ,[Ryear]
      ,[PARTREFUND]
      ,[PARTREFUNDINT], Branch.BranchID, 1, getdate(),   tblMstLoanType.LoanTypeId
  FROM [MicroPayB4].[dbo].[tblMstLoanPriority] e inner join Branch on e.BranchCode = Branch.BranchCode  
  inner join tblMstLoanType on e.LoanType = tblMstLoanType.LoanType



--  update tblMstLoanPriority SET Asubmitted=CASE WHEN Asubmitted='Y' THEN 1 ELSE 0 END
--update tblMstLoanPriority SET Bsubmitted=CASE WHEN Bsubmitted='Y' THEN 1 ELSE 0 END
--update tblMstLoanPriority SET Csubmitted=CASE WHEN Csubmitted='Y' THEN 1 ELSE 0 END
--update tblMstLoanPriority SET Dsubmitted=CASE WHEN Dsubmitted='Y' THEN 1 ELSE 0 END
--update tblMstLoanPriority SET Esubmitted=CASE WHEN Esubmitted='Y' THEN 1 ELSE 0 END
--update tblMstLoanPriority SET Fsubmitted=CASE WHEN Fsubmitted='Y' THEN 1 ELSE 0 END
--update tblMstLoanPriority SET Gsubmitted=CASE WHEN Gsubmitted='Y' THEN 1 ELSE 0 END
--update tblMstLoanPriority SET Hsubmitted=CASE WHEN Hsubmitted='Y' THEN 1 ELSE 0 END