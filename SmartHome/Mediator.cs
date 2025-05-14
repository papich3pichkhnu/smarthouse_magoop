using System;
namespace SmartHome
{
    public interface ISmartHomeMediator
    {
        void Register(IDeviceControl device);
        void Notify(IDeviceControl sender, string eventType, object eventData);
    }
}