using AutoMapper;
using MonthlyRevenueApi.Models;
using MonthlyRevenueApi.Dtos;

namespace MonthlyRevenueApi.Features
{
    public class MonthlyRevenueQueryProfile : Profile
    {
        public MonthlyRevenueQueryProfile()
        {
            CreateMap<MonthlyRevenue, MonthlyRevenueQueryDto>()
                .ForMember(dest => dest.CompanyId, opt => opt.MapFrom(src => src.CompanyId))
                .ForMember(dest => dest.DataYearMonth, opt => opt.MapFrom(src => src.DataYearMonth))
                .ForMember(dest => dest.ReportDate, opt => opt.MapFrom(src => src.ReportDate))
                .ForMember(dest => dest.Revenue, opt => opt.MapFrom(src => src.Revenue))
                .ForMember(dest => dest.LastMonthRevenue, opt => opt.MapFrom(src => src.LastMonthRevenue))
                .ForMember(dest => dest.LastYearMonthRevenue, opt => opt.MapFrom(src => src.LastYearMonthRevenue))
                .ForMember(dest => dest.MoMChange, opt => opt.MapFrom(src => src.MoMChange))
                .ForMember(dest => dest.YoYChange, opt => opt.MapFrom(src => src.YoYChange))
                .ForMember(dest => dest.AccRevenue, opt => opt.MapFrom(src => src.AccRevenue))
                .ForMember(dest => dest.LastYearAccRevenue, opt => opt.MapFrom(src => src.LastYearAccRevenue))
                .ForMember(dest => dest.AccChange, opt => opt.MapFrom(src => src.AccChange))
                .ForMember(dest => dest.Memo, opt => opt.MapFrom(src => src.Memo))
                .ForMember(dest => dest.CompanyName, opt => opt.Ignore())
                .ForMember(dest => dest.IndustryName, opt => opt.Ignore());
        }
    }
}
