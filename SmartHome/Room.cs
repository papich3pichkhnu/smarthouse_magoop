using System;
using System.Diagnostics.Contracts;
namespace SmartHome
{
    public class Room
    {
        private List<Device> _devices = new List<Device>();
        private Dictionary<string,List<Device>> _devicesByTypeList=new Dictionary<string, List<Device>>();
        public string Name {get; set;}
        public void AddDevice(Device device)=>_devices.Add(device);
        public void RemoveDevice(Device device)=>_devices.Remove(device);
        public void AddDeviceByType(Device device,string deviceType)
        {
            if(!_devicesByTypeList.ContainsKey(deviceType))
                _devicesByTypeList[deviceType]=new List<Device>();
            _devicesByTypeList[deviceType].Add(device);

        }
        public Dictionary<string,List<Device>> getDevicesByTypeList()
        {
            return _devicesByTypeList;
        }
        public List<Device> GetDevices()=>_devices;
        public ISmartHomeIterator CreateIterator()=>new SmartHomeIterator(this);
        
        public Room(string name)
        {
            Name=name;
        }

    }
}