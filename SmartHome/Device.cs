using System;
namespace SmartHome{
public abstract class Device
    {
        public Device():this("Default")
        {
            
            
        }
        public Device(string name)
        {
            this.Name=name;
            _currentState = new OffState();

        }
        public void ExecuteCommand(Command command)
        {
            System.Console.WriteLine($"Executing {command.CommandType} on device {Name}");
            
        }
        public virtual void TurnOn(){
            if(CurrentState is OffState)
            {
                CurrentState=new OnState();
            }
        }

        public virtual void TurnOff(){
            if(CurrentState is OnState)
            {
                CurrentState=new OffState();
            }
        }
        

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
        private DeviceState _currentState;
        public DeviceState CurrentState{
            get=>_currentState;
            protected set
            {
                _currentState?.OnExit(this);
                _currentState=value;
                _currentState?.OnEnter(this);
            }
        }
        

    }

}