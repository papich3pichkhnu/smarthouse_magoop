using System;
namespace SmartHome{
public abstract class Device
    {
        public abstract void TurnOn();

        public abstract void TurnOff();

        public abstract Device Clone();
        public string Name { get; set; } = "Device";
        

    }

}