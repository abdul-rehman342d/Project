USE [OpenfireDb]
GO
/****** Object:  Trigger [dbo].[trne_SendNotification]    Script Date: 4/30/2019 3:17:36 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Zohaib Hanif
-- Create date: 05/03/2014 (MM/dd/yyyy)
-- Last Modification date: 10/12/2014, 10/28/2014, 01/05/2015
-- [14-Jan-2015] - Room name in json body is changed.[zohaib]
-- [25-Mar-2015] - Notfication failure on double qoutes is fixed. [zohaib]
-- [10-Aug-2015] - Added audio and video types. [zohaib]
-- Description:	It sends push notification for offline messages by calling notification service.
-- =============================================
ALTER TRIGGER [dbo].[trne_SendNotification] 
   ON  [dbo].[ofOffline] 
   AFTER INSERT
AS 
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    DECLARE @receiverID nvarchar(64);
	DECLARE @senderID nvarchar(64); 
	DECLARE @senderJID nvarchar(500); 
	DECLARE @msgId varchar(40);
	DECLARE @senderName nvarchar(100); 
	DECLARE @deviceToken varchar(1024);
	DECLARE @devicePlatform char(1);
	DECLARE @pnSource char(1);
	DECLARE @appVersion varchar(15);
	DECLARE @offlineMsgCount varchar(10);
	DECLARE @msgType tinyint = 0;
	DECLARE @stanzaXML xml;
	DECLARE @notificationMsg nvarchar(max);
	DECLARE @imTone char(1);
	-- Group Chat
	DECLARE @roomSubject nvarchar(100) = '';
	DECLARE @inviter nvarchar(164);

	DECLARE @reqBody nvarchar(max);
	-- Constant variables
	DECLARE @Chat tinyint = 1;
	DECLARE @ImageFile tinyint = 2;
	DECLARE @AudioFile tinyint = 18;
	DECLARE @AudioRecordedFile tinyint = 19;
	DECLARE @VideoFile tinyint = 20;
	DECLARE @PDFFile tinyint = 100;
	DECLARE @DocFile tinyint = 102;
	DECLARE @PPTFile tinyint = 104;
	DECLARE @XLSFile tinyint = 106;
	DECLARE @SharedLocation tinyint = 108;
	DECLARE @NotificationType char(1);
	DECLARE @IsNearByMe char(1) = '0';

	SET @receiverID = (SELECT inserted.username
							   FROM inserted); 

	SELECT @deviceToken = neUserExtension.deviceToken, @devicePlatform = neUserExtension.devicePlatform, @imTone = neUserExtension.imTone, @appVersion = appVersion, @pnSource = neUserExtension.pnSource
			FROM neUserExtension
			WHERE neUserExtension.username = @receiverID;

	IF @devicePlatform = '2' AND @appVersion < 'Neeo-3.3.2'
		RETURN;

    SET @stanzaXML = (SELECT ofOffline.stanza
						  FROM ofOffline
						  WHERE ofOffline.messageID = (SELECT inserted.messageID
													   FROM inserted));
	
	SET @senderJID = (@stanzaXML.value('(/message/@from)[1]','nvarchar(100)'));
	SET @msgId = COALESCE((@stanzaXML.value('(/message/@id)[1]','varchar(40)')), '');
	
	SET @senderID = [dbo].GetIdFromJid(@senderJID);

	IF(dbo.IsFromGroup(@senderJID) = 1)
	BEGIN

		SET @NotificationType = '5';

		WITH XMLNAMESPACES ('http://jabber.org/protocol/muc#user' AS ns)
		SELECT @inviter = @stanzaXML.value('(/message/ns:x/ns:invite/@from)[1]','nvarchar(164)');

		--**************************************************************************
			
		-- Get User offline messages count and update it.
		UPDATE neOfflineUserMessageCount
		SET messageCount = messageCount + 1
		WHERE neOfflineUserMessageCount.username = @receiverID;

		SELECT @offlineMsgCount = (messageCount + mcrCount)
		FROM neOfflineUserMessageCount
		WHERE neOfflineUserMessageCount.username = @receiverID;
			
		--*************************************************************************
			  
		IF ((@deviceToken = '' OR @deviceToken = '-1'))
			RETURN;

		SET @senderName = (SELECT name
							FROM ofUser
							WHERE username = [dbo].[GetIdFromJid](@inviter)); 
		SELECT @roomSubject = [subject]
		FROM ofMucRoom
		WHERE name = @senderID;

		SET @notificationMsg = @senderName + ' added you to the group "' + @roomSubject + '"';
		--SET @notificationMsg = @senderName + ' added you to the group \"Test\"';

		SET @reqBody = N'{'; 
		SET @reqBody += '"nType": ' + @NotificationType + ',';
		SET @reqBody += '"rName": "' + @senderID + '",';
		SET @reqBody += '"alert": "' + [dbo].[FormatString](@notificationMsg) + '",';
		SET @reqBody += '"receiverID": "' + @receiverID + '",';
		SET @reqBody += '"dToken": "' + @deviceToken + '",';
		SET @reqBody += '"dp": ' + @devicePlatform + ',';
		SET @reqBody += '"pnSource": ' + @pnSource + ',';
		SET @reqBody += '"badge": ' + @offlineMsgCount + ',';
		SET @reqBody += '"imTone": ' + @imTone + ',';
		SET @reqBody += '"msgId": "' + @msgId + '"';
		SET @reqBody += '}';

		EXEC spne_SendNotification @reqBody;
		
	END
	ELSE
	BEGIN

		SET @NotificationType = '1';

		WITH XMLNAMESPACES ('x' AS ns)
		SELECT @msgType = (@stanzaXML.value('(/message/@msgType)[1]','tinyint')),
			   @msgType = @stanzaXML.value('(/message/ns:type/@_type)[1]','tinyint'),
			   @notificationMsg = (LTRIM(RTRIM(@stanzaXML.value('(/message/body)[1]','nvarchar(max)'))));

		WITH XMLNAMESPACES ('x' AS ns)
		SELECT @IsNearByMe = ISNULL(@stanzaXML.value('(/message/ns:isnNearByMeExtension/@_isNearByMe)[1]','int'), 0);

	  --*****************************************************************************
		-- If message type does not find in the message, consider it as a IM message.
		IF COALESCE(@msgType, 0) = 0
		BEGIN
			SET @msgType = @Chat;
		END
	  --*****************************************************************************
		IF (@msgType = @Chat OR @msgType = @ImageFile OR @msgType = @AudioFile OR @msgType = @AudioRecordedFile OR @msgType = @VideoFile OR @msgType = @PDFFile OR @msgType = @DocFile OR @msgType = @PPTFile OR @msgType = @XLSFile OR @msgType = @SharedLocation)
		BEGIN

		  --*************************************************************************

			SET @senderName = (SELECT CASE WHEN (name = '' OR name IS NULL) THEN CONCAT('+',username) ELSE name END
								FROM ofUser 
								WHERE username = @senderID);
							    
			IF @msgType = @Chat
			BEGIN
				SET @notificationMsg = (@senderName + ' : ' + @notificationMsg);
			END
			ELSE IF @msgType = @ImageFile
			BEGIN
				SET @notificationMsg = (@senderName + ' sent you a photo');
			END
			ELSE IF @msgType = @AudioFile
			BEGIN
				IF @appVersion < 'Neeo-3.4.0'  AND @devicePlatform = '1'
					RETURN;

				SET @notificationMsg = (@senderName + ' sent you an audio');
			END
			ELSE IF @msgType = @AudioRecordedFile
			BEGIN
				IF @appVersion < 'Neeo-3.4.0'  AND @devicePlatform = '1'
					RETURN;

				SET @notificationMsg = (@senderName + ' sent you a voice message');
			END
			ELSE IF @msgType = @VideoFile
			BEGIN
				IF @appVersion < 'Neeo-3.5.0'  AND @devicePlatform = '1'
					RETURN;

				SET @notificationMsg = (@senderName + ' sent you a video');
			END
			ELSE IF @msgType = @PDFFile OR @msgType = @DocFile OR @msgType = @PPTFile OR @msgType = @XLSFile
			BEGIN
				IF @appVersion < 'Neeo-4.4'  AND @devicePlatform = '1'
					RETURN;

				SET @notificationMsg = (@senderName + ' sent you a document');
			END
			ELSE IF @msgType = @SharedLocation
			BEGIN
				IF @appVersion < 'Neeo-4.4'  AND @devicePlatform = '1'
					RETURN;

				SET @notificationMsg = (@senderName + ' shared location with you');
			END

			UPDATE neOfflineUserMessageCount
			SET messageCount = messageCount + 1
			WHERE neOfflineUserMessageCount.username = @receiverID;

			SELECT @offlineMsgCount = (messageCount + mcrCount)
			FROM neOfflineUserMessageCount
			WHERE neOfflineUserMessageCount.username = @receiverID;
					
			IF (COALESCE(@notificationMsg,'') = '' OR @deviceToken = '' OR @deviceToken = '-1')
				RETURN;
			   
			SET @reqBody = N'{'; 
			SET @reqBody += '"nType": ' + @NotificationType + ',';
			SET @reqBody += '"dp": ' + @devicePlatform + ',';
			SET @reqBody += '"pnSource": ' + @pnSource + ',';
			SET @reqBody += '"dToken": "' + @deviceToken + '",';
			SET @reqBody += '"alert": "' + [dbo].[FormatString](@notificationMsg) + '",';
			SET @reqBody += '"receiverID": "' + @receiverID + '",';
			SET @reqBody += '"senderID": "' + @senderID + '",';
			SET @reqBody += '"badge": ' +  @offlineMsgCount + ',';
			SET @reqBody += '"imTone": ' + @imTone + ',' ;
			SET @reqBody += '"isNearByMe": ' + @IsNearByMe + ',' ;
			SET @reqBody += '"msgId": "' + @msgId + '"';
			SET @reqBody += '}';
					
			EXEC spne_SendNotification @reqBody;

			PRINT @reqBody;
			
		END
	END
END


