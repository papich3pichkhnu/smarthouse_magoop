using System;
namespace SmartHome
{
    interface ICommand
    {
        void Execute();
        void Undo();
    }

    class TurnOnCommand : ICommand
    {
        private Device _device;
        public TurnOnCommand(Device device)
        {
            _device = device;
        }
        public void Execute()
        {
            _device.TurnOn();
        }
        public void Undo()
        {
            _device.TurnOff();
        }

    }

    class TurnOffCommand : ICommand
    {
        private Device _device;
        public TurnOffCommand(Device device)
        {
            _device = device;
        }
        public void Execute()
        {
            _device.TurnOff();
        }
        public void Undo()
        {
            _device.TurnOn();
        }
    }

    class SetHalfBrightnessCommand:ICommand
    {
        private Lamp _device;
        private int old_val;
        public SetHalfBrightnessCommand(Lamp device)
        {
            _device = device;
            old_val=device.Brightness;
            
        }
        public void Execute()
        {
            _device.Brightness=50;
            System.Console.WriteLine($"Setting lamp {this._device.Name} brightness to 50");
        }
        public void Undo()
        {
            _device.Brightness=old_val;
            System.Console.WriteLine($"Setting lamp {this._device.Name} brightness to {old_val}");
        }
    }
    class RemoteCommandInvoker
    {
        private List<ICommand> _commands=new List<ICommand>();
        public void AddCommand(ICommand command)
        {
            _commands.Add(command);
        }
        public void ExecuteCommands()
        {
            foreach (var c in _commands)
            {
                c?.Execute();                
            }
            _commands.Clear();
        }
    }
    class MacroCommand:ICommand
    {
        private List<ICommand> _commands;
        public MacroCommand(List<ICommand> commands)
        {
            _commands = commands;
        }
        public void AddCommand(ICommand command)
        {
            _commands.Add(command);
        }

        public void Execute()
        {
            foreach(ICommand c in _commands)
            {
                c?.Execute();
            }
        }

        public void Undo()
        {
            foreach(ICommand c in _commands)
            {
                c?.Undo();
            }
        }
    }
}