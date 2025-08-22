CREATE TYPE [dbo].[MonthlyRevenueTableType] AS TABLE
(
    [ReportDate] NVARCHAR(10),
    [DataYearMonth] NVARCHAR(6),
    [CompanyId] NVARCHAR(10),
    [Revenue] DECIMAL(18,2),
    [LastMonthRevenue] DECIMAL(18,2) NULL,
    [LastYearMonthRevenue] DECIMAL(18,2) NULL,
    [MoMChange] DECIMAL(18,15) NULL,
    [YoYChange] DECIMAL(18,15) NULL,
    [AccRevenue] DECIMAL(18,2) NULL,
    [LastYearAccRevenue] DECIMAL(18,2) NULL,
    [AccChange] DECIMAL(18,15) NULL,
    [Memo] NVARCHAR(200) NULL
)
GO

CREATE PROCEDURE [dbo].[ImportMonthlyRevenueBatch]
    @MonthlyRevenueList [dbo].[MonthlyRevenueTableType] READONLY,
    @OutString NVARCHAR(8) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        MERGE INTO [dbo].[MonthlyRevenue] AS Target
        USING @MonthlyRevenueList AS Source
        ON Target.CompanyId = Source.CompanyId AND Target.DataYearMonth = Source.DataYearMonth
        WHEN MATCHED THEN
            UPDATE SET
                Target.ReportDate = Source.ReportDate,
                Target.Revenue = Source.Revenue,
                Target.LastMonthRevenue = Source.LastMonthRevenue,
                Target.LastYearMonthRevenue = Source.LastYearMonthRevenue,
                Target.MoMChange = Source.MoMChange,
                Target.YoYChange = Source.YoYChange,
                Target.AccRevenue = Source.AccRevenue,
                Target.LastYearAccRevenue = Source.LastYearAccRevenue,
                Target.AccChange = Source.AccChange,
                Target.Memo = Source.Memo
        WHEN NOT MATCHED THEN
            INSERT (
                ReportDate, DataYearMonth, CompanyId, Revenue, LastMonthRevenue, LastYearMonthRevenue,
                MoMChange, YoYChange, AccRevenue, LastYearAccRevenue, AccChange, Memo
            )
            VALUES (
                Source.ReportDate, Source.DataYearMonth, Source.CompanyId, Source.Revenue, Source.LastMonthRevenue, Source.LastYearMonthRevenue,
                Source.MoMChange, Source.YoYChange, Source.AccRevenue, Source.LastYearAccRevenue, Source.AccChange, Source.Memo
            );
        SET @OutString = '00000000';
    END TRY
    BEGIN CATCH
        SET @OutString = '99999999';
    END CATCH
END
GO
