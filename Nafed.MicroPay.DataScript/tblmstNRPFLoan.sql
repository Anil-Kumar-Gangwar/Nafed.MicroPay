alter table tblmstNRPFLoan
add  EmployeeID int ,
BranchID int

ALTER TABLE tblmstNRPFLoan
ADD FOREIGN KEY (EmployeeID) REFERENCES [tblmstEmployee](EmployeeID);

ALTER TABLE tblmstNRPFLoan
ADD FOREIGN KEY (BranchID) REFERENCES [Branch](BranchID);

ALTER TABLE tblmstNRPFLoan
ADD FOREIGN KEY (CreatedBy) REFERENCES [User](UserID);

ALTER TABLE tblmstNRPFLoan
ADD FOREIGN KEY (UpdatedBy) REFERENCES [User](UserID);