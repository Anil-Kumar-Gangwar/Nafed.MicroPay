USE [master]
GO
/****** Object:  Database [MicroPay]    Script Date: 22/07/2019 17:55:18 ******/
CREATE DATABASE [MicroPay]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'MicroPayNew', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL12.VAIBHAVSQL\MSSQL\DATA\MicroPayNew.mdf' , SIZE = 4096KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'MicroPayNew_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL12.VAIBHAVSQL\MSSQL\DATA\MicroPayNew_log.ldf' , SIZE = 3840KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO
ALTER DATABASE [MicroPay] SET COMPATIBILITY_LEVEL = 120
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [MicroPay].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [MicroPay] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [MicroPay] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [MicroPay] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [MicroPay] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [MicroPay] SET ARITHABORT OFF 
GO
ALTER DATABASE [MicroPay] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [MicroPay] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [MicroPay] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [MicroPay] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [MicroPay] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [MicroPay] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [MicroPay] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [MicroPay] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [MicroPay] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [MicroPay] SET  DISABLE_BROKER 
GO
ALTER DATABASE [MicroPay] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [MicroPay] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [MicroPay] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [MicroPay] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [MicroPay] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [MicroPay] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [MicroPay] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [MicroPay] SET RECOVERY FULL 
GO
ALTER DATABASE [MicroPay] SET  MULTI_USER 
GO
ALTER DATABASE [MicroPay] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [MicroPay] SET DB_CHAINING OFF 
GO
ALTER DATABASE [MicroPay] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [MicroPay] SET TARGET_RECOVERY_TIME = 0 SECONDS 
GO
ALTER DATABASE [MicroPay] SET DELAYED_DURABILITY = DISABLED 
GO
EXEC sys.sp_db_vardecimal_storage_format N'MicroPay', N'ON'
GO
USE [MicroPay]
GO
/****** Object:  User [mw]    Script Date: 22/07/2019 17:55:19 ******/
CREATE USER [mw] FOR LOGIN [mw] WITH DEFAULT_SCHEMA=[dbo]
GO
/****** Object:  UserDefinedTableType [dbo].[DepartmentRights]    Script Date: 22/07/2019 17:55:19 ******/
CREATE TYPE [dbo].[DepartmentRights] AS TABLE(
	[DepartmentID] [int] NULL,
	[MenuID] [int] NULL,
	[ShowMenu] [bit] NULL
)
GO
/****** Object:  UserDefinedTableType [dbo].[UserRights]    Script Date: 22/07/2019 17:55:19 ******/
CREATE TYPE [dbo].[UserRights] AS TABLE(
	[UserID] [int] NULL,
	[DepartmentID] [int] NULL,
	[MenuID] [int] NULL,
	[ShowMenu] [bit] NULL,
	[CreateRight] [bit] NULL,
	[ViewRight] [bit] NULL,
	[EditRight] [bit] NULL,
	[DeleteRight] [bit] NULL
)
GO
/****** Object:  Table [dbo].[ArrearDetail]    Script Date: 22/07/2019 17:55:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ArrearDetail](
	[BranchCode] [varchar](5) NOT NULL,
	[SalMonth] [varchar](2) NOT NULL,
	[SalYear] [smallint] NOT NULL,
	[SeqNo] [int] NOT NULL,
	[EmployeeCode] [varchar](10) NOT NULL,
	[E_Basic] [money] NULL,
	[E_SP] [money] NULL,
	[E_FDA] [money] NULL,
	[E_01] [money] NULL,
	[E_02] [money] NULL,
	[E_03] [money] NULL,
	[E_04] [money] NULL,
	[E_05] [money] NULL,
	[E_06] [money] NULL,
	[E_07] [money] NULL,
	[E_08] [money] NULL,
	[E_09] [money] NULL,
	[E_10] [money] NULL,
	[E_11] [money] NULL,
	[E_12] [money] NULL,
	[E_13] [money] NULL,
	[E_14] [money] NULL,
	[E_15] [money] NULL,
	[E_16] [money] NULL,
	[E_17] [money] NULL,
	[E_18] [money] NULL,
	[E_19] [money] NULL,
	[E_20] [money] NULL,
	[E_21] [money] NULL,
	[E_22] [money] NULL,
	[E_23] [money] NULL,
	[E_24] [money] NULL,
	[E_25] [money] NULL,
	[E_26] [money] NULL,
	[E_27] [money] NULL,
	[E_28] [money] NULL,
	[E_29] [money] NULL,
	[E_30] [money] NULL,
	[D_PF] [money] NULL,
	[D_VPF] [money] NULL,
	[D_01] [money] NULL,
	[D_02] [money] NULL,
	[D_03] [money] NULL,
	[D_04] [money] NULL,
	[D_05] [money] NULL,
	[D_06] [money] NULL,
	[D_07] [money] NULL,
	[D_08] [money] NULL,
	[D_09] [money] NULL,
	[D_10] [money] NULL,
	[D_11] [money] NULL,
	[D_12] [money] NULL,
	[D_13] [money] NULL,
	[D_14] [money] NULL,
	[D_15] [money] NULL,
	[D_16] [money] NULL,
	[D_17] [money] NULL,
	[D_18] [money] NULL,
	[D_19] [money] NULL,
	[D_20] [money] NULL,
	[D_21] [money] NULL,
	[D_22] [money] NULL,
	[D_23] [money] NULL,
	[D_24] [money] NULL,
	[D_25] [money] NULL,
	[D_26] [money] NULL,
	[D_27] [money] NULL,
	[D_28] [money] NULL,
	[D_29] [money] NULL,
	[D_30] [money] NULL,
	[DateOfGenerateArrear] [varchar](15) NOT NULL,
	[ArrearType] [varchar](1) NULL,
	[E_BASIC_INC] [float] NULL,
	[RateADAA] [float] NULL,
	[RateHRAA] [float] NULL,
	[RatePFA] [float] NULL,
	[sum_e_01] [money] NULL,
	[TOTDAYS_LWPDAYS] [varchar](50) NULL,
	[NOOFMONTH] [int] NULL,
	[OLD_DA] [money] NULL,
	[Sequence] [numeric](18, 0) NULL,
	[DateOfGenerate] [datetime] NULL,
	[PeriodFrom] [int] NULL,
	[PeriodTo] [int] NULL,
	[Formula] [nvarchar](250) NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Branch]    Script Date: 22/07/2019 17:55:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Branch](
	[BranchID] [int] IDENTITY(1,1) NOT NULL,
	[BranchTypeID] [int] NOT NULL,
	[BranchCode] [varchar](10) NOT NULL,
	[BranchName] [varchar](100) NOT NULL,
	[IsHillComp] [bit] NOT NULL,
	[Address1] [varchar](50) NOT NULL,
	[Address2] [varchar](50) NULL,
	[Address3] [varchar](50) NULL,
	[Pin] [varchar](6) NULL,
	[CityID] [int] NULL,
	[GradeID] [int] NULL,
	[Region] [varchar](20) NULL,
	[PhoneSTD] [varchar](15) NULL,
	[PhoneNo] [varchar](15) NULL,
	[FaxSTD] [varchar](15) NULL,
	[FaxNo] [varchar](15) NULL,
	[Remarks] [varchar](50) NULL,
	[IsDeleted] [bit] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[CreatedBy] [int] NOT NULL,
	[UpdatedOn] [datetime] NULL,
	[UpdatedBy] [int] NULL,
 CONSTRAINT [PK_Branch] PRIMARY KEY CLUSTERED 
(
	[BranchID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Cadre]    Script Date: 22/07/2019 17:55:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Cadre](
	[CadreID] [int] IDENTITY(1,1) NOT NULL,
	[CadreCode] [varchar](3) NOT NULL,
	[CadreName] [varchar](50) NULL,
 CONSTRAINT [PK_Cadre] PRIMARY KEY CLUSTERED 
(
	[CadreID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[City]    Script Date: 22/07/2019 17:55:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[City](
	[StateID] [int] NOT NULL,
	[CityID] [int] NOT NULL,
	[CityName] [varchar](50) NULL,
 CONSTRAINT [PK_City] PRIMARY KEY CLUSTERED 
(
	[StateID] ASC,
	[CityID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Department]    Script Date: 22/07/2019 17:55:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Department](
	[DepartmentID] [int] IDENTITY(1,1) NOT NULL,
	[DepartmentCode] [varchar](20) NOT NULL,
	[DepartmentName] [varchar](50) NULL,
 CONSTRAINT [PK_TblMstDepartment] PRIMARY KEY CLUSTERED 
(
	[DepartmentID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[DepartmentRights]    Script Date: 22/07/2019 17:55:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DepartmentRights](
	[DepartmentID] [int] NOT NULL,
	[MenuID] [int] NOT NULL,
	[ShowMenu] [bit] NOT NULL CONSTRAINT [DF_mstuserright_ShowMenu]  DEFAULT ((1)),
 CONSTRAINT [PK_mstuserright] PRIMARY KEY CLUSTERED 
(
	[DepartmentID] ASC,
	[MenuID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Designation]    Script Date: 22/07/2019 17:55:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Designation](
	[DesignationID] [int] IDENTITY(1,1) NOT NULL,
	[DesignationCode] [varchar](4) NOT NULL,
	[DesignationName] [varchar](50) NOT NULL,
	[Level] [varchar](3) NULL,
	[IsOfficer] [bit] NOT NULL,
	[Rank] [tinyint] NULL,
	[CadreID] [int] NULL,
	[Cateogry] [varchar](50) NULL,
	[LCT] [numeric](18, 0) NULL,
	[Promotion] [numeric](18, 0) NULL,
	[Direct] [numeric](18, 0) NULL,
	[IsUpgradedDesignation] [bit] NULL,
	[IsDeleted] [bit] NOT NULL,
	[B1] [numeric](18, 0) NULL,
	[E1] [numeric](18, 0) NULL,
	[B2] [numeric](18, 0) NULL,
	[E2] [numeric](18, 0) NULL,
	[B3] [numeric](18, 0) NULL,
	[E3] [numeric](18, 0) NULL,
	[B4] [numeric](18, 0) NULL,
	[E4] [numeric](18, 0) NULL,
	[B5] [numeric](18, 0) NULL,
	[E5] [numeric](18, 0) NULL,
	[B6] [numeric](18, 0) NULL,
	[E6] [numeric](18, 0) NULL,
	[B7] [numeric](18, 0) NULL,
	[E7] [numeric](18, 0) NULL,
	[B8] [numeric](18, 0) NULL,
	[E8] [numeric](18, 0) NULL,
	[B9] [numeric](18, 0) NULL,
	[E9] [numeric](18, 0) NULL,
	[B10] [numeric](18, 0) NULL,
	[E10] [numeric](18, 0) NULL,
	[B11] [numeric](18, 0) NULL,
	[E11] [numeric](18, 0) NULL,
	[B12] [numeric](18, 0) NULL,
	[E12] [numeric](18, 0) NULL,
	[B13] [numeric](18, 0) NULL,
	[E13] [numeric](18, 0) NULL,
	[B14] [numeric](18, 0) NULL,
	[E14] [numeric](18, 0) NULL,
	[B15] [numeric](18, 0) NULL,
	[E15] [numeric](18, 0) NULL,
	[B16] [numeric](18, 0) NULL,
	[E16] [numeric](18, 0) NULL,
	[B17] [numeric](18, 0) NULL,
	[E17] [numeric](18, 0) NULL,
	[B18] [numeric](18, 0) NULL,
	[E18] [numeric](18, 0) NULL,
	[B19] [numeric](18, 0) NULL,
	[E19] [numeric](18, 0) NULL,
	[B20] [numeric](18, 0) NULL,
	[E20] [numeric](18, 0) NULL,
	[B21] [numeric](18, 0) NULL,
	[E21] [numeric](18, 0) NULL,
	[B22] [numeric](18, 0) NULL,
	[E22] [numeric](18, 0) NULL,
	[B23] [numeric](18, 0) NULL,
	[E23] [numeric](18, 0) NULL,
	[B24] [numeric](18, 0) NULL,
	[E24] [numeric](18, 0) NULL,
	[B25] [numeric](18, 0) NULL,
	[E25] [numeric](18, 0) NULL,
	[B26] [numeric](18, 0) NULL,
	[E26] [numeric](18, 0) NULL,
	[B27] [numeric](18, 0) NULL,
	[E27] [numeric](18, 0) NULL,
	[B28] [numeric](18, 0) NULL,
	[E28] [numeric](18, 0) NULL,
	[B29] [numeric](18, 0) NULL,
	[E29] [numeric](18, 0) NULL,
	[B30] [numeric](18, 0) NULL,
	[E30] [numeric](18, 0) NULL,
	[B31] [numeric](18, 0) NULL,
	[E31] [numeric](18, 0) NULL,
	[B32] [numeric](18, 0) NULL,
	[E32] [numeric](18, 0) NULL,
	[B33] [numeric](18, 0) NULL,
	[E33] [numeric](18, 0) NULL,
	[B34] [numeric](18, 0) NULL,
	[E34] [numeric](18, 0) NULL,
	[B35] [numeric](18, 0) NULL,
	[E35] [numeric](18, 0) NULL,
	[B36] [numeric](18, 0) NULL,
	[E36] [numeric](18, 0) NULL,
	[B37] [numeric](18, 0) NULL,
	[E37] [numeric](18, 0) NULL,
	[B38] [numeric](18, 0) NULL,
	[E38] [numeric](18, 0) NULL,
	[B39] [numeric](18, 0) NULL,
	[E39] [numeric](18, 0) NULL,
	[B40] [numeric](18, 0) NULL,
	[E40] [numeric](18, 0) NULL,
 CONSTRAINT [PK_Designation] PRIMARY KEY CLUSTERED 
(
	[DesignationID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Docment]    Script Date: 22/07/2019 17:55:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Docment](
	[DocumentID] [int] IDENTITY(1,1) NOT NULL,
	[DocumentDescription] [varchar](200) NOT NULL,
	[DocumentExtension] [tinyint] NOT NULL,
	[DocumentSize] [nchar](10) NULL,
	[ParentDocumentID] [int] NULL,
	[SequenceNo] [int] NULL,
	[CreatedBy] [int] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[UpdatedBy] [int] NULL,
	[UpdatedOn] [datetime] NULL,
 CONSTRAINT [PK_Docment] PRIMARY KEY CLUSTERED 
(
	[DocumentID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[DocumentExtension]    Script Date: 22/07/2019 17:55:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[DocumentExtension](
	[DocumentExtensionID] [int] NOT NULL,
	[DocumentExtension] [varchar](10) NOT NULL,
	[CreatedBy] [int] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[UpdatedBy] [int] NOT NULL,
	[UpdatedOn] [datetime] NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_DocumentExtension_1] PRIMARY KEY CLUSTERED 
(
	[DocumentExtensionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[EmailConfiguration]    Script Date: 22/07/2019 17:55:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[EmailConfiguration](
	[EmailConfigurationID] [int] IDENTITY(1,1) NOT NULL,
	[ToEmail] [varchar](50) NOT NULL,
	[Bcc] [varchar](50) NULL,
	[CcEmail] [varchar](50) NULL,
	[Password] [varchar](50) NULL,
	[Server] [varchar](50) NOT NULL,
	[Port] [int] NOT NULL,
	[Signature] [varchar](200) NULL,
	[SSLStatus] [bit] NOT NULL,
	[CreatedBy] [int] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[UpdatedBy] [int] NULL,
	[UpdatedOn] [datetime] NULL,
	[IsDeleted] [bit] NOT NULL,
	[MaintenanceDateTime] [datetime] NULL,
	[IsMaintenance] [bit] NOT NULL,
 CONSTRAINT [PK_EmailConfiguration] PRIMARY KEY CLUSTERED 
(
	[EmailConfigurationID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Employee]    Script Date: 22/07/2019 17:55:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Employee](
	[EmployeeCode] [varchar](4) NOT NULL,
	[Title] [varchar](50) NULL,
	[Name] [varchar](40) NULL,
	[GAURDIANNAME] [varchar](30) NULL,
	[EmpType] [varchar](1) NULL,
	[BranchCode] [varchar](2) NULL,
	[DepartmentCode] [varchar](4) NULL,
	[DesignationCode] [varchar](4) NULL,
	[DOB] [datetime] NULL,
	[DOJ] [datetime] NULL,
	[PlaceOfJoin] [varchar](2) NULL,
	[Gender] [char](1) NULL,
	[Category] [varchar](50) NULL,
	[Religion] [varchar](50) NULL,
	[MTongue] [varchar](15) NULL,
	[BGroup] [varchar](3) NULL,
	[ID_Mark] [varchar](50) NULL,
	[MaritalSts] [varchar](1) NULL,
	[EmpCategory] [varchar](1) NULL,
	[PAdd] [varchar](100) NULL,
	[PStreet] [varchar](20) NULL,
	[PCity] [varchar](12) NULL,
	[PPin] [varchar](6) NULL,
	[TelPh] [varchar](15) NULL,
	[PmtAdd] [varchar](100) NULL,
	[PmtStreet] [varchar](20) NULL,
	[PmtCity] [varchar](12) NULL,
	[PmtPin] [varchar](6) NULL,
	[FileNo] [numeric](18, 0) NULL,
	[PFNO] [numeric](18, 0) NULL,
	[Folio_No] [numeric](18, 0) NULL,
	[ACR_No] [numeric](18, 0) NULL,
	[SL_No] [numeric](18, 0) NULL,
	[ASSUR_NO] [int] NULL,
	[DOSupAnnuating] [datetime] NULL,
	[PassPortNo] [varchar](10) NULL,
	[PPIDate] [datetime] NULL,
	[PPEDate] [datetime] NULL,
	[GISNominee] [varchar](30) NULL,
	[Relation] [varchar](1) NULL,
	[Pr_Loc_DOJ] [datetime] NULL,
	[Pr_desg] [varchar](4) NULL,
	[Sen_Code] [varchar](20) NULL,
	[P_Scale] [varchar](50) NULL,
	[CadOfEmp] [varchar](20) NULL,
	[IncrementMonth] [int] NULL,
	[ValidateIncrement] [bit] NULL,
	[Reason] [varchar](50) NULL,
	[Dept_Enq] [bit] NULL,
	[Cer_Given] [bit] NULL,
	[Inv_Issued] [bit] NULL,
	[Books_LIB] [bit] NULL,
	[DeletedEmployee] [bit] NULL,
	[PanNo] [varchar](50) NULL,
	[DOPENSIONJOIN] [datetime] NULL,
	[DATEOFLEAVING] [datetime] NULL,
	[LEAVINGREMARKS] [varchar](30) NULL,
	[PENSIONSCHNO] [int] NULL,
	[DATEENTITLEMENTPS] [datetime] NULL,
	[OtaCode] [smallint] NULL,
	[IsSalgenrated] [bit] NULL,
	[PENSIONDEDUCT] [bit] NULL,
	[PensionUAN] [varchar](12) NULL,
 CONSTRAINT [PK_tblMstEmployee_1] PRIMARY KEY CLUSTERED 
(
	[EmployeeCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[EmployeeDependant]    Script Date: 22/07/2019 17:55:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[EmployeeDependant](
	[EmpDependantID] [int] IDENTITY(1,1) NOT NULL,
	[EmpID] [int] NOT NULL,
	[DependantName] [varchar](50) NOT NULL,
	[Gender] [char](1) NOT NULL,
	[DOB] [datetime] NULL,
	[Relation] [char](1) NOT NULL,
	[Handicapped] [bit] NULL,
 CONSTRAINT [PK_EmployeeDependant] PRIMARY KEY CLUSTERED 
(
	[EmpDependantID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Grade]    Script Date: 22/07/2019 17:55:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Grade](
	[GradeID] [int] IDENTITY(1,1) NOT NULL,
	[GradeName] [varchar](5) NOT NULL,
 CONSTRAINT [PK_Grade] PRIMARY KEY CLUSTERED 
(
	[GradeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Holiday]    Script Date: 22/07/2019 17:55:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Holiday](
	[HolidayID] [int] IDENTITY(1,1) NOT NULL,
	[HolidayName] [varchar](50) NOT NULL,
	[HolidayDate] [date] NOT NULL,
 CONSTRAINT [PK_Holiday] PRIMARY KEY CLUSTERED 
(
	[HolidayID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Menu]    Script Date: 22/07/2019 17:55:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Menu](
	[MenuID] [int] IDENTITY(1,1) NOT NULL,
	[MenuName] [varchar](100) NOT NULL,
	[ParentID] [int] NULL,
	[SequenceNo] [int] NULL,
	[Url] [varchar](100) NULL,
	[IconClass] [varchar](100) NULL,
	[IsActive] [bit] NULL,
 CONSTRAINT [PK_mstmenu] PRIMARY KEY CLUSTERED 
(
	[MenuID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Process]    Script Date: 22/07/2019 17:55:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Process](
	[ProcessID] [int] IDENTITY(1,1) NOT NULL,
	[ProcessName] [varchar](50) NOT NULL,
	[CreatedBy] [int] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[UpdatedBy] [int] NULL,
	[UpdatedOn] [datetime] NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_Process] PRIMARY KEY CLUSTERED 
(
	[ProcessID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ProcessStage]    Script Date: 22/07/2019 17:55:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ProcessStage](
	[StageID] [int] IDENTITY(1,1) NOT NULL,
	[ProcessID] [int] NOT NULL,
	[StageName] [varchar](50) NULL,
	[CreatedBy] [int] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[UpdatedBy] [int] NULL,
	[UpdatedOn] [datetime] NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_ProcessStage] PRIMARY KEY CLUSTERED 
(
	[StageID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[SMSConfiguration]    Script Date: 22/07/2019 17:55:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[SMSConfiguration](
	[SMSConfigurationID] [int] IDENTITY(1,1) NOT NULL,
	[ApiKey] [varchar](50) NOT NULL,
	[SMSUrl] [varchar](50) NOT NULL,
	[SenderID] [varchar](50) NOT NULL,
	[Channel] [varchar](50) NOT NULL,
	[DCS] [varchar](50) NOT NULL,
	[FlashSMS] [varchar](50) NOT NULL,
	[Route] [varchar](50) NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[CreatedBy] [int] NOT NULL,
	[UpdatedOn] [datetime] NULL,
	[UpdatedBy] [int] NULL,
 CONSTRAINT [PK_SMSConfiguration] PRIMARY KEY CLUSTERED 
(
	[SMSConfigurationID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[StaffGrievances]    Script Date: 22/07/2019 17:55:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[StaffGrievances](
	[TransNo] [int] IDENTITY(1,1) NOT NULL,
	[EmployeeID] [int] NOT NULL,
	[BranchID] [int] NOT NULL,
	[GrievancesDate] [datetime] NULL,
	[GrievancesType] [varchar](100) NULL,
	[Description] [varchar](100) NULL,
	[ActionTaken] [bit] NULL,
	[Reason] [varchar](100) NULL,
 CONSTRAINT [PK_StaffGrievances] PRIMARY KEY CLUSTERED 
(
	[TransNo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[State]    Script Date: 22/07/2019 17:55:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[State](
	[StateID] [int] IDENTITY(1,1) NOT NULL,
	[StateName] [varchar](50) NULL,
 CONSTRAINT [PK_State] PRIMARY KEY CLUSTERED 
(
	[StateID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Training]    Script Date: 22/07/2019 17:55:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Training](
	[TrainingID] [int] IDENTITY(1,1) NOT NULL,
	[StartDate] [datetime] NOT NULL,
	[EndDate] [datetime] NULL,
	[Subject] [varchar](300) NOT NULL,
	[Venue] [varchar](50) NOT NULL,
	[ConductedBy] [varchar](50) NULL,
 CONSTRAINT [PK_Training] PRIMARY KEY CLUSTERED 
(
	[TrainingID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[User]    Script Date: 22/07/2019 17:55:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[User](
	[UserID] [int] IDENTITY(1,1) NOT NULL,
	[UserName] [varchar](50) NOT NULL,
	[FullName] [varchar](100) NOT NULL,
	[Password] [varchar](200) NOT NULL,
	[DepartmentID] [int] NOT NULL,
	[UserTypeID] [int] NOT NULL,
	[MobileNo] [varchar](10) NOT NULL,
	[EmailID] [varchar](50) NOT NULL,
	[ImageName] [varchar](50) NULL,
	[CreatedBy] [int] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[UpdatedBy] [int] NULL,
	[UpdatedOn] [datetime] NULL,
	[IsActive] [bit] NOT NULL CONSTRAINT [DF_User_IsActive]  DEFAULT ((0)),
	[IsDeleted] [bit] NOT NULL CONSTRAINT [DF_User_Isdeleted]  DEFAULT ((0)),
 CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED 
(
	[UserID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[UserRights]    Script Date: 22/07/2019 17:55:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserRights](
	[UserID] [int] NOT NULL,
	[DepartmentID] [int] NOT NULL,
	[MenuID] [int] NOT NULL,
	[ShowMenu] [bit] NOT NULL CONSTRAINT [DF_UserRights_ShowMenu]  DEFAULT ((0)),
	[CreateRight] [bit] NOT NULL CONSTRAINT [DF_UserRights_CreateRight]  DEFAULT ((0)),
	[ViewRight] [bit] NOT NULL CONSTRAINT [DF_UserRights_ViewRight]  DEFAULT ((0)),
	[EditRight] [bit] NOT NULL CONSTRAINT [DF_UserRights_EditRight]  DEFAULT ((0)),
	[DeleteRight] [bit] NOT NULL CONSTRAINT [DF_UserRights_DeleteRight]  DEFAULT ((0)),
 CONSTRAINT [PK_UserMenuRights] PRIMARY KEY CLUSTERED 
(
	[UserID] ASC,
	[DepartmentID] ASC,
	[MenuID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[UserType]    Script Date: 22/07/2019 17:55:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserType](
	[UserTypeID] [int] NOT NULL,
	[UserTypeName] [nvarchar](50) NOT NULL,
	[IsDeleted] [bit] NOT NULL CONSTRAINT [DF_UserType_IsDeleted]  DEFAULT ((0)),
 CONSTRAINT [PK_UserType] PRIMARY KEY CLUSTERED 
(
	[UserTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[Branch] ADD  CONSTRAINT [DF_Branch_IsHillComp]  DEFAULT ((0)) FOR [IsHillComp]
GO
ALTER TABLE [dbo].[Branch] ADD  CONSTRAINT [DF_Branch_IsOldBranch]  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[Designation] ADD  CONSTRAINT [DF_Designation_IsOfficer]  DEFAULT ((0)) FOR [IsOfficer]
GO
ALTER TABLE [dbo].[Designation] ADD  CONSTRAINT [DF_Designation_IsDeleted]  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[DocumentExtension] ADD  CONSTRAINT [DF_DocumentExtension_IsDeleted]  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[EmailConfiguration] ADD  CONSTRAINT [DF_EmailConfiguration_SSLStatus]  DEFAULT ((0)) FOR [SSLStatus]
GO
ALTER TABLE [dbo].[EmailConfiguration] ADD  CONSTRAINT [DF_EmailConfiguration_IsDeleted]  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[EmailConfiguration] ADD  CONSTRAINT [DF_EmailConfiguration_IsMaintenance]  DEFAULT ((0)) FOR [IsMaintenance]
GO
ALTER TABLE [dbo].[Process] ADD  CONSTRAINT [DF_Process_CreatedOn]  DEFAULT (getdate()) FOR [CreatedOn]
GO
ALTER TABLE [dbo].[Process] ADD  CONSTRAINT [DF_Process_IsDeleted]  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[ProcessStage] ADD  CONSTRAINT [DF_ProcessStage_IsDeleted]  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[SMSConfiguration] ADD  CONSTRAINT [DF_SMSConfiguration_CreatedOn]  DEFAULT (getdate()) FOR [CreatedOn]
GO
ALTER TABLE [dbo].[DepartmentRights]  WITH CHECK ADD  CONSTRAINT [FK_DepartmentRights_Department] FOREIGN KEY([DepartmentID])
REFERENCES [dbo].[Department] ([DepartmentID])
GO
ALTER TABLE [dbo].[DepartmentRights] CHECK CONSTRAINT [FK_DepartmentRights_Department]
GO
ALTER TABLE [dbo].[DepartmentRights]  WITH CHECK ADD  CONSTRAINT [FK_DepartmentRights_Menu] FOREIGN KEY([MenuID])
REFERENCES [dbo].[Menu] ([MenuID])
GO
ALTER TABLE [dbo].[DepartmentRights] CHECK CONSTRAINT [FK_DepartmentRights_Menu]
GO
ALTER TABLE [dbo].[Docment]  WITH CHECK ADD  CONSTRAINT [FK_Docment_User] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[User] ([UserID])
GO
ALTER TABLE [dbo].[Docment] CHECK CONSTRAINT [FK_Docment_User]
GO
ALTER TABLE [dbo].[Docment]  WITH CHECK ADD  CONSTRAINT [FK_Docment_User1] FOREIGN KEY([UpdatedBy])
REFERENCES [dbo].[User] ([UserID])
GO
ALTER TABLE [dbo].[Docment] CHECK CONSTRAINT [FK_Docment_User1]
GO
ALTER TABLE [dbo].[DocumentExtension]  WITH CHECK ADD  CONSTRAINT [FK_DocumentExtension_User] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[User] ([UserID])
GO
ALTER TABLE [dbo].[DocumentExtension] CHECK CONSTRAINT [FK_DocumentExtension_User]
GO
ALTER TABLE [dbo].[DocumentExtension]  WITH CHECK ADD  CONSTRAINT [FK_DocumentExtension_User1] FOREIGN KEY([UpdatedBy])
REFERENCES [dbo].[User] ([UserID])
GO
ALTER TABLE [dbo].[DocumentExtension] CHECK CONSTRAINT [FK_DocumentExtension_User1]
GO
ALTER TABLE [dbo].[Process]  WITH CHECK ADD  CONSTRAINT [FK_Process_Process] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[User] ([UserID])
GO
ALTER TABLE [dbo].[Process] CHECK CONSTRAINT [FK_Process_Process]
GO
ALTER TABLE [dbo].[Process]  WITH CHECK ADD  CONSTRAINT [FK_Process_User] FOREIGN KEY([UpdatedBy])
REFERENCES [dbo].[User] ([UserID])
GO
ALTER TABLE [dbo].[Process] CHECK CONSTRAINT [FK_Process_User]
GO
ALTER TABLE [dbo].[ProcessStage]  WITH CHECK ADD  CONSTRAINT [FK_ProcessStage_Process] FOREIGN KEY([ProcessID])
REFERENCES [dbo].[Process] ([ProcessID])
GO
ALTER TABLE [dbo].[ProcessStage] CHECK CONSTRAINT [FK_ProcessStage_Process]
GO
ALTER TABLE [dbo].[ProcessStage]  WITH CHECK ADD  CONSTRAINT [FK_ProcessStage_User] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[User] ([UserID])
GO
ALTER TABLE [dbo].[ProcessStage] CHECK CONSTRAINT [FK_ProcessStage_User]
GO
ALTER TABLE [dbo].[ProcessStage]  WITH CHECK ADD  CONSTRAINT [FK_ProcessStage_User1] FOREIGN KEY([UpdatedBy])
REFERENCES [dbo].[User] ([UserID])
GO
ALTER TABLE [dbo].[ProcessStage] CHECK CONSTRAINT [FK_ProcessStage_User1]
GO
ALTER TABLE [dbo].[User]  WITH CHECK ADD  CONSTRAINT [FK_User_Department] FOREIGN KEY([DepartmentID])
REFERENCES [dbo].[Department] ([DepartmentID])
GO
ALTER TABLE [dbo].[User] CHECK CONSTRAINT [FK_User_Department]
GO
ALTER TABLE [dbo].[User]  WITH CHECK ADD  CONSTRAINT [FK_User_UserType] FOREIGN KEY([UserTypeID])
REFERENCES [dbo].[UserType] ([UserTypeID])
GO
ALTER TABLE [dbo].[User] CHECK CONSTRAINT [FK_User_UserType]
GO
ALTER TABLE [dbo].[UserRights]  WITH CHECK ADD  CONSTRAINT [FK_UserRights_Department] FOREIGN KEY([DepartmentID])
REFERENCES [dbo].[Department] ([DepartmentID])
GO
ALTER TABLE [dbo].[UserRights] CHECK CONSTRAINT [FK_UserRights_Department]
GO
ALTER TABLE [dbo].[UserRights]  WITH CHECK ADD  CONSTRAINT [FK_UserRights_Menu] FOREIGN KEY([MenuID])
REFERENCES [dbo].[Menu] ([MenuID])
GO
ALTER TABLE [dbo].[UserRights] CHECK CONSTRAINT [FK_UserRights_Menu]
GO
ALTER TABLE [dbo].[UserRights]  WITH CHECK ADD  CONSTRAINT [FK_UserRights_User] FOREIGN KEY([UserID])
REFERENCES [dbo].[User] ([UserID])
GO
ALTER TABLE [dbo].[UserRights] CHECK CONSTRAINT [FK_UserRights_User]
GO
/****** Object:  StoredProcedure [dbo].[InsertUpdateDepartmentRights]    Script Date: 22/07/2019 17:55:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Author:		<Anshu>
-- Create date: <12-07-2019>
-- Description:	<Add DepartmentRight Tree View>
-- =============================================
create PROCEDURE [dbo].[InsertUpdateDepartmentRights]
	@DepartmentID int,
	@DepartmentRights AS [DepartmentRights] READONLY  ,
	@ProcessStatus INT OUTPUT
AS
BEGIN TRY
SET NOCOUNT ON	
	

	DELETE FROM DepartmentRights where DepartmentID = @DepartmentID;
	INSERT INTO DepartmentRights SELECT * FROM @DepartmentRights;

-- Insert Parent menu
INSERT INTO DepartmentRights select @DepartmentID DepartmentID, * ,1 ShowMenu  from (
select t1.MenuID from
(select m.MenuID, m.ParentID from Menu m
join DepartmentRights u on u.MenuID = m.MenuID where u.DepartmentID = @DepartmentID
UNION
select m.MenuID, m.ParentID from Menu m
  where m.MenuID in (select distinct m.ParentID from Menu m
join DepartmentRights u on u.MenuID = m.MenuID where u.DepartmentID = @DepartmentID and m.ParentID<>0))t1
EXCEPT
select MenuID from DepartmentRights r where r.DepartmentID = @DepartmentID)p1

SET @ProcessStatus=1

END TRY
BEGIN CATCH
DECLARE @ErrorMessage NVARCHAR(MAX);
DECLARE @ErrorNumber INT;
DECLARE @ErrorSeverity INT;
DECLARE @ErrorState INT;
SELECT @ErrorMessage = ERROR_MESSAGE(),
@ErrorSeverity = ERROR_SEVERITY(),
@ErrorState = ERROR_STATE(),
@ErrorNumber = ERROR_NUMBER();
RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState);
END CATCH




GO
/****** Object:  StoredProcedure [dbo].[InsertUpdateDepartmentUserRights]    Script Date: 22/07/2019 17:55:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Author:		<Anshu>
-- Create date: <12-07-2019>
-- Description:	<Add Update DepartmentUserRights Tree View>
-- =============================================
CREATE PROCEDURE [dbo].[InsertUpdateDepartmentUserRights]
    @UserID int,
	@DepartmentID int,
	@UserRights AS [UserRights] READONLY  ,
	@ProcessStatus INT OUTPUT
AS
BEGIN TRY
SET NOCOUNT ON	
	

	DELETE FROM UserRights where DepartmentID = @DepartmentID and UserID=@UserID;
	INSERT INTO UserRights SELECT * FROM @UserRights;

-- Insert Parent menu
	INSERT INTO UserRights select @UserID UserID, @DepartmentID DepartmentID, * ,1 ShowMenu,1 CreateRight, 1 ViewRight, 1 EditRight, 1 DeleteRight  from (
	select t1.MenuID from
	(select m.MenuID, m.ParentID from Menu m
	join UserRights u on u.MenuID = m.MenuID where u.DepartmentID = @DepartmentID and u.UserID=@UserID
	UNION
	select m.MenuID, m.ParentID from Menu m
	where m.MenuID in (select distinct m.ParentID from Menu m
	join UserRights u on u.MenuID = m.MenuID where u.DepartmentID = @DepartmentID and u.UserID=@UserID and m.ParentID<>0))t1
	EXCEPT
	select MenuID from UserRights r where r.DepartmentID = @DepartmentID and r.UserID=@UserID)p1

SET @ProcessStatus=1

END TRY
BEGIN CATCH
DECLARE @ErrorMessage NVARCHAR(MAX);
DECLARE @ErrorNumber INT;
DECLARE @ErrorSeverity INT;
DECLARE @ErrorState INT;
SELECT @ErrorMessage = ERROR_MESSAGE(),
@ErrorSeverity = ERROR_SEVERITY(),
@ErrorState = ERROR_STATE(),
@ErrorNumber = ERROR_NUMBER();
RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState);
END CATCH




GO
USE [master]
GO
ALTER DATABASE [MicroPay] SET  READ_WRITE 
GO
