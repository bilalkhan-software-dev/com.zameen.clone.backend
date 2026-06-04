using System.Text.Json;
using AutoMapper;
using com.zameen.Models;
using com.zameen.Models.Dto.Request;
using com.zameen.Models.Dto.Response;

namespace com.zameen.Mapper;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Agent, AgentResponse>();
        CreateMap<Property, PropertyResponse>()
            .ForMember(
                dest => dest.Amenities,
                opt =>
                    opt.MapFrom(src =>
                        JsonSerializer.Deserialize<Dictionary<string, object>>(src.AmenitiesJson)
                        ?? new Dictionary<string, object>()
                    )
            );
        CreateMap<CreatePropertyRequest, Property>()
            .ForMember(
                dest => dest.AmenitiesJson,
                opt => opt.MapFrom(src => JsonSerializer.Serialize(src.Amenities))
            );
        CreateMap<CreateEnquiryRequest, Enquiry>();

        CreateMap<Enquiry, EnquiryResponse>();
    }
}
