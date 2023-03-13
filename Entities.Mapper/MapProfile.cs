using AutoMapper;
using Entities;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using System.Xml.Serialization;
using Unidecode.NET;

namespace EntitiesMapper
{
    public class MapProfile : Profile
    {
        public MapProfile()
        {
            this.CreateMap<CreateFactoryUserRequest, User>();
            this.CreateMap<string, Note>()
                .ForMember(src => src.Text, opt => opt.MapFrom(desc => desc))
                .ForMember(src => src.CreateTime, opt => opt.MapFrom(desc => DateTime.UtcNow));

            // ToDo: переработать хардкод
            this.CreateMap<FactoryDto, Factory>()
                .ForMember(src => src.DatabaseConnection, opt => opt.MapFrom(desc => new DatabaseConnection()
                {
                    Database = "shark_vesy1",
                    Password = "1q2w3e4r5T",
                    Server = "localhost",
                    User = "root"
                }));
            this.CreateMap<CreateNoteRequest, Note>();

            // ToDo: переработать хардкод
            this.CreateMap<string, Factory>()
                .ForMember(src => src.Title, opt => opt.MapFrom(desc => desc))
                .ForMember(src => src.DatabaseConnection, opt => opt.MapFrom(desc => new DatabaseConnection() 
                {
                    Database = "shark_vesy1",
                    Password = "1q2w3e4r5T",
                    Server = "localhost",
                    User = "root"
                }));

            this.CreateMap<string, User>()
                .ForMember(src => src.UserName, 
                opt => opt.MapFrom(desc => 
                desc.Unidecode(null)
                    .Replace(" ", "_")));

            this.CreateMap<ClaimsIdentity, IdentityData>()
                .ForMember(src => src.Claims, opt => opt.MapFrom(desc => desc.Claims))
                .ForMember(src => src.IsAuthenticated, opt => opt.MapFrom(desc => desc.IsAuthenticated))
                .ForMember(src => src.Name, opt => opt.MapFrom(desc => desc.Name))
                .ForMember(src => src.AuthenticationType, opt => opt.MapFrom(desc => desc.AuthenticationType));

            this.CreateMap<Claim, ClaimData>()
                .ForMember(src => src.Issuer, opt => opt.MapFrom(desc => desc.Issuer))
                .ForMember(src => src.Value, opt => opt.MapFrom(desc => desc.Value))
                .ForMember(src => src.ValueType, opt => opt.MapFrom(desc => desc.ValueType))
                .ForMember(src => src.Type, opt => opt.MapFrom(desc => desc.Type))
                .ForMember(src => src.OriginalIssuer, opt => opt.MapFrom(desc => desc.OriginalIssuer)).ReverseMap();

            this.CreateMap<AuthenticationState, AuthenticationData>()
                .ForMember(src => src.User, opt => opt.MapFrom(desc => desc.User)).ReverseMap();
        }
    }
}