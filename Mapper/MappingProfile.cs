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
        CreateMap<Property, PropertyResponse>();
        CreateMap<CreateEnquiryRequest, Enquiry>();
        CreateMap<CreatePropertyRequest, Property>();
        CreateMap<Enquiry, EnquiryResponse>();
    }
}
