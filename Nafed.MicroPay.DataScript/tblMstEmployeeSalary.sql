  select employeeCode [EmployeeCode Not Found In MonthlyInputTable],[Name] from tblMstEmployee where  DOLeaveOrg is null 
 --  and EmployeeTypeID =5 BranchID  <> 44 and  IsDeleted =0 and and IsSalgenrated =0 
   and not exists(select 1 from TBLMONTHLYINPUT where tblMstEmployee.EmployeeCode = TBLMONTHLYINPUT.EmployeeCode 
    and SalYear=2020 and SalMonth=2 
  )

  select 
  --tblMstEmployee.EmployeeTypeID,  tblMstEmployee.employeeCode [EmployeeCode Not Found In MstSalaryTable],[Name],

Micropay.dbo.TblMstEmployeeSalary.* from tblMstEmployee
left join Micropay.dbo.TblMstEmployeeSalary  on tblMstEmployee.EmployeeCode = TblMstEmployeeSalary.EmployeeCode

where DOLeaveOrg is null   and IsDeleted =0 
  --BranchID  <> 44 and  IsDeleted =0 and    and EmployeeTypeID =5 
 -- and IsSalgenrated =0 
  and not exists(select 1 from TblMstEmployeeSalary where tblMstEmployee.EmployeeCode = TblMstEmployeeSalary.EmployeeCode)  
  and Micropay.dbo.TblMstEmployeeSalary.EmployeeCode is not null


 ---//==== Query to migrate missing employee(s) salary data from Payroll DB to hrms===== 

 insert into  [HRMS].[dbo].[TblMstEmployeeSalary](
[BranchCode]
      ,[EmployeeCode] ,[E_Basic] ,[LastBasic] ,[E_SP] ,[LastIncrement] ,[SPCashALW] ,[SPTelexALW]  ,[SPStenographyALW] ,[SPAccWorkALW]
      ,[SPODupMachineALW]  ,[SPFaxMachineALW] ,[SPAsDriverALW] ,[E_FDA]  ,[E_01] ,[E_02]   ,[E_03]    ,[E_04]   ,[E_05]
      ,[E_06]  ,[E_07]   ,[E_08]   ,[E_09]  ,[SANightWatch]  ,[SADCAllowance]  ,[SAAssamComp] ,[SAFairEPart]   ,[SAToSplPost]
      ,[E_10]  ,[E_11]   ,[E_12]   ,[E_13]  ,[E_14]  ,[E_15] ,[E_16]  ,[E_17]  ,[E_18]  ,[E_19]   ,[E_20]    ,[E_21]
      ,[E_22]  ,[E_23]   ,[E_24]   ,[E_25]  ,[E_26]  ,[E_27] ,[E_28],[E_29],[E_30] ,[D_PF],[D_VPF],[D_01],[D_02],[D_03]
      ,[D_04] ,[D_05] ,[D_06],[D_07],[D_08],[D_09] ,[D_10],[D_11] ,[D_12],[D_13],[D_14] ,[D_15],[D_16],[D_17],[D_18],[D_19]
      ,[D_20] ,[D_21] ,[D_22] ,[D_23],[D_24],[D_25],[D_26] ,[D_27] ,[D_28],[D_29],[D_30]
      ,[ModePay] ,[BankCode] ,[BankAcNo],[HRA],[UnionFee] ,[ProfTax],[NoofChildren] ,[SportClub],[IsRateVPF],[VPFValueRA]
      ,[LastIncrementDate] ,[CCA] ,[WASHING],[None],[DELETEDEMPLOYEE],[IsSalgenrated] ,[E_31] ,[E_32]    
      ,[CreatedBy],BranchID)
	  
	  select *,1,1 from (
	  select 
		Micropay.dbo.TblMstEmployeeSalary.* from tblMstEmployee
		left join Micropay.dbo.TblMstEmployeeSalary  on tblMstEmployee.EmployeeCode = TblMstEmployeeSalary.EmployeeCode

		where DOLeaveOrg is null   and IsDeleted =0 
		  --BranchID  <> 44 and  IsDeleted =0 and    and EmployeeTypeID =5 
		 -- and IsSalgenrated =0 
		  and not exists(select 1 from TblMstEmployeeSalary where tblMstEmployee.EmployeeCode = TblMstEmployeeSalary.EmployeeCode)  
		  and Micropay.dbo.TblMstEmployeeSalary.EmployeeCode is not null) res


		  UPDATE A    
SET EmployeeId = RA.EmployeeId
FROM TblMstEmployeeSalary A
INNER JOIN tblMstEmployee RA
ON A.EmployeeCode = RA.EmployeeCode

UPDATE A    
SET BranchID = RA.BranchID
FROM TblMstEmployeeSalary A
INNER JOIN Branch RA
ON substring(A.BranchCode, patindex('%[^0]%',A.BranchCode), 10) = RA.BranchCode

 -----///====  End===================================================================

 ---/// == Data migration from old micropay to hrms -- fields like - hra, CCA, etc ---- Dated On- 04-may-2020

 select  l.IsPensionDeducted, l.E_02,  1.EmployeeID, l.EmployeeCode, o.EmployeeCode, l.E_Basic, o.E_Basic, o.* from TblMstEmployeeSalary l   left join  Micropay.dbo.TblMstEmployeeSalary o
on l.EmployeeCode = o.EmployeeCode 

update TblMstEmployeeSalary set E_Basic =  o.E_Basic  from TblMstEmployeeSalary l   left join  Micropay.dbo.TblMstEmployeeSalary o
on l.EmployeeCode = o.EmployeeCode where  l.E_Basic <> o.E_Basic

update TblMstEmployeeSalary set HRA = 0 where HRA is null

update TblMstEmployeeSalary set BankCode =  o.Bankcode, BankAcNo = o.BankAcNo, IsSalgenrated = o.IsSalgenrated , IsRateVPF = o.IsRateVPF, VPFValueRA = o.VPFValueRA
, NoofChildren = o.NoofChildren, ModePay = o.ModePay, E_02 = case when o.HRA = 1 then 1 else 0 end , D_07=  case when o.HRA = 1 then 0 else 1 end, 
D_14=  case when o.UnionFee = 1 then 1 else 0 end, 
D_02 =  case when o.ProfTax = 1 then 1 else 0 end, D_06 =  case when o.SportClub = 1 then 1 else 0 end , D_VPF =  case when o.D_VPF > 0 then o.D_VPF else 0 end
, E_05 =  case when o.CCA = 1 then 1 else 0 end  , E_07 =  case when o.WASHING = 1 then 1 else 0 end
from TblMstEmployeeSalary l   left join  MicroPayAfter.dbo.TblMstEmployeeSalary o
on l.EmployeeCode = o.EmployeeCode 

update TblMstEmployeeSalary set  VPFValueRA =  case when o.D_VPF > 0 then o.D_VPF else 0 end
from TblMstEmployeeSalary l   left join  MicroPayAfter.dbo.TblMstEmployeeSalary o
on l.EmployeeCode = o.EmployeeCode 

--=====================end  =========================


---========== Dated On- 05-may-2020=====================
insert into  [HRMS].[dbo].[TblMstEmployeeSalary](
[BranchCode]
      ,[EmployeeCode] ,[E_Basic] ,[LastBasic] ,[E_SP] ,[LastIncrement] ,[SPCashALW] ,[SPTelexALW]  ,[SPStenographyALW] ,[SPAccWorkALW]
      ,[SPODupMachineALW]  ,[SPFaxMachineALW] ,[SPAsDriverALW] ,[E_FDA]  ,[E_01] ,[E_02]   ,[E_03]    ,[E_04]   ,[E_05]
      ,[E_06]  ,[E_07]   ,[E_08]   ,[E_09]  ,[SANightWatch]  ,[SADCAllowance]  ,[SAAssamComp] ,[SAFairEPart]   ,[SAToSplPost]
      ,[E_10]  ,[E_11]   ,[E_12]   ,[E_13]  ,[E_14]  ,[E_15] ,[E_16]  ,[E_17]  ,[E_18]  ,[E_19]   ,[E_20]    ,[E_21]
      ,[E_22]  ,[E_23]   ,[E_24]   ,[E_25]  ,[E_26]  ,[E_27] ,[E_28],[E_29],[E_30] ,[D_PF],[D_VPF],[D_01],[D_02],[D_03]
      ,[D_04] ,[D_05] ,[D_06],[D_07],[D_08],[D_09] ,[D_10],[D_11] ,[D_12],[D_13],[D_14] ,[D_15],[D_16],[D_17],[D_18],[D_19]
      ,[D_20] ,[D_21] ,[D_22] ,[D_23],[D_24],[D_25],[D_26] ,[D_27] ,[D_28],[D_29],[D_30]
      ,[ModePay] ,[BankCode] ,[BankAcNo],[HRA],[UnionFee] ,[ProfTax],[NoofChildren] ,[SportClub],[IsRateVPF],[VPFValueRA]
      ,[LastIncrementDate] ,[CCA] ,[WASHING],[None],[DELETEDEMPLOYEE],[IsSalgenrated] ,[E_31] ,[E_32]    
      ,[CreatedBy],BranchID)

	  SELECT  [BranchCode]
      ,[EmployeeCode]
      ,[E_Basic]
      ,[LastBasic]
      ,[E_SP]
      ,[LastIncrement]
      ,[SPCashALW]
      ,[SPTelexALW]
      ,[SPStenographyALW]
      ,[SPAccWorkALW]
      ,[SPODupMachineALW]
      ,[SPFaxMachineALW]
      ,[SPAsDriverALW]
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
      ,[SANightWatch]
      ,[SADCAllowance]
      ,[SAAssamComp]
      ,[SAFairEPart]
      ,[SAToSplPost]
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
      ,[ModePay]
      ,[BankCode]
      ,[BankAcNo]
      ,[HRA]
      ,[UnionFee]
      ,[ProfTax]
      ,[NoofChildren]
      ,[SportClub]
      ,[IsRateVPF]
      ,[VPFValueRA]
      ,[LastIncrementDate]
      ,[CCA]
      ,[WASHING]
      ,[None]
      ,[DELETEDEMPLOYEE]
      ,[IsSalgenrated]
      ,[E_31]
      ,[E_32],1,1
  FROM [MicroPayB4].[dbo].[TblMstEmployeeSalary]




  update TblMstEmployeeSalary set IsPensionDeducted  = e.PENSIONDEDUCT
--select s.EmployeeCode, e.EmployeeCode , s.IsPensionDeducted, e.PENSIONDEDUCT
from TblMstEmployeeSalary s inner join MicroPayAfter.dbo.tblMstEmployee e on s.EmployeeCode = e.EmployeeCode and e.PENSIONDEDUCT = 1
  ----============ End==================


