using System;
using System.Diagnostics.Contracts;
namespace SmartHome
{
    public class Room
    {
        private List<Device> _devices = new List<Device>();
        public string Name {get; set;}
        public void AddDevice(Device device)=>_devices.Add(device);
        public void RemoveDevice(Device device)=>_devices.Remove(device);
        public List<Device> GetDevices()=>_devices;
        public ISmartHomeIterator CreateIterator()=>new SmartHomeIterator(this);
        
        public Room(string name)
        {
            Name=name;
        }

    }
}