using System;
namespace SmartHome
{
    public abstract class DeviceState
    {
        public virtual void OnEnter(Device device){}
        public virtual void OnExit(Device device){}        
        public abstract bool CanHandleCommand(CommandType command);
        
    }

    public class OffState:DeviceState
    {
        public override void OnEnter(Device device)
        {
            System.Console.WriteLine($"Device {device.Name} is now powered off");
        }
        public override bool CanHandleCommand(CommandType command)
        {
            return command==CommandType.PowerOn || command==CommandType.Status;
        }
    }
    public class OnState:DeviceState
    {
        public override void OnEnter(Device device)
        {
            System.Console.WriteLine($"Device {device.Name} is now powered on");
        }
        public override bool CanHandleCommand(CommandType command)
        {
            return command!=CommandType.PowerOn;
        }
                
    }
    public class ErrorState:DeviceState
    {
        private string _errorMessage;
        public ErrorState(string errorMessage="unknown error")=>errorMessage=_errorMessage;
        public override void OnEnter(Device device)
        {
            System.Console.WriteLine($"Device {device.Name} entered error state: {_errorMessage}");            
        }
        public override bool CanHandleCommand(CommandType command)
        {
            return command==CommandType.PowerOff || command==CommandType.Status;
        }
    }
    public class ActiveState:DeviceState
    {
        private readonly string _activity;
        public ActiveState(string activity) 
        {
            _activity=activity;
        }
        public override void OnEnter(Device device)
        {
            System.Console.WriteLine($"Device {device.Name} is now active with activity {_activity}");            
        }
        public override bool CanHandleCommand(CommandType command)
        {
            return command!=CommandType.PowerOn;
        }
    }
}