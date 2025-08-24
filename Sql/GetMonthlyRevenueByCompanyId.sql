-- 查詢 SP
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetMonthlyRevenueByCompanyId]
	@CompanyId NVARCHAR(10) = NULL,
	@OutString NVARCHAR(8) OUTPUT
AS
BEGIN
	BEGIN TRY
		SELECT m.*, c.CompanyName, i.IndustryName
		FROM MonthlyRevenue m
		LEFT JOIN Company c ON m.CompanyId = c.CompanyId
		LEFT JOIN Industry i ON c.IndustryId = i.IndustryId
		WHERE @CompanyId IS NULL OR m.CompanyId = @CompanyId
		SET @OutString = '00000000';
	END TRY
	BEGIN CATCH
		SET @OutString = '99999999';
	END CATCH
END