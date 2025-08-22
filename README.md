# MonthlyRevenueApi

此專案為 .NET 6 Web API，提供上市公司每月營業收入查詢與匯入功能。

## 主要功能
- 依公司代號查詢營收資料 API
- 匯入 CSV 並寫入 MSSQL 資料庫 API
- 整合 MediatR、AutoMapper
- Action Filter 全域錯誤處理/Log
- Swagger 查詢頁面

## 套件
- Microsoft.EntityFrameworkCore.SqlServer 6.0.x
- MediatR.Extensions.Microsoft.DependencyInjection
- AutoMapper.Extensions.Microsoft.DependencyInjection
- Swashbuckle.AspNetCore 6.2.x

## 使用
1. 於 appsettings.json 設定 MSSQL 連線字串
2. 執行 `dotnet run` 啟動 API
3. 透過 Swagger UI 測試 API

## 匯入資料
- 透過 API 上傳政府開放平台下載的 csv 檔案

---
