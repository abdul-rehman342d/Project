USE [XMPPDb]
GO
/****** Object:  StoredProcedure [dbo].[spne_UpdateUserDeviceInfoByPhoneNumber]    Script Date: 11/5/2014 9:45:24 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Zohaib Hanif
-- Create date: 16/02/2014 (dd/MM/yyyy)
-- Modificaiton date: 11/08/2014,12/10/2014, 05/11/2014
-- Description:	Update the user's device information by user's phone number
-- =============================================
CREATE PROCEDURE [dbo].[spne_UpdateUserDeviceInfoByPhoneNumber] 
	-- Add the parameters for the stored procedure here
	@phoneNumber nvarchar(64), 
	@devicePlatform tinyint,
	@deviceVenderID varchar(36),
	@applicationID varchar(36),
	@deviceToken varchar(200),
	@applicationVersion varchar(15),
	@deviceModel nvarchar(50),
	@osVersion varchar(30),
	@insertUpdateAllColumns bit

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT OFF;
	DECLARE @True bit = 1;
	DECLARE @False bit = 0;
	DECLARE @DefaultIMTone tinyint = 1; -- Default IM tone\
	DECLARE @previousCount int;

	IF @deviceToken <> ''
	BEGIN
		UPDATE [dbo].[neUserExtension]
		SET [deviceToken] = ''
		WHERE [username] <> @phoneNumber AND [deviceToken] = @deviceToken;
	END

	IF EXISTS (SELECT 1 
				FROM [dbo].[neUserExtension]
				WHERE [username] = @phoneNumber)
	BEGIN
		IF @insertUpdateAllColumns = @True
		BEGIN
			
			UPDATE [dbo].[neUserExtension]
			SET	 [devicePlatform] = @devicePlatform
				,[deviceVenderID] = @deviceVenderID
				,[deviceToken] = @deviceToken
				,[applicationID] = @applicationID
				,[appVersion] = @applicationVersion
				,[deviceModel] = @deviceModel
				,[osVersion] = @osVersion
				,[modificationDate] = GETUTCDATE()
				,[imTone] = @DefaultIMTone
			WHERE [username] = @phoneNumber;

			IF @@ROWCOUNT = 0
				RETURN @False;
			ELSE
				RETURN @True;
		END
		ELSE
		BEGIN
			
			UPDATE [dbo].[neUserExtension]
			SET	 [deviceToken] = CASE WHEN @deviceToken = '' THEN [deviceToken] ELSE @deviceToken END
				,[appVersion] = CASE WHEN @applicationVersion = '' THEN [appVersion] ELSE @applicationVersion END
				,[osVersion] = CASE WHEN @osVersion = '' THEN [osVersion] ELSE @osVersion END
				,[modificationDate] = GETUTCDATE()
			WHERE [username] = @phoneNumber AND [devicePlatform] = @devicePlatform AND [deviceVenderID] = @deviceVenderID;

			IF @@ROWCOUNT = 0
				RETURN @False;
			ELSE
				RETURN @True;
		END
	END
	ELSE
	BEGIN
		IF @insertUpdateAllColumns = @True
		BEGIN

			INSERT INTO [dbo].[neUserExtension]
					([username]
					,[devicePlatform]
					,[deviceVenderID]
					,[deviceToken]
					,[applicationID]
					,[appVersion]
					,[deviceModel]
					,[osVersion]
					,[creationDate])
				VALUES
					(@phoneNumber
					,@devicePlatform
					,@deviceVenderID
					,@deviceToken
					,@applicationID
					,@applicationVersion
					,@deviceModel
					,@osVersion
					,GETUTCDATE());

			IF @@ROWCOUNT = 0
				RETURN @False;
			ELSE
				RETURN @True;

		END
	END
END

