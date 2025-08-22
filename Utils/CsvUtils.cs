using CsvHelper;
using CsvHelper.Configuration;
using MonthlyRevenueApi.Dtos;
using System.Globalization;
using System.Text;

namespace MonthlyRevenueApi.Utils
{
    public static class CsvUtils
    {
        public static IEnumerable<MonthlyRevenueDto> ParseMonthlyRevenue(Stream csvStream, CultureInfo? numberCulture = null)
        {
            numberCulture ??= CultureInfo.GetCultureInfo("zh-TW");
            var percentCulture = CultureInfo.InvariantCulture;
            var config = new CsvConfiguration(numberCulture)
            {
                HasHeaderRecord = true,
                PrepareHeaderForMatch = args => RemoveBom(args.Header)?.Trim()?.ToLowerInvariant(),
                TrimOptions = TrimOptions.Trim,
                Delimiter = ",",
                DetectDelimiter = false,
                IgnoreBlankLines = true,
                MissingFieldFound = null,
                BadDataFound = null,
                HeaderValidated = null,
            };
            using var reader = new StreamReader(csvStream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true, leaveOpen: true);
            using var csv = new CsvReader(reader, config);
            csv.Context.RegisterClassMap(new MonthlyRevenueMap(numberCulture, percentCulture));
            while (csv.Read())
            {
                MonthlyRevenueDto? row = null;
                try { row = csv.GetRecord<MonthlyRevenueDto>(); } catch { }
                if (row != null) yield return row;
            }
        }
        private static string? RemoveBom(string? s)
        {
            if (string.IsNullOrEmpty(s)) return s;
            return s.TrimStart('\uFEFF');
        }
        private sealed class MonthlyRevenueMap : ClassMap<MonthlyRevenueDto>
        {
            public MonthlyRevenueMap(CultureInfo numberCulture, CultureInfo percentCulture)
            {
                Map(m => m.ReportDate).Name("出表日期");
                Map(m => m.DataYearMonth).Name("資料年月");
                Map(m => m.CompanyId).Name("公司代號");
                Map(m => m.CompanyName).Name("公司名稱");
                Map(m => m.IndustryName).Name("產業別");
                Map(m => m.Revenue).Name("營業收入-當月營收").TypeConverterOption.CultureInfo(numberCulture).TypeConverterOption.NumberStyles(NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands).Default(0m);
                Map(m => m.LastMonthRevenue).Name("營業收入-上月營收").TypeConverterOption.CultureInfo(numberCulture).TypeConverterOption.NumberStyles(NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands);
                Map(m => m.LastYearMonthRevenue).Name("營業收入-去年當月營收").TypeConverterOption.CultureInfo(numberCulture).TypeConverterOption.NumberStyles(NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands);
                Map(m => m.MoMChange).Name("營業收入-上月比較增減(%)").TypeConverterOption.CultureInfo(percentCulture).TypeConverterOption.NumberStyles(NumberStyles.Float | NumberStyles.AllowLeadingSign);
                Map(m => m.YoYChange).Name("營業收入-去年同月增減(%)").TypeConverterOption.CultureInfo(percentCulture).TypeConverterOption.NumberStyles(NumberStyles.Float | NumberStyles.AllowLeadingSign);
                Map(m => m.AccRevenue).Name("累計營業收入-當月累計營收").TypeConverterOption.CultureInfo(numberCulture).TypeConverterOption.NumberStyles(NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands);
                Map(m => m.LastYearAccRevenue).Name("累計營業收入-去年累計營收").TypeConverterOption.CultureInfo(numberCulture).TypeConverterOption.NumberStyles(NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands);
                Map(m => m.AccChange).Name("累計營業收入-前期比較增減(%)").TypeConverterOption.CultureInfo(percentCulture).TypeConverterOption.NumberStyles(NumberStyles.Float | NumberStyles.AllowLeadingSign);
                Map(m => m.Memo).Name("備註");
            }
        }
    }
}
