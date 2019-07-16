
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Zohaib Hanif
-- Create date: 17-Dec-2014
-- Description:	This trigger resets the offine user message count
-- =============================================
CREATE TRIGGER trne_ResetOfflineUserMessageCount 
   ON  ofOffline 
   AFTER DELETE
AS 
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    UPDATE neOfflineUserMessageCount
	SET neOfflineUserMessageCount.messageCount = 0
	WHERE neOfflineUserMessageCount.username IN (SELECT DISTINCT deleted.username
													FROM deleted);

END
GO
