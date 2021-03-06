USE [XMPPDb]
GO
/****** Object:  Trigger [dbo].[trne_SendGroupNotification]    Script Date: 1/21/2015 4:26:15 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Zohaib Hanif
-- Create date: 23-Dec-2014
-- Modification 
-- [05-Jan-2015]
-- [14-Jan-2015]- Room name is added to send the room information to client.[zohaib]
-- [21-Jan-2015]- Show username i.e phone number if the user's name is not available.[zohaib]
-- [25-Mar-2015] - Notfication failure on double qoutes is fixed. [zohaib]
-- Description:	Sends group notifications to the group members.
-- =============================================
CREATE TRIGGER [dbo].[trne_SendGroupNotification] 
   ON  [dbo].[ofMucConversationLog] 
   AFTER INSERT
AS 
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @roomID varchar(10);
	DECLARE @roomName varchar(50);
	DECLARE @roomSubject nvarchar(100);
	DECLARE @senderID nvarchar(64);
	DECLARE @senderName nvarchar(100);
	DECLARE @msg nvarchar(max);
	DECLARE @reqBody nvarchar(max);
	DECLARE @logTime char(15);
	DECLARE @senderJid nvarchar(1024);
	DECLARE @nickname nvarchar(255);
	DECLARE @Key varchar(14);

	DECLARE @NotificationType char(1) = '4';
	DECLARE @Text varchar(14) = '~$%#32@92%^45~';
	DECLARE @File varchar(14) = '~$%#35$92%^50~';
	DECLARE @KeyLength tinyint = 14;

	SELECT @roomID = roomID, @logTime = logTime, @senderjid = sender, @nickname = nickname
	FROM inserted;
	
	SET @msg = (SELECT body
			    FROM ofMucConversationLog
			    WHERE roomID = @roomID
				      AND sender = @senderjid
					  AND logTime = @logTime);

	IF (COALESCE(@msg,'') <> '')
	BEGIN
	    SET @Key = SUBSTRING(@msg, 0, @KeyLength + 1);
		IF(@Key = @Text)
		BEGIN
			SET @msg = SUBSTRING(@msg, @KeyLength + 1, LEN(@msg) - @KeyLength + 1);
			SELECT @roomSubject = [subject], @roomName = name
			FROM ofMucRoom
			WHERE roomID = @roomID;
			SELECT @senderName = CASE WHEN (name = '' OR name IS NULL) THEN CONCAT('+',username) ELSE name END, @senderID = username
			FROM ofUser
			WHERE username = @nickname; 

			SET @msg = @senderName + ' @ "' + @roomSubject + '": ' + @msg;

			SET @reqBody = '{'; 
			SET @reqBody += '"nType": ' + @NotificationType + ',';
			SET @reqBody += '"rID": ' + @roomID + ',';
			SET @reqBody += '"rName": "' + @roomName + '",';
			SET @reqBody += '"alert": "' + [dbo].[FormatString](@msg) + '",';
			SET @reqBody += '"senderID": "' + @senderID + '"';
			SET @reqBody += '}';
        
			EXEC spne_SendNotification @reqBody;

		END
		ELSE IF(@Key = @File)
		BEGIN
			
			SELECT @roomSubject = [subject], @roomName = name
			FROM ofMucRoom
			WHERE roomID = @roomID;
			SELECT @senderName = name, @senderID = username
			FROM ofUser
			WHERE username = @nickname; 

			SET @msg = @senderName + ' sent an image to "' + @roomSubject + '"';

			SET @reqBody = '{'; 
			SET @reqBody += '"nType": ' + @NotificationType + ',';
			SET @reqBody += '"rID": ' + @roomID + ',';
			SET @reqBody += '"rName": "' + @roomName + '",';
			SET @reqBody += '"alert": "' + [dbo].[FormatString](@msg) + '",';
			SET @reqBody += '"senderID": "' + @senderID + '"';
			SET @reqBody += '}';
        
			EXEC spne_SendNotification @reqBody;
		END
	END
END

