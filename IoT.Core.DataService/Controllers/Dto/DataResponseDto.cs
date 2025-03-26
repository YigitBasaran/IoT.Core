namespace IoT.Core.DataService.Controllers.Dto
{
    public record DataResponseDto(int ClientId, string DevEui, string Payload, DateTime CreatedAt);
}
