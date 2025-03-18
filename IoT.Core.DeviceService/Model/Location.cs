namespace IoT.Core.DeviceService.Model
{
    public record Location(string Country, string Province, string District)
    {
        public bool isEqual(Location location)
        {
            return !this.Country.Equals(location.Country) ||
                   !this.Province.Equals(location.Province) ||
                   !this.District.Equals(location.District);
        }
    }
}
