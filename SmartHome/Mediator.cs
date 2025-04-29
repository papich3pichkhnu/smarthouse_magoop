using System;
namespace SmartHome
{
    public interface ISmartHomeMediator
    {
        void Register(Device device);
        void Notify(Device sender, string eventType, object eventData);
    }
}