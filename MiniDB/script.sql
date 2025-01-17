USE [master]
GO
/****** Object:  Database [MiniDB]    Script Date: 2020/11/8 22:00:16 ******/
CREATE DATABASE [MiniDB]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'MiniDB', FILENAME = N'D:\Program Files\Microsoft SQL Server\MSSQL15.SQLEXPRESS\MSSQL\DATA\MiniDB.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'MiniDB_log', FILENAME = N'D:\Program Files\Microsoft SQL Server\MSSQL15.SQLEXPRESS\MSSQL\DATA\MiniDB_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT
GO
ALTER DATABASE [MiniDB] SET COMPATIBILITY_LEVEL = 150
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [MiniDB].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [MiniDB] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [MiniDB] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [MiniDB] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [MiniDB] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [MiniDB] SET ARITHABORT OFF 
GO
ALTER DATABASE [MiniDB] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [MiniDB] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [MiniDB] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [MiniDB] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [MiniDB] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [MiniDB] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [MiniDB] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [MiniDB] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [MiniDB] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [MiniDB] SET  DISABLE_BROKER 
GO
ALTER DATABASE [MiniDB] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [MiniDB] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [MiniDB] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [MiniDB] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [MiniDB] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [MiniDB] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [MiniDB] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [MiniDB] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [MiniDB] SET  MULTI_USER 
GO
ALTER DATABASE [MiniDB] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [MiniDB] SET DB_CHAINING OFF 
GO
ALTER DATABASE [MiniDB] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [MiniDB] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [MiniDB] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [MiniDB] SET QUERY_STORE = OFF
GO
USE [MiniDB]
GO
/****** Object:  Table [dbo].[Article]    Script Date: 2020/11/8 22:00:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Article](
	[Id] [uniqueidentifier] NOT NULL,
	[Title] [varchar](50) NOT NULL,
	[Author] [uniqueidentifier] NOT NULL,
	[Time] [datetime] NOT NULL,
	[Match] [varchar](100) NOT NULL,
	[Recommand] [varchar](100) NOT NULL,
	[Analysis] [text] NULL,
	[Attachment] [uniqueidentifier] NULL,
	[IsTrue] [bit] NULL,
	[Preference] [int] NOT NULL,
	[Collection] [int] NOT NULL,
	[Money] [decimal](18, 2) NOT NULL,
	[Status] [smallint] NOT NULL,
 CONSTRAINT [PK_Article_Free] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Attachment]    Script Date: 2020/11/8 22:00:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Attachment](
	[Id] [uniqueidentifier] NOT NULL,
	[FileName] [varchar](100) NOT NULL,
	[FileSize] [bigint] NOT NULL,
	[FileExt] [varchar](50) NOT NULL,
	[AttachmentType] [varchar](50) NOT NULL,
	[FileType] [varchar](100) NOT NULL,
	[UpTime] [datetime] NOT NULL,
	[UpAccount] [uniqueidentifier] NOT NULL,
	[Belong] [uniqueidentifier] NOT NULL,
	[Status] [smallint] NOT NULL,
 CONSTRAINT [PK_Attachment] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Bet]    Script Date: 2020/11/8 22:00:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Bet](
	[Id] [uniqueidentifier] NOT NULL,
	[Member] [uniqueidentifier] NOT NULL,
	[Match] [varchar](200) NOT NULL,
	[Team] [varchar](200) NULL,
	[Time] [datetime] NOT NULL,
	[Money] [decimal](18, 2) NOT NULL,
	[Profit] [decimal](18, 2) NOT NULL,
	[Odds] [float] NOT NULL,
	[Platform] [varchar](20) NOT NULL,
	[Remarks] [varchar](200) NULL,
	[IsSuccess] [bit] NULL,
	[Attachment] [uniqueidentifier] NULL,
 CONSTRAINT [PK_Bet] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Collection]    Script Date: 2020/11/8 22:00:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Collection](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Member] [uniqueidentifier] NOT NULL,
	[Article] [uniqueidentifier] NOT NULL,
	[Time] [datetime] NOT NULL,
	[Status] [smallint] NOT NULL,
 CONSTRAINT [PK_Collection] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Log]    Script Date: 2020/11/8 22:00:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Log](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Member] [uniqueidentifier] NULL,
	[Time] [datetime] NOT NULL,
	[Type] [nvarchar](100) NOT NULL,
	[Remarks] [nvarchar](400) NOT NULL,
	[IP] [varchar](200) NOT NULL,
	[UserAgent] [varchar](2000) NOT NULL,
 CONSTRAINT [PK_Log] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Member]    Script Date: 2020/11/8 22:00:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Member](
	[Id] [uniqueidentifier] NOT NULL,
	[OpenId] [varchar](50) NOT NULL,
	[NickName] [varchar](50) NOT NULL,
	[Gender] [smallint] NOT NULL,
	[Phone] [nchar](11) NULL,
	[City] [varchar](20) NOT NULL,
	[Province] [varchar](20) NOT NULL,
	[Country] [varchar](20) NOT NULL,
	[AvatarUrl] [varchar](200) NOT NULL,
	[UnionId] [varchar](100) NULL,
	[SessionKey] [varchar](100) NOT NULL,
	[Time] [datetime] NOT NULL,
	[Maker] [bit] NOT NULL,
	[Expert] [bit] NOT NULL,
 CONSTRAINT [PK_Member_1] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Order]    Script Date: 2020/11/8 22:00:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Order](
	[Id] [uniqueidentifier] NOT NULL,
	[Member] [uniqueidentifier] NOT NULL,
	[Article] [uniqueidentifier] NOT NULL,
	[Money] [decimal](18, 2) NOT NULL,
	[TimeStart] [datetime] NOT NULL,
	[TimeExpire] [datetime] NOT NULL,
	[Status] [smallint] NOT NULL,
 CONSTRAINT [PK_Order] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Member]    Script Date: 2020/11/8 22:00:17 ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_Member] ON [dbo].[Member]
(
	[OpenId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Article] ADD  CONSTRAINT [DF_Article_Free_Preference]  DEFAULT ((0)) FOR [Preference]
GO
ALTER TABLE [dbo].[Article] ADD  CONSTRAINT [DF_Article_Free_CollectionCount]  DEFAULT ((0)) FOR [Collection]
GO
ALTER TABLE [dbo].[Article] ADD  CONSTRAINT [DF_Article_Free_Money]  DEFAULT ((0.00)) FOR [Money]
GO
ALTER TABLE [dbo].[Bet] ADD  CONSTRAINT [DF_Bet_Profit]  DEFAULT ((0.00)) FOR [Profit]
GO
ALTER TABLE [dbo].[Member] ADD  CONSTRAINT [DF_Member_Maker]  DEFAULT ((0)) FOR [Maker]
GO
ALTER TABLE [dbo].[Member] ADD  CONSTRAINT [DF_Member_Expert]  DEFAULT ((0)) FOR [Expert]
GO
ALTER TABLE [dbo].[Article]  WITH CHECK ADD  CONSTRAINT [FK_Article_Attachment] FOREIGN KEY([Attachment])
REFERENCES [dbo].[Attachment] ([Id])
GO
ALTER TABLE [dbo].[Article] CHECK CONSTRAINT [FK_Article_Attachment]
GO
ALTER TABLE [dbo].[Article]  WITH CHECK ADD  CONSTRAINT [FK_Article_Member] FOREIGN KEY([Author])
REFERENCES [dbo].[Member] ([Id])
GO
ALTER TABLE [dbo].[Article] CHECK CONSTRAINT [FK_Article_Member]
GO
ALTER TABLE [dbo].[Bet]  WITH CHECK ADD  CONSTRAINT [FK_Bet_Attachment] FOREIGN KEY([Attachment])
REFERENCES [dbo].[Attachment] ([Id])
GO
ALTER TABLE [dbo].[Bet] CHECK CONSTRAINT [FK_Bet_Attachment]
GO
ALTER TABLE [dbo].[Bet]  WITH CHECK ADD  CONSTRAINT [FK_Bet_Member] FOREIGN KEY([Member])
REFERENCES [dbo].[Member] ([Id])
GO
ALTER TABLE [dbo].[Bet] CHECK CONSTRAINT [FK_Bet_Member]
GO
ALTER TABLE [dbo].[Collection]  WITH CHECK ADD  CONSTRAINT [FK_Collection_Article] FOREIGN KEY([Article])
REFERENCES [dbo].[Article] ([Id])
GO
ALTER TABLE [dbo].[Collection] CHECK CONSTRAINT [FK_Collection_Article]
GO
ALTER TABLE [dbo].[Collection]  WITH CHECK ADD  CONSTRAINT [FK_Collection_Member] FOREIGN KEY([Member])
REFERENCES [dbo].[Member] ([Id])
GO
ALTER TABLE [dbo].[Collection] CHECK CONSTRAINT [FK_Collection_Member]
GO
ALTER TABLE [dbo].[Log]  WITH CHECK ADD  CONSTRAINT [FK_Log_Member] FOREIGN KEY([Member])
REFERENCES [dbo].[Member] ([Id])
GO
ALTER TABLE [dbo].[Log] CHECK CONSTRAINT [FK_Log_Member]
GO
ALTER TABLE [dbo].[Order]  WITH CHECK ADD  CONSTRAINT [FK_Order_Article] FOREIGN KEY([Article])
REFERENCES [dbo].[Article] ([Id])
GO
ALTER TABLE [dbo].[Order] CHECK CONSTRAINT [FK_Order_Article]
GO
ALTER TABLE [dbo].[Order]  WITH CHECK ADD  CONSTRAINT [FK_Order_Member] FOREIGN KEY([Member])
REFERENCES [dbo].[Member] ([Id])
GO
ALTER TABLE [dbo].[Order] CHECK CONSTRAINT [FK_Order_Member]
GO
USE [master]
GO
ALTER DATABASE [MiniDB] SET  READ_WRITE 
GO
