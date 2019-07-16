ALTER TRIGGER [dbo].[trne_SendFriendRequestNotification] 
   ON  [dbo].[neNearByMeRoster] 
   AFTER INSERT, UPDATE
AS 
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    DECLARE @receiverID nvarchar(64);
	DECLARE @senderID nvarchar(64); 
	DECLARE @temp nvarchar(64); 
	DECLARE @senderName nvarchar(100); 
	DECLARE @deviceToken varchar(1024);
	DECLARE @devicePlatform char(1);
	DECLARE @pnSource char(1);
	DECLARE @appVersion varchar(15);
	DECLARE @offlineMsgCount varchar(10);
	DECLARE @notificationMsg nvarchar(max);
	DECLARE @imTone char(1);
	DECLARE @currentStatus int;
	DECLARE @oldStatus int = null;

	DECLARE @reqBody nvarchar(max);
	-- Constant variables
	DECLARE @Chat tinyint = 1;
	DECLARE @ImageFile tinyint = 2;
	DECLARE @AudioFile tinyint = 18;
	DECLARE @AudioRecordedFile tinyint = 19;
	DECLARE @VideoFile tinyint = 20;
	DECLARE @NotificationType char(1);

	SET @NotificationType = '1'; -- Friend Request

	SELECT @receiverID = inserted.fid, @senderID = inserted.username, @currentStatus = inserted.[status]
							   FROM inserted;

	IF @currentStatus = 2
		RETURN;

	IF EXISTS (SELECT 1 FROM deleted)
	BEGIN
		SELECT @oldStatus = deleted.[status]
							   FROM deleted;

		--IF @oldStatus <> 1 AND @currentStatus = 1
		IF @currentStatus = 1
		BEGIN
			SET @NotificationType = '1'; -- Request Accepted
			SET @temp = @receiverID;
			SET @receiverID = @senderID;
			SET @senderID = @temp;
			
		END
		--ELSE
		--	RETURN;
	END
	ELSE
	BEGIN
	   -- Ignore if request automatically gets accepted.
 		IF @currentStatus = 1
		RETURN;
	END

	SELECT @deviceToken = neUserExtension.deviceToken, @devicePlatform = neUserExtension.devicePlatform, @imTone = neUserExtension.imTone, @appVersion = appVersion, @pnSource = neUserExtension.pnSource
			FROM neUserExtension
			WHERE neUserExtension.username = @receiverID;

	--IF (@devicePlatform = '2' AND @appVersion < 'Neeo-3.3.2') OR (@devicePlatform = '1' AND @appVersion < 'Neeo-3.3.2')
	--	RETURN;

	SET @senderName = (SELECT CASE WHEN (name = '' OR name IS NULL) THEN CONCAT('+',username) ELSE name END
								FROM ofUser 
								WHERE username = @senderID);

	SET @notificationMsg = @senderName + ' has sent you friend request.';
		
	IF @NotificationType = '8'		    
	BEGIN
		SET @notificationMsg = @senderName + ' has accepted your friend request.';
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
	SET @reqBody += '"imTone": ' + @imTone  ;
	SET @reqBody += '}';
					
	EXEC spne_SendNotification @reqBody;

	print @reqBody;
END
