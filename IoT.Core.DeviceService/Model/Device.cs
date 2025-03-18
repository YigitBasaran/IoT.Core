using Microsoft.AspNetCore.Components;

namespace IoT.Core.DeviceService.Model;

public class Device
{
    public Guid Id { get; set; }
    public string DevEUI { get; set; }
    public string Name { get; set; }
    public int CustomerId { get; set; }
    public Location Location { get; set; }

    public Device(Guid id, string devEui, string name, int customerId, Location location)
    {
        this.Id = id;
        this.DevEUI = devEui;
        this.Name = name;
        this.CustomerId = customerId;
        this.Location = location;
    }

    public static Device OnCreate(string devEui, string name, int customerId, Location location)
    {
        return new Device(Guid.NewGuid(), devEui, name, customerId, location);
    }

    public bool isDirty(Device updatedDevice)
    {
        return !this.DevEUI.Equals(updatedDevice.DevEUI) || 
               !this.Name.Equals(updatedDevice.Name) || 
               !this.CustomerId.Equals(updatedDevice.CustomerId) ||
               !this.Location.isEqual(updatedDevice.Location);
    }
}