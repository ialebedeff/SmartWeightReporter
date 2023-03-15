using AutoMapper;
using Dapper.FluentMap.Mapping;
using Entities;
using Entities.Database;
using Microsoft.AspNetCore.Components.Authorization;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using System.Security.Claims;
using Unidecode.NET;

namespace EntitiesMapper
{
    public class TruckProfile : EntityMap<Truck>
    { 
        public TruckProfile() 
        {
            Map(t => t.TransportNumber).ToColumn("transport_number");
            Map(t => t.TrailerNumber).ToColumn("trailer_number");
            Map(t => t.StockWeight).ToColumn("stock_weight");
            Map(t => t.MaxAllowedDifference).ToColumn("max_allowed_difference");
            Map(t => t.UseAsTara).ToColumn("use_as_tara");
            Map(t => t.Capacity).ToColumn("capacity");
            Map(t => t.Volume).ToColumn("volume");
           
            Map(t => t.VehicleId).ToColumn("vehicle_id");
            Map(t => t.DriverId).ToColumn("driver_id");
        }
    }

    public class DriverProfile : EntityMap<Driver> 
    {
        public DriverProfile() 
        {
            Map(d => d.Id).ToColumn("id");
            Map(d => d.Name).ToColumn("name");
        }
    }

    public class VehicleProfile : EntityMap<Vehicle> 
    {
        public VehicleProfile() 
        {
            Map(v => v.Id).ToColumn("Vehicle_ID");
            Map(v => v.ModelId).ToColumn("MODEL_ID_FK");
            Map(v => v.Height).ToColumn("height");
            Map(v => v.Lenght).ToColumn("length");
            Map(v => v.Width).ToColumn("width");
            Map(v => v.Note).ToColumn("Note");
        }
    }

    public class BrandProfile : EntityMap<Brand> 
    {
        public BrandProfile()
        {
            Map(b => b.Id).ToColumn($"CarBrand_ID");
            Map(b => b.Name).ToColumn($"Brand");
        }
    }

    public class CarTypesProfile : EntityMap<CarType> 
    {
        public CarTypesProfile()
        {
            Map(b => b.Id).ToColumn("id");
            Map(b => b.Name).ToColumn("name");
        }
    }

    public class ModelProfile : EntityMap<Model> 
    {
        public ModelProfile()
        {
            Map(m => m.Id).ToColumn("Model_ID");
            Map(m => m.Name).ToColumn("MODEL");
        }
    }
    public class WeighingsProfile : EntityMap<Weighings>
    {
        public WeighingsProfile() 
        {
            var properties = typeof(Weighings).GetProperties();
            
            Map(w => w.Id).ToColumn(GetColumnName(properties, "Id"));
            Map(w => w.UseStockWeightAsTara).ToColumn(GetColumnName(properties, "UseStockWeightAsTara"));
            Map(w => w.Inn).ToColumn(GetColumnName(properties, "Inn"));
            Map(w => w.TrailerConfidence).ToColumn(GetColumnName(properties, "TrailerConfidence"));
            Map(w => w.Confidence).ToColumn(GetColumnName(properties, "Confidence"));
            Map(w => w.CarBrandId).ToColumn(GetColumnName(properties, "CarBrandId"));
            Map(w => w.CargoId).ToColumn(GetColumnName(properties, "CargoId"));
            Map(w => w.CarrierId).ToColumn(GetColumnName(properties, "CarrierId"));
            Map(w => w.CarTypeId).ToColumn(GetColumnName(properties, "CarTypeId"));
            Map(w => w.Comment).ToColumn(GetColumnName(properties, "Comment"));
            Map(w => w.ControlweightMode).ToColumn(GetColumnName(properties, "ControlweightMode"));
            Map(w => w.CorrectWeight).ToColumn(GetColumnName(properties, "CorrectWeight"));
            Map(w => w.DateMaxWeight).ToColumn(GetColumnName(properties, "DateMaxWeight"));
            Map(w => w.DateStableWeight).ToColumn(GetColumnName(properties, "DateStableWeight"));
            Map(w => w.DatetimeFirst).ToColumn(GetColumnName(properties, "DatetimeFirst"));
            Map(w => w.Direction).ToColumn(GetColumnName(properties, "Direction"));
            Map(w => w.DriverId).ToColumn(GetColumnName(properties, "DriverId"));
            Map(w => w.EndDatetime).ToColumn(GetColumnName(properties, "EndDatetime"));
            Map(w => w.IdScreenshot).ToColumn(GetColumnName(properties, "IdScreenshot"));
            Map(w => w.IdTrailerScreenshot).ToColumn(GetColumnName(properties, "IdTrailerScreenshot"));
            Map(w => w.IdUser).ToColumn(GetColumnName(properties, "IdUser"));
            Map(w => w.Isdeleted).ToColumn(GetColumnName(properties, "Isdeleted"));
            Map(w => w.IsExpeditor).ToColumn(GetColumnName(properties, "IsExpeditor"));
            Map(w => w.Weight).ToColumn("weight");
            Map(w => w.StableWeight).ToColumn("stable_weight");
            Map(w => w.StockWeight).ToColumn("stock_weight");
            Map(w => w.MaxWeight).ToColumn("max_weight");
        }

        private string GetColumnName(IEnumerable<PropertyInfo> propertyInfos, string propertyName)
            => propertyInfos.First(property => property.Name == propertyName).GetCustomAttribute<ColumnAttribute>()?.Name ?? string.Empty;
    }
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