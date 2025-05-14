using System;
using System.Diagnostics.Contracts;
namespace SmartHome
{
    public class Room
    {
        private List<IDeviceControl> _devices = new List<IDeviceControl>();
        private Dictionary<string, List<IDeviceControl>> _devicesByTypeList = new Dictionary<string, List<IDeviceControl>>();
        public string Name {get; set;}
        
        public void AddDevice(IDeviceControl device) => _devices.Add(device);
        public void RemoveDevice(IDeviceControl device) => _devices.Remove(device);
        
        public void AddDeviceByType(IDeviceControl device, string deviceType)
        {
            if(!_devicesByTypeList.ContainsKey(deviceType))
                _devicesByTypeList[deviceType] = new List<IDeviceControl>();
            _devicesByTypeList[deviceType].Add(device);
        }
        
        public Dictionary<string, List<IDeviceControl>> getDevicesByTypeList()
        {
            return _devicesByTypeList;
        }
        
        public List<IDeviceControl> GetDevices() => _devices;
        
        public ISmartHomeIterator CreateIterator() => new SmartHomeIterator(this);
        
        public Room(string name)
        {
            Name = name;
        }

        public void Accept(ISmartHomeVisitor visitor)
        {
            visitor.Visit(this);
            foreach(var device in _devices)
            {
                if (device is Device actualDevice)
                {
                    actualDevice.Accept(visitor);
                }
            }
        }

        public RoomMemento CreateMemento()
        {
            var deviceMementos = new List<Device.DeviceMemento>();
            foreach (var device in _devices)
            {
                if (device is Device actualDevice)
                {
                    deviceMementos.Add(actualDevice.CreateMemento());
                }
            }
            return new RoomMemento(deviceMementos);
        }

        public void RestoreMemento(RoomMemento memento)
        {
            if (memento != null)
            {
                var deviceMementos = memento.DeviceMementos;
                
                var mementosByName = new Dictionary<string, Device.DeviceMemento>();
                foreach (var deviceMemento in deviceMementos)
                {
                    mementosByName[deviceMemento.Name] = deviceMemento;
                }
                
                foreach (var device in _devices)
                {
                    if (device is Device actualDevice && 
                        mementosByName.TryGetValue(actualDevice.Name, out var deviceMemento))
                    {
                        actualDevice.RestoreMemento(deviceMemento);
                    }
                }
            }
        }

        public class RoomMemento
        {
            public List<Device.DeviceMemento> DeviceMementos { get; }

            public RoomMemento(List<Device.DeviceMemento> deviceMementos)
            {
                DeviceMementos = deviceMementos;
            }
        }
    }
}