using System;
namespace SmartHome{
public abstract class Device
    {
        protected ISmartHomeMediator mediator;
        public void SetMediator(ISmartHomeMediator mediator)
        {
            this.mediator=mediator;
            mediator.Register(this);
        }
        public Device():this("Default")
        {
            
            
        }
        public Device(string name)
        {
            this.Name=name;
            CurrentState = new OffState();

        }
        public virtual void ExecuteCommand(Command command)
        {
            System.Console.WriteLine($"Executing {command.CommandType} on device {Name}");
            if(_currentState.CanHandleCommand(command.CommandType))
            {
                switch(command.CommandType)
                {
                    case CommandType.TurnOn:
                        this.TurnOn();
                        break;
                    case CommandType.TurnOff:
                        this.TurnOff();
                        break;
                    case CommandType.Status:
                        this.RequestStatus();
                        break;
                    default:
                        HandleSpecificCommand(command);
                        break;
                }
            }
            
        }
        public virtual void RequestStatus()
        {
            System.Console.WriteLine($"Device {Name} status: {_currentState.GetType().Name}");
        }
        public virtual void ReportError(string error)
        {
            System.Console.WriteLine($"Error on device {Name}: {error}");
            CurrentState=new ErrorState(error);
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
        protected abstract void HandleSpecificCommand(Command command);
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

        public DeviceMemento CreateMemento()
        {
            return new DeviceMemento(Name, CurrentState);
        }

        public void RestoreMemento(DeviceMemento memento)
        {
            if (memento != null)
            {
                Name = memento.Name;
                CurrentState = memento.State;
            }
        }

        public class DeviceMemento
        {
            public string Name { get; }
            public DeviceState State { get; }

            public DeviceMemento(string name, DeviceState state)
            {
                Name = name;
                State = state;
            }
        }
        

    }

}