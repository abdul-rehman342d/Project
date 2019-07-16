USE [XMPPDb]
GO

/****** Object:  Table [dbo].[ofRoster]    Script Date: 4/27/2019 3:34:20 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[neNearByMeRoster](
	[rosterID] [bigint] NOT NULL IDENTITY(1, 1),
	[username] [nvarchar](64) NOT NULL,
	[fid] [nvarchar](64) NOT NULL,
	[status] [int] NOT NULL,
	[createdDate] [datetime] NOT NULL,
	[updatedDate] [datetime] NULL
 CONSTRAINT [neNearByMeRoster_pk] PRIMARY KEY CLUSTERED 
(
	[rosterID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


