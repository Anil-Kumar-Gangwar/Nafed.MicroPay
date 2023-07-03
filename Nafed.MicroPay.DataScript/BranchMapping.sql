/****** Object:  Table [dbo].[BranchMapping]    Script Date: 05-05-2020 18:13:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BranchMapping](
	[BranchCode] [nvarchar](5) NULL,
	[NewBranchCode] [nvarchar](5) NULL
) ON [PRIMARY]
GO
INSERT [dbo].[BranchMapping] ([BranchCode], [NewBranchCode]) VALUES (N'01', N'')
GO
INSERT [dbo].[BranchMapping] ([BranchCode], [NewBranchCode]) VALUES (N'02', N'')
GO
INSERT [dbo].[BranchMapping] ([BranchCode], [NewBranchCode]) VALUES (N'03', N'')
GO
INSERT [dbo].[BranchMapping] ([BranchCode], [NewBranchCode]) VALUES (N'04', N'')
GO
INSERT [dbo].[BranchMapping] ([BranchCode], [NewBranchCode]) VALUES (N'05', N'')
GO
INSERT [dbo].[BranchMapping] ([BranchCode], [NewBranchCode]) VALUES (N'06', N'')
GO
INSERT [dbo].[BranchMapping] ([BranchCode], [NewBranchCode]) VALUES (N'07', N'')
GO
INSERT [dbo].[BranchMapping] ([BranchCode], [NewBranchCode]) VALUES (N'08', N'')
GO
INSERT [dbo].[BranchMapping] ([BranchCode], [NewBranchCode]) VALUES (N'09', N'')
GO
INSERT [dbo].[BranchMapping] ([BranchCode], [NewBranchCode]) VALUES (N'10', N'')
GO
INSERT [dbo].[BranchMapping] ([BranchCode], [NewBranchCode]) VALUES (N'11', N'')
GO
INSERT [dbo].[BranchMapping] ([BranchCode], [NewBranchCode]) VALUES (N'12', N'')
GO
INSERT [dbo].[BranchMapping] ([BranchCode], [NewBranchCode]) VALUES (N'13', N'')
GO
INSERT [dbo].[BranchMapping] ([BranchCode], [NewBranchCode]) VALUES (N'15', N'')
GO
INSERT [dbo].[BranchMapping] ([BranchCode], [NewBranchCode]) VALUES (N'16', N'')
GO
INSERT [dbo].[BranchMapping] ([BranchCode], [NewBranchCode]) VALUES (N'18', N'')
GO
INSERT [dbo].[BranchMapping] ([BranchCode], [NewBranchCode]) VALUES (N'19', N'')
GO
INSERT [dbo].[BranchMapping] ([BranchCode], [NewBranchCode]) VALUES (N'20', N'')
GO
INSERT [dbo].[BranchMapping] ([BranchCode], [NewBranchCode]) VALUES (N'21', N'')
GO
INSERT [dbo].[BranchMapping] ([BranchCode], [NewBranchCode]) VALUES (N'22', N'')
GO
INSERT [dbo].[BranchMapping] ([BranchCode], [NewBranchCode]) VALUES (N'23', N'')
GO
INSERT [dbo].[BranchMapping] ([BranchCode], [NewBranchCode]) VALUES (N'24', N'')
GO
INSERT [dbo].[BranchMapping] ([BranchCode], [NewBranchCode]) VALUES (N'25', N'')
GO
INSERT [dbo].[BranchMapping] ([BranchCode], [NewBranchCode]) VALUES (N'26', N'')
GO
INSERT [dbo].[BranchMapping] ([BranchCode], [NewBranchCode]) VALUES (N'27', N'')
GO
INSERT [dbo].[BranchMapping] ([BranchCode], [NewBranchCode]) VALUES (N'31', N'')
GO
INSERT [dbo].[BranchMapping] ([BranchCode], [NewBranchCode]) VALUES (N'32', N'')
GO
INSERT [dbo].[BranchMapping] ([BranchCode], [NewBranchCode]) VALUES (N'33', N'')
GO
INSERT [dbo].[BranchMapping] ([BranchCode], [NewBranchCode]) VALUES (N'34', N'')
GO
INSERT [dbo].[BranchMapping] ([BranchCode], [NewBranchCode]) VALUES (N'35', N'')
GO
INSERT [dbo].[BranchMapping] ([BranchCode], [NewBranchCode]) VALUES (N'37', N'')
GO
INSERT [dbo].[BranchMapping] ([BranchCode], [NewBranchCode]) VALUES (N'38', N'36')
GO
INSERT [dbo].[BranchMapping] ([BranchCode], [NewBranchCode]) VALUES (N'40', N'')
GO
INSERT [dbo].[BranchMapping] ([BranchCode], [NewBranchCode]) VALUES (N'41', N'')
GO
INSERT [dbo].[BranchMapping] ([BranchCode], [NewBranchCode]) VALUES (N'42', N'')
GO
INSERT [dbo].[BranchMapping] ([BranchCode], [NewBranchCode]) VALUES (N'43', N'')
GO
INSERT [dbo].[BranchMapping] ([BranchCode], [NewBranchCode]) VALUES (N'44', N'')
GO
INSERT [dbo].[BranchMapping] ([BranchCode], [NewBranchCode]) VALUES (N'45', N'')
GO
INSERT [dbo].[BranchMapping] ([BranchCode], [NewBranchCode]) VALUES (N'46', N'')
GO
INSERT [dbo].[BranchMapping] ([BranchCode], [NewBranchCode]) VALUES (N'47', N'')
GO
INSERT [dbo].[BranchMapping] ([BranchCode], [NewBranchCode]) VALUES (N'48', N'')
GO
INSERT [dbo].[BranchMapping] ([BranchCode], [NewBranchCode]) VALUES (N'70', N'289')
GO
INSERT [dbo].[BranchMapping] ([BranchCode], [NewBranchCode]) VALUES (N'71', N'39')
GO
INSERT [dbo].[BranchMapping] ([BranchCode], [NewBranchCode]) VALUES (N'72', N'38')
GO
INSERT [dbo].[BranchMapping] ([BranchCode], [NewBranchCode]) VALUES (N'73', N'163')
GO
INSERT [dbo].[BranchMapping] ([BranchCode], [NewBranchCode]) VALUES (N'74', N'236')
GO
INSERT [dbo].[BranchMapping] ([BranchCode], [NewBranchCode]) VALUES (N'75', N'118')
GO
INSERT [dbo].[BranchMapping] ([BranchCode], [NewBranchCode]) VALUES (N'76', N'')
GO
INSERT [dbo].[BranchMapping] ([BranchCode], [NewBranchCode]) VALUES (N'77', N'')
GO
INSERT [dbo].[BranchMapping] ([BranchCode], [NewBranchCode]) VALUES (N'78', N'')
GO
INSERT [dbo].[BranchMapping] ([BranchCode], [NewBranchCode]) VALUES (N'99', N'')
GO



--------------------- Update Branch Code ------------------------------
update Branch
set BranchCode='0' + BranchCode 
where len(BranchCode)=1