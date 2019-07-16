USE [XMPPDb]
GO
/****** Object:  StoredProcedure [dbo].[spne_GetClientAppVersionByReceiverID]    Script Date: 2014-10-30 7:45:10 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Zohaib Hanif
-- Create date: 27-Oct-2014
-- Description:	Gets the receiver client information. It also checks that both the sender and the receiver are the application users.
-- =============================================
CREATE PROCEDURE [dbo].[spne_GetClientAppVersionByReceiverID] 
	@senderID nvarchar(64), 
	@receiverID nvarchar(64) 
AS
BEGIN
	SET NOCOUNT ON;
    DECLARE @InvalidUserCode int = 50404;

	IF ((SELECT Count(1) 
			   FROM ofUser
			   WHERE username IN (@senderID, @receiverID)) = 2)
	BEGIN
			SELECT CASE WHEN appVersion <> '' THEN appVersion ELSE 'NEEO-0.0.0' END
			FROM neUserExtension
			WHERE username = @receiverID;
	END
	ELSE
		RAISERROR(@InvalidUserCode,-1,-1); 
END
