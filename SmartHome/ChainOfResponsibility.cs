using System;
namespace SmartHome
{
    public abstract class CommandProcessor
    {
        protected CommandProcessor _nextProcessor;
        protected SmartHomeController _controller;
        public CommandProcessor(SmartHomeController controller)
        {
            _controller = controller;
        }
        public CommandProcessor SetNext(CommandProcessor nextProcessor)
        {
            _nextProcessor=nextProcessor;
            return nextProcessor;
        }
        public virtual void ProcessCommand(Command command)
        {
            if(_nextProcessor!=null)
            {
                _nextProcessor.ProcessCommand(command);
            }
        }
        protected abstract bool CanProcessCommand(Command command);
    }
    public class StatusProcessor : CommandProcessor
    {
        public StatusProcessor(SmartHomeController controller) : base(controller){}
        protected override bool CanProcessCommand(Command command)
        {
            if (command.CommandType==CommandType.Status)
            {
                var device = _controller.FindDevice(command.targetName);
                if(device!=null)
                {
                    device.RequestStatus();
                    return true;
                }
            }
            return false;
        }
        
    }
    public class FunctionalCommandProcessor : CommandProcessor
    {
        public FunctionalCommandProcessor(SmartHomeController controller):base(controller){}
        protected override bool CanProcessCommand(Command command)
        {
            if(command.CommandType!=CommandType.TurnOn &&
               command.CommandType!=CommandType.TurnOff &&
               command.CommandType!=CommandType.Status)
               {
                var device=_controller.FindDevice(command.targetName);
                if(device!=null)
                {
                    device.ExecuteCommand(command);
                    return true;
                }
               }
               return false;
        }
    }
    public class SmartHomeController
    {
        private CommandProcessor _commandChain;
        private Room room;
        public SmartHomeController()
        {
            

        }
        public void AddDevice(Device device)
        {
            room.AddDevice(device);
            System.Console.WriteLine($"Added {device.Name} to room {room.Name}");
        }
        public Device FindDevice(string name)
        {
            ISmartHomeIterator iterator=room.CreateIterator();
            while(iterator.HasNext())
            {
                var device=iterator.Next();
                if(device.Name==name)
            {
                return device;
            }
            }
            System.Console.WriteLine($"Device {name} not found");
            return null;
        }
        public void ExecuteCommand(Command command)
        {
            System.Console.WriteLine($"Executing command {command.CommandType} on {command.targetName}");
            _commandChain.ProcessCommand(command);   
        }
    }
}