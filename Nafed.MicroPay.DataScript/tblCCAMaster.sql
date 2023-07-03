alter table tblCCAMaster 
add AmtCityGradeC money


insert into tblCCAMaster(UpperLimit,AmtCityGradeA1,AmtCityGradeA,AmtCityGradeB1,AmtCityGradeB2,AmtCityGradeC,CreatedBy)
select UpperLimit,AmtCityGradeA1,AmtCityGradeA,AmtCityGradeB1,AmtCityGradeB2,AmtCityGradeC,1 from MicroPay.dbo.tblCCAMaster