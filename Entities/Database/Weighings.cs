using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Metadata;

namespace Entities.Database
{
    public class Truck
    {
        /// <summary>
        /// Номер автомобиля
        /// </summary>
        [Column("transport_number")] public string Number { get; set; } = null!;
        [Column("stock_weight")] public string? TrailerNumber { get; set; }
        [Column("stock_weight")] public int StockWeight { get; set; }
        [Column("max_allowed_difference")] public int MaxAllowedDifference { get; set; }
        [Column("use_as_tara")] public bool UseAsTara { get; set; }
        [Column("capacity")] public int Capacity { get; set; }
        [Column("volume")] public decimal Volume { get; set; }
        [Column("rfid")] public Blob RFID { get; set; }
        [Column("vehicle_id")] public int VehicleId { get; set; }
        [Column("driver_id")] public int DriverId { get; set; } 
        [Column("is_deleted")] public bool IsDeleted { get; set; }
        [Column("name")] public string? DriverName { get; set; }
    }
    public class Weighings
    {
        [Column("id")] public int Id { get; set; }
        [Column("datetime_first")] public DateTime DatetimeFirst { get; set; }
        [Column("transport_number")] public string? TransportNumber { get; set; }
        [Column("trailer_number")] public string? TrailerNumber { get; set; }
        [Column("original_number")] public string? OriginalNumber { get; set; }
        [Column("comment")] public string? Comment { get; set; }
        [Column("confidence")] public float Confidence { get; set; }
        [Column("trailer_confidence")] public float TrailerConfidence { get; set; }
        [Column("inn")] public string Inn { get; set; }
        [Column("operator")] public string? Operator { get; set; }
        [Column("direction")] public int Direction { get; set; }
        [Column("weight")] public int Weight { get; set; }
        [Column("correct_weight")] public int CorrectWeight { get; set; }
        [Column("scales_number")] public int ScalesNumber { get; set; }
        [Column("id_screenshot")] public int? IdScreenshot { get; set; }
        [Column("id_trailer_screenshot")] public int? IdTrailerScreenshot { get; set; }
        [Column("work_mode")] public int? WorkMode { get; set; }
        [Column("controlweight_mode")] public byte ControlweightMode { get; set; }
        [Column("date_stable_weight")] public DateTime? DateStableWeight { get; set; }
        [Column("stable_weight")] public int? StableWeight { get; set; }
        [Column("date_max_weight")] public DateTime DateMaxWeight { get; set; }
        [Column("max_weight")] public int MaxWeight { get; set; }
        [Column("end_datetime")] public DateTime EndDatetime { get; set; }
        [Column("sended_contragent_id")] public int? SendedContragentId { get; set; }
        [Column("sended_delivery_address_id")] public int? SendedDeliveryAddressId { get; set; }
        [Column("recipient_contragent_id")] public int? RecipientContragentId { get; set; }
        [Column("recipient_delivery_address_id")] public int? RecipientDeliveryAddressId { get; set; }
        [Column("cargo_id")] public int? CargoId { get; set; }
        [Column("driver_id")] public int? DriverId { get; set; }
        [Column("car_type_id")] public int? CarTypeId { get; set; }
        [Column("car_brand_id")] public int? CarBrandId { get; set; }
        [Column("trailer_id")] public int? TrailerId { get; set; }
        [Column("carrier_id")] public int? CarrierId { get; set; }
        [Column("is_expeditor")] public byte IsExpeditor { get; set; }
        [Column("type_ownership")] public int TypeOwnership { get; set; }
        [Column("id_user")] public int? IdUser { get; set; }
        [Column("recognized_material_id")] public int? RecognizedMaterialId { get; set; }
        [Column("main_camera_id")] public int? MainCameraId { get; set; }
        [Column("left_camera_id")] public int? LeftCameraId { get; set; }
        [Column("right_camera_id")] public int? RightCameraId { get; set; }
        [Column("recognize_left_camera_id")] public int? RecognizeLeftCameraId { get; set; }
        [Column("recognize_right_camera_id")] public int? RecognizeRightCameraId { get; set; }
        [Column("work_car_id")] public int? WorkCarId { get; set; }
        [Column("stock_weight")] public int? StockWeight { get; set; }
        [Column("max_allowed_difference")] public int? MaxAllowedDifference { get; set; }
        [Column("use_stock_weight_as_tara")] public byte UseStockWeightAsTara { get; set; }
        [Column("weight_jump_id")] public int? WeightJumpId { get; set; }
        [Column("isdeleted")] public byte Isdeleted { get; set; }
    }
}
