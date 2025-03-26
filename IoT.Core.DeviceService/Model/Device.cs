using System.Runtime.CompilerServices;
using IoT.Core.CommonInfrastructure.Database.Repo;
using Microsoft.AspNetCore.Components;

namespace IoT.Core.DeviceService.Model;

public class Device : BaseEntity<string>
{
    public string Name { get; set; }
    public int ClientId { get; set; }
    public Location Location { get; set; }

    public Device(string devEui, string name, int clientId, Location location)
    {
        this.Id = devEui;
        this.Name = name;
        this.ClientId = clientId;
        this.Location = location;
    }

    public static Device OnCreate(string devEui, string name, int clientId, Location location)
    {
        var device = new Device(devEui, name, clientId, location);
        device.CreatedAt = DateTime.UtcNow;
        return device;
    }

    public void OnUpdateName(string newName)
    {
        this.Name = newName;
        this.UpdatedAt = DateTime.UtcNow;
    }

    public void OnUpdateLocation(Location newLocation)
    {
        this.Location = newLocation;
        this.UpdatedAt = DateTime.UtcNow;
    }
}