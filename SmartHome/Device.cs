using System;
namespace SmartHome{
public abstract class Device
    {
        public abstract void TurnOn();

        public abstract void TurnOff();

        protected void ConnectToNetwork()
        {
            System.Console.WriteLine($"Device {Name} connected to network.");
            IsConnected=true;
        }
        protected void Initialize()
        {
            System.Console.WriteLine($"Device {Name} initialized.");
        }
        protected abstract void ExecuteMainFunction();
        public void OperateDevice()
        {
            ConnectToNetwork();
            Initialize();
            TurnOn();
            ExecuteMainFunction();
            TurnOff();
        }
        public abstract Device Clone();
        public string Name { get; set; } = "Device";
        public bool IsConnected {get;set;}=false;
        

    }

}