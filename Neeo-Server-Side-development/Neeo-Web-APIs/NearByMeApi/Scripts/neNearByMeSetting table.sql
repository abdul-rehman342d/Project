USE [XMPPDb]
GO

/****** Object:  Table [dbo].[neNearByMeSetting]    Script Date: 3/14/2019 1:18:20 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[neNearByMeSetting](
	[username] [nvarchar](64) NOT NULL,
	[indexer] [bigint] IDENTITY(1,1) NOT NULL,
	[enabled] [bit] NOT NULL CONSTRAINT [DF_neNearByMeSetting_enable]  DEFAULT ((0)),
	[enableDate] [datetime] NULL,
	[updateDate] [datetime] NULL,
	[lastFindDate] [datetime] NULL,
	[notificationTone] [tinyint] NOT NULL CONSTRAINT [DF_Table_1_NotificationTone]  DEFAULT ((0)),
	[notificationOn] [bit] NOT NULL CONSTRAINT [DF_Table_1_NotificationOn]  DEFAULT ((1)),
	[showInfo] [bit] NOT NULL CONSTRAINT [DF_Table_1_ShowInfo]  DEFAULT ((0)),
	[showProfileImage] [bit] NOT NULL CONSTRAINT [DF_Table_1_ShowProfileImage]  DEFAULT ((1)),
	[createDate] [datetime] NULL,
	[isPrivateAccount] [bit] NOT NULL DEFAULT ((0)),
 CONSTRAINT [PK_neNearByMeSetting] PRIMARY KEY CLUSTERED 
(
	[username] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


