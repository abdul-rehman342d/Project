CREATE PROCEDURE [dbo].[Northwind]

      @weburl [nvarchar](4000),

      @returnval [nvarchar](2000) OUTPUT

WITH EXECUTE AS CALLER

AS

EXTERNAL NAME [SPAssembly].[StoreProcedures].[SampleWSGet]