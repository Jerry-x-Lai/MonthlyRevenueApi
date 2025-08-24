
CREATE PROCEDURE [dbo].[ImportMonthlyRevenueBatch]
    @IndustryList [dbo].[IndustryTableType] READONLY,
    @CompanyList [dbo].[CompanyTableType] READONLY,
    @MonthlyRevenueList [dbo].[MonthlyRevenueTableType] READONLY,
    @OutString NVARCHAR(8) OUTPUT
AS
BEGIN
    SET XACT_ABORT ON;
    BEGIN TRY
        BEGIN TRAN

        -- Industry
        INSERT INTO Industry (IndustryName)
        SELECT s.IndustryName
        FROM @IndustryList s
        WHERE NOT EXISTS (SELECT 1 FROM Industry t WHERE t.IndustryName = s.IndustryName);

        -- Company
        INSERT INTO Company (CompanyId, CompanyName, IndustryId)
        SELECT s.CompanyId, s.CompanyName, s.IndustryId
        FROM @CompanyList s
        WHERE NOT EXISTS (SELECT 1 FROM Company t WHERE t.CompanyId = s.CompanyId);

        -- Upsert MonthlyRevenue
        UPDATE t
        SET t.ReportDate = s.ReportDate,
            t.Revenue = s.Revenue,
            t.LastMonthRevenue = s.LastMonthRevenue,
            t.LastYearMonthRevenue = s.LastYearMonthRevenue,
            t.MoMChange = s.MoMChange,
            t.YoYChange = s.YoYChange,
            t.AccRevenue = s.AccRevenue,
            t.LastYearAccRevenue = s.LastYearAccRevenue,
            t.AccChange = s.AccChange,
            t.Memo = s.Memo
        FROM MonthlyRevenue t
        INNER JOIN @MonthlyRevenueList s ON t.CompanyId = s.CompanyId AND t.DataYearMonth = s.DataYearMonth;

        INSERT INTO MonthlyRevenue (ReportDate, DataYearMonth, CompanyId, Revenue, LastMonthRevenue, LastYearMonthRevenue, MoMChange, YoYChange, AccRevenue, LastYearAccRevenue, AccChange, Memo)
        SELECT s.ReportDate, s.DataYearMonth, s.CompanyId, s.Revenue, s.LastMonthRevenue, s.LastYearMonthRevenue, s.MoMChange, s.YoYChange, s.AccRevenue, s.LastYearAccRevenue, s.AccChange, s.Memo
        FROM @MonthlyRevenueList s
        WHERE NOT EXISTS (SELECT 1 FROM MonthlyRevenue t WHERE t.CompanyId = s.CompanyId AND t.DataYearMonth = s.DataYearMonth);

        COMMIT
        SET @OutString = '00000000';
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK
        SET @OutString = '99999999';
    END CATCH
END
GO
