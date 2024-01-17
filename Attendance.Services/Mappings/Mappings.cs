using Attendance.Models;
using Attendance.Services.DTOs;
using AutoMapper;
using Operations.Models;
using System.Linq;

namespace Attendance.Services.Mappings
{
    public class AutoMapping : Profile
    {
        public AutoMapping()
        {
            CreateMap<Vessel, VesselDTO>()
                .ForMember(_ => _.Id, s => s.MapFrom(_ => _.VesselId))
                .ForMember(_ => _.Name, s => s.MapFrom(_ => _.VesselName))
            ;
            CreateMap<InspectionTypes, InspectionTypeDTO>()
                .ForMember(_ => _.Id, s => s.MapFrom(_ => _.InspectionTypeId))
                .ForMember(_ => _.Name, s => s.MapFrom(_ => _.InspectionType))
            ;
            CreateMap<InspectionSource, InspectionSourceDTO>()
                .ForMember(_ => _.Id, s => s.MapFrom(_ => _.InspectionSourceId))
                .ForMember(_ => _.Name, s => s.MapFrom(_ => _.SourceName))
            ;
            CreateMap<Port, PortDTO>()
                .ForMember(_ => _.Id, s => s.MapFrom(_ => _.Id))
                .ForMember(_ => _.Code, s => s.MapFrom(_ => _.Code))
                .ForMember(_ => _.Name, s => s.MapFrom(_ => _.Name))
                .ForMember(_ => _.Area, s => s.MapFrom(_ => _.Country.Region.Area.Name))
                .ForMember(_ => _.Region, s => s.MapFrom(_ => _.Country.Region.Name))
                .ForMember(_ => _.Country, s => s.MapFrom(_ => _.Country.Name))
                .ForMember(_ => _.CountryCode, s => s.MapFrom(_ => _.Country.Alpha2))
                .ForMember(_ => _.Lat, s => s.MapFrom(_ => _.Lat))
                .ForMember(_ => _.Lng, s => s.MapFrom(_ => _.Lng))
                ;
            CreateMap<VIQInfoModel, VIQInfoDTO>()
                ;
            CreateMap<User, UserDTO>()
                .ForMember(_ => _.FullName, s => s.MapFrom(_ => _.FirstName + " " + _.LastName))
                ;
            CreateMap<Briefcase, BriefcaseDTO>()
                .ForMember(_ => _.Questionnaires, s => s.MapFrom(_ => _.Questionnaires.Select(_ => _.Questionnaire)))
                ;
            CreateMap<Briefcase, BriefcasePayloadDTO>()
                .ForMember(_ => _.Questionnaires, s => s.MapFrom(_ => _.Questionnaires.Select(_ => _.QId)))
                ;
            CreateMap<BriefcaseQuestionnaire, BriefcaseQuestionnaireDTO>()
                ;
        }
    }
}



