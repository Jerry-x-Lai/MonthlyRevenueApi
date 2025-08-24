using AutoMapper;
using MonthlyRevenueApi.Dtos;
using MonthlyRevenueApi.Models;

namespace MonthlyRevenueApi.Features
{
    public class ImportMonthlyRevenueProfile : Profile
    {
        public ImportMonthlyRevenueProfile()
        {
            CreateMap<MonthlyRevenueDto, MonthlyRevenue>()
                .ForMember(dest => dest.CompanyId, opt => opt.MapFrom(src => src.CompanyId ?? string.Empty))
                .ForMember(dest => dest.ReportDate, opt => opt.MapFrom(src => src.ReportDate ?? string.Empty))
                .ForMember(dest => dest.DataYearMonth, opt => opt.MapFrom(src => src.DataYearMonth ?? string.Empty))
                .ForMember(dest => dest.Memo, opt => opt.MapFrom(src => src.Memo))
                .ForMember(dest => dest.Revenue, opt => opt.MapFrom(src => src.Revenue))
                .ForMember(dest => dest.LastMonthRevenue, opt => opt.MapFrom(src => src.LastMonthRevenue))
                .ForMember(dest => dest.LastYearMonthRevenue, opt => opt.MapFrom(src => src.LastYearMonthRevenue))
                .ForMember(dest => dest.MoMChange, opt => opt.MapFrom(src => src.MoMChange))
                .ForMember(dest => dest.YoYChange, opt => opt.MapFrom(src => src.YoYChange))
                .ForMember(dest => dest.AccRevenue, opt => opt.MapFrom(src => src.AccRevenue))
                .ForMember(dest => dest.LastYearAccRevenue, opt => opt.MapFrom(src => src.LastYearAccRevenue))
                .ForMember(dest => dest.AccChange, opt => opt.MapFrom(src => src.AccChange));

            CreateMap<MonthlyRevenueDto, Company>()
                .ForMember(dest => dest.CompanyId, opt => opt.MapFrom(src => src.CompanyId ?? string.Empty))
                .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.CompanyName ?? string.Empty))
                .ForMember(dest => dest.IndustryId, opt => opt.Ignore()); // 需後續補正

            CreateMap<MonthlyRevenueDto, Industry>()
                .ForMember(dest => dest.IndustryName, opt => opt.MapFrom(src => src.IndustryName ?? string.Empty))
                .ForMember(dest => dest.IndustryId, opt => opt.Ignore()); // 需後續補正
        }
    }
}
