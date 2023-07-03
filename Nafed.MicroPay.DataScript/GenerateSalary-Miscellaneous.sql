


---//=== List of employee monthly salary list====  (TblMstEmployeeSalary )
select TblMstEmployeeSalary.EmployeeCode, E_Basic , PFNO, Sen_Code ,TblMstEmployeeSalary.BranchCode,SEN_CODE,pfno from TblMstEmployeeSalary 
inner join tblMstEmployee on TblMstEmployeeSalary.EmployeeCode= tblMstEmployee.EmployeeCode
left join branch on tblMstEmployee.BranchID = branch.BranchID
where tblMstEmployee.EmployeeTypeID =5 and tblMstEmployee.IsDeleted =0 and tblMstEmployee.DOLeaveOrg is null
and tblMstEmployee.BranchID <> 44 
order by branch.BRANCHCODE, convert(numeric,tblMstEmployee.SEN_CODE) desc ,  E_BASIC desc , convert(numeric,tblMstEmployee.pfno) asc  