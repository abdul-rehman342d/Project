USE [XMPPDb]
GO
/****** Object:  StoredProcedure [dbo].[spne_GetUserInfoByCallerIDAndReceiverIDWithOfflineMsgCount]    Script Date: 6/24/2014 9:13:16 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Zohaib Hanif
-- Create date: 23/06/2014
-- Modification date: 23-Dec-2014
-- [03-Feb-2015] - Updated to update mcr count in database and return sum of offline count and mcr count.[zohaib]
-- Description:	It gives the information about receiver  and caller user for push notification with receiver offline messages count including mcr count as well.
-- =============================================
CREATE PROCEDURE [dbo].[spne_GetUserInfoByCallerIDAndReceiverIDWithOfflineMsgCount] 
	-- Add the parameters for the stored procedure here
	@receiverID nvarchar(64),
	@callerID nvarchar(64),
	@getOfflineMsgCount bit,
	@mcrCount int = 0  

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	DECLARE @offlineMsgCount int = 0;
	-- Constant variables
	DECLARE @Chat tinyint = 0;
	DECLARE @File tinyint = 1;
    -- Insert statements for procedure here
	IF (@getOfflineMsgCount = 1)
	BEGIN

		UPDATE neOfflineUserMessageCount
		SET mcrCount = @mcrCount
		WHERE username = @receiverID;

		SET @offlineMsgCount = (SELECT (messageCount + mcrCount) AS messageCount
								FROM neOfflineUserMessageCount
								WHERE username = @receiverID);
		
		SELECT neUserExtension.deviceToken AS receiverUserDToken,neUserExtension.devicePlatform AS receiverUserDp, @offlineMsgCount AS offlineMsgCount
		From neUserExtension 
		WHERE neUserExtension.username = @receiverID;
	END
	ELSE
		SELECT neUserExtension.deviceToken AS receiverUserDToken,neUserExtension.devicePlatform AS receiverUserDp
		From neUserExtension
		WHERE neUserExtension.username = @receiverID;

	SELECT CASE WHEN (name = '' OR name IS NULL) THEN CONCAT('+', username) ELSE name END AS callerName
	From ofUser
	WHERE ofUser.username = @callerID;
END
