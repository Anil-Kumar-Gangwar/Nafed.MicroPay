 declare @table [dbo].[udtGenericStringList]
  --insert into @table(VALUE) values (0)
  insert into @table(VALUE) values (2)
  insert into @table(VALUE) values (3)
  --insert into @table(VALUE) values (4)

  declare @defaultFieldID int =12

   declare @COLnAME [dbo].[udtSelectListModel]
insert into @COLnAME values (1,'BranchName')
  insert into @COLnAME values (2,'ACR_No')
  insert into @COLnAME values (3,'PFNO')
  insert into @COLnAME values (4,'TitleName')
  insert into @COLnAME values (5,'EL')


  declare @displayTable [dbo].[udtSelectListModel]
 insert into @displayTable values (1,'Branch Name')
  insert into @displayTable values (2,'ACR')
  insert into @displayTable values (3,'PF No.')
  insert into @displayTable values (4,'Title')
  insert into @displayTable values (5,'EL')


    declare @PAYsCALE [dbo].[udtSelectListModel]
    insert into @PAYsCALE values (1,'Branch Name')
 
  exec GetAdvanceSearchResult @defaultFieldID,0,'02/10/2012','01/28/2020',@table,@PAYsCALE,@COLnAME,@displayTable