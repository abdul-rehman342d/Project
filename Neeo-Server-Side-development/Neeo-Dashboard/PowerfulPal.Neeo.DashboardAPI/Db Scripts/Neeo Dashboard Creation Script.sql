USE [master]
GO
/****** Object:  Database [NeeoDashboard]    Script Date: 1/18/2015 10:08:20 AM ******/
CREATE DATABASE [NeeoDashboard] 
GO

ALTER DATABASE [NeeoDashboard] SET COMPATIBILITY_LEVEL = 110
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [NeeoDashboard].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [NeeoDashboard] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [NeeoDashboard] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [NeeoDashboard] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [NeeoDashboard] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [NeeoDashboard] SET ARITHABORT OFF 
GO
ALTER DATABASE [NeeoDashboard] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [NeeoDashboard] SET AUTO_CREATE_STATISTICS ON 
GO
ALTER DATABASE [NeeoDashboard] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [NeeoDashboard] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [NeeoDashboard] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [NeeoDashboard] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [NeeoDashboard] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [NeeoDashboard] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [NeeoDashboard] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [NeeoDashboard] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [NeeoDashboard] SET  DISABLE_BROKER 
GO
ALTER DATABASE [NeeoDashboard] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [NeeoDashboard] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [NeeoDashboard] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [NeeoDashboard] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [NeeoDashboard] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [NeeoDashboard] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [NeeoDashboard] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [NeeoDashboard] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [NeeoDashboard] SET  MULTI_USER 
GO
ALTER DATABASE [NeeoDashboard] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [NeeoDashboard] SET DB_CHAINING OFF 
GO
ALTER DATABASE [NeeoDashboard] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [NeeoDashboard] SET TARGET_RECOVERY_TIME = 0 SECONDS 
GO

/****** Object:  Table [dbo].[neSessionDetails]    Script Date: 1/18/2015 10:08:20 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[neSessionDetails](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[uID] [int] NOT NULL,
	[loginTime] [datetime] NOT NULL,
	[lastActivityTime] [datetime] NOT NULL,
	[isActive] [bit] NOT NULL,
	[authKey] [nvarchar](100) NOT NULL,
 CONSTRAINT [PK_sessionDetails] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[neUser]    Script Date: 1/18/2015 10:08:20 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[neUser](
	[uID] [int] IDENTITY(1,1) NOT NULL,
	[userName] [nvarchar](30) NOT NULL,
	[password] [nvarchar](200) NOT NULL,
	[isActive] [bit] NOT NULL,
 CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED 
(
	[uID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[neSessionDetails]  WITH CHECK ADD  CONSTRAINT [FK_sessionDetails_User] FOREIGN KEY([uID])
REFERENCES [dbo].[neUser] ([uID])
GO
ALTER TABLE [dbo].[neSessionDetails] CHECK CONSTRAINT [FK_sessionDetails_User]
GO
USE [master]
GO
ALTER DATABASE [NeeoDashboard] SET  READ_WRITE 
GO

USE [NeeoDashboard];
/****** Object:  Table [dbo].[neLog]    Script Date: 1/19/2015 11:41:58 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[neLog](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Log] [text] NULL,
 CONSTRAINT [PK_neLog] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO