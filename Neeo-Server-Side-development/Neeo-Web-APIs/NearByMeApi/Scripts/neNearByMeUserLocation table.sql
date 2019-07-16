USE [XMPPDb]
GO

/****** Object:  Table [dbo].[neNearByMeUserLocation]    Script Date: 3/14/2019 1:18:27 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[neNearByMeUserLocation](
	[indexer] [bigint] NOT NULL,
	[username] [nvarchar](64) NOT NULL,
	[latitude] [float] NOT NULL,
	[longitude] [float] NOT NULL,
	[geoLocation] [geography] NOT NULL,
	[lastUpdateDate] [datetime] NOT NULL DEFAULT (getutcdate()),
 CONSTRAINT [PK_neNearByMeUser] PRIMARY KEY CLUSTERED 
(
	[indexer] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[username] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO


