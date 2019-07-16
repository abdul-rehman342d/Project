USE [NeeoDashboard]
GO

/****** Object:  StoredProcedure [dbo].[spne_getAllIosRegisteredUser]    Script Date: 1/18/2015 10:03:18 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Usman
-- Create date: 01/15/2015
-- Description:	It returns phone numbers of all ios registred users
-- =============================================
CREATE PROCEDURE [dbo].[spne_GetAllIosRegisteredUser]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT username 
	FROM neUserExtension 
	WHERE devicePlatform = 1 AND deviceModel <> 'SIMULATOR' AND username <> '00989121133952';
END

GO

