USE [XMPPDb]
GO
/****** Object:  StoredProcedure [dbo].[spne_GetUserInfoByCallerIDAndCallingID]    Script Date: 6/11/2014 12:23:41 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Zohaib Hanif
-- Create date: 05/06/2014
-- Description:	It gives the information about calling user and caller user for push notification.
-- =============================================
CREATE PROCEDURE [dbo].[spne_GetUserInfoByCallerIDAndReceiverID] 
	-- Add the parameters for the stored procedure here
	@receiverID nvarchar(64),
	@callerID nvarchar(64)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT neUserExtension.deviceToken AS receiverUserDToken,neUserExtension.devicePlatform AS receiverUserDp
	From neUserExtension
	WHERE neUserExtension.username = @receiverID;

	SELECT CASE WHEN (name = '' OR name IS NULL) THEN username ELSE name END AS callerName
	From ofUser
	WHERE ofUser.username = @callerID;
END
