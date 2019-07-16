-- ================================================
-- Template generated from Template Explorer using:
-- Create Procedure (New Menu).SQL
--
-- Use the Specify Values for Template Parameters 
-- command (Ctrl-Shift-M) to fill in the parameter 
-- values below.
--
-- This block of comments will not be included in
-- the definition of the procedure.
-- ================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:	Zohaib Hanif
-- Create date: 10/02/2014
-- Description:	The following procedure implements the time based blocking of the user.
-- =============================================
CREATE PROCEDURE [dbo].[spne_GetUserBlockingStateByPhoneNumber] 
	-- Add the parameters for the stored procedure here
	@phoneNumber nvarchar(64),
	@userState int output
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	DECLARE @userAttemptCount tinyint;
	DECLARE @oldRequestDate datetime;
	DECLARE @currentRequestDate datetime = GETUTCDATE();
	DECLARE @requestTimeDifference int;
	DECLARE @TotalBlockingTime int = 1440; -- in minutes

	IF EXISTS (SELECT 1 
			   FROM [dbo].[neBlockedUser] 
			   WHERE [phoneNumber] = @phoneNumber)
	BEGIN
		SET @userAttemptCount = (SELECT [neBlockedUser].attemptsCount 
								 FROM [dbo].[neBlockedUser] 
								 WHERE [phoneNumber] = @phoneNumber);

		SET @oldRequestDate = (SELECT [neBlockedUser].requestDate 
							   FROM [dbo].[neBlockedUser] 
							   WHERE [phoneNumber] = @phoneNumber);
		SET @requestTimeDifference = DATEDIFF(MINUTE,@oldRequestDate,@currentRequestDate);
		IF (DATEDIFF(MINUTE,@oldRequestDate,@currentRequestDate) <= @TotalBlockingTime)
		BEGIN
			IF @userAttemptCount != 2
			BEGIN
				UPDATE [dbo].[neBlockedUser]
				SET [attemptsCount] = 2
				   ,[requestDate] = @currentRequestDate
				WHERE [phoneNumber] = @phoneNumber;

				IF @@ROWCOUNT = 1
					SET @userState = 0;
			END
			ELSE
			BEGIN
				--SET @userState = 0;
				--SET @userState = 1;--User is blocked
				-- if user is blocked, return the remaining time to unblock user.
				SET @userState = @requestTimeDifference - @TotalBlockingTime;
			END;
			
		END
		ELSE
		BEGIN
			IF @userAttemptCount = 2
			BEGIN
				UPDATE [dbo].[neBlockedUser]
				SET [attemptsCount] = 1
				   ,[requestDate] = @currentRequestDate
				WHERE [phoneNumber] = @phoneNumber;

				IF @@ROWCOUNT = 1
					SET @userState = 0;
			END
			ELSE
			BEGIN
				UPDATE [dbo].[neBlockedUser]
				SET [requestDate] = @currentRequestDate
				WHERE [phoneNumber] = @phoneNumber;

				IF @@ROWCOUNT = 1
					SET @userState = 0;
			END;

		END;

	END
	ELSE
	BEGIN
		INSERT INTO [dbo].[neBlockedUser]
			([phoneNumber]
			,[requestDate]
			,[attemptsCount])
		VALUES
			(@PhoneNumber
			,@currentRequestDate
			,1);

		IF @@ROWCOUNT = 1
			SET @userState = 0;
	END;
END
GO
