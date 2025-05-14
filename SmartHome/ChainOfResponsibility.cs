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
            _nextProcessor = nextProcessor;
            return nextProcessor;
        }
        public virtual void ProcessCommand(Command command)
        {
            if (!CanProcessCommand(command) && _nextProcessor != null)
            {
                _nextProcessor.ProcessCommand(command);
            }

        }
        protected abstract bool CanProcessCommand(Command command);
    }
    public class StatusProcessor : CommandProcessor
    {
        public StatusProcessor(SmartHomeController controller) : base(controller) { }
        protected override bool CanProcessCommand(Command command)
        {
            if (command.CommandType == CommandType.Status)
            {
                var device = _controller.FindIDevice(command.targetName);
                if (device != null)
                {
                    device.RequestStatus();
                    return true;
                }
            }
            return false;
        }

    }
    
    public class SecurityCommandProcessor : CommandProcessor
    {
        public SecurityCommandProcessor(SmartHomeController controller) : base(controller) { }
        
        protected override bool CanProcessCommand(Command command)
        {
            if (command.CommandType == CommandType.EnableSecurityMode || 
                command.CommandType == CommandType.DisableSecurityMode)
            {
                if (command.CommandType == CommandType.EnableSecurityMode)
                {
                    _controller.EnableSecurityMode();
                }
                else
                {
                    _controller.DisableSecurityMode();
                }
                return true;
            }
            return false;
        }
    }
    
    public class FunctionalCommandProcessor : CommandProcessor
    {
        public FunctionalCommandProcessor(SmartHomeController controller) : base(controller) { }
        protected override bool CanProcessCommand(Command command)
        {
            if (command.CommandType != CommandType.TurnOn &&
               command.CommandType != CommandType.TurnOff &&
               command.CommandType != CommandType.Status &&
               command.CommandType != CommandType.EnableSecurityMode &&
               command.CommandType != CommandType.DisableSecurityMode)
            {
                var device = _controller.FindIDevice(command.targetName);
                if (device != null)
                {
                    device.ExecuteCommand(command);
                    return true;
                }
            }
            return false;
        }
    }
    public class LoggerProcessor : CommandProcessor
    {
        public LoggerProcessor(SmartHomeController controller) : base(controller)
        {
        }

        protected override bool CanProcessCommand(Command command)
        {
            return true;
        }
        public override void ProcessCommand(Command command)
        {
            _controller.LogCommand(command);
            if (_nextProcessor != null)
            {
                _nextProcessor.ProcessCommand(command);
            }
        }

    }

    public class SmartHomeController : ISmartHomeMediator
    {
        private CommandProcessor _commandChain;
        private Dictionary<string, Room> _rooms = new Dictionary<string, Room>();
        private List<String> logs = new List<string>();
        private SecurityManager _securityManager;
        
        public SmartHomeController(string mainRoomName)
        {
            _rooms = new Dictionary<string, Room>
            {
                { mainRoomName, new Room(mainRoomName) }
            };
            _securityManager = new SecurityManager();

            var loggerProcessor = new LoggerProcessor(this);
            var statusProcessor = new StatusProcessor(this);
            var securityProcessor = new SecurityCommandProcessor(this);
            var functionProcessor = new FunctionalCommandProcessor(this);
            _commandChain = loggerProcessor;
            loggerProcessor.SetNext(statusProcessor).SetNext(securityProcessor).SetNext(functionProcessor);
        }
        
        public void AddRoom(string roomName)
        {
            if (!_rooms.ContainsKey(roomName))
            {
                _rooms[roomName] = new Room(roomName);
                Console.WriteLine($"Room '{roomName}' added to the smart home controller");
            }
            else
            {
                Console.WriteLine($"Room '{roomName}' already exists");
            }
        }
        
        public Room GetRoom(string roomName)
        {
            if (_rooms.ContainsKey(roomName))
            {
                return _rooms[roomName];
            }
            
            Console.WriteLine($"Room '{roomName}' not found");
            return null;
        }
        
        public void AddDevice(IDeviceControl device, string roomName = null)
        {
            // If no room specified, use the first room
            if (string.IsNullOrEmpty(roomName))
            {
                roomName = _rooms.Keys.FirstOrDefault();
                if (string.IsNullOrEmpty(roomName))
                {
                    Console.WriteLine("No rooms available to add device");
                    return;
                }
            }
            
            if (!_rooms.ContainsKey(roomName))
            {
                Console.WriteLine($"Room '{roomName}' not found. Device not added.");
                return;
            }
            
            string deviceType = "none";
            if (device is Lamp) deviceType = "lamp";
            else if (device is MotionSensor) deviceType = "motionSensor";
            else if (device is Thermostat) deviceType = "thermostat";
            
            _rooms[roomName].AddDevice(device);
            _rooms[roomName].AddDeviceByType(device, deviceType);
            this.Register(device);
            
            if (device is MotionSensor motionSensor)
            {
                _securityManager.RegisterMotionSensor(motionSensor);
            }
            
            System.Console.WriteLine($"Added {device.Name} to room {roomName}");
        }
        
        public IDeviceControl FindIDevice(string name)
        {
            name = name.ToLower();
            
            foreach (var room in _rooms.Values)
            {
                ISmartHomeIterator iterator = room.CreateIterator();
                while (iterator.HasNext())
                {
                    var device = iterator.Next();
                    if (device.Name.ToLower() == name)
                    {
                        return device;
                    }
                }
            }
            
            System.Console.WriteLine($"Device {name} not found");
            return null;
        }
        
        public Device FindDevice(string name)
        {
            var device = FindIDevice(name);
            if (device is Device actualDevice)
            {
                return actualDevice;
            }
            else if (device != null)
            {
                System.Console.WriteLine($"Device {name} found but is not a Device instance");
            }
            return null;
        }
        
        public void ExecuteCommand(Command command)
        {
            System.Console.WriteLine($"Executing command {command.CommandType} on {command.targetName}");
            _commandChain.ProcessCommand(command);
        }

        public void EnableSecurityMode()
        {
            _securityManager.EnableSecurityMode(this, _rooms);
        }
        
        public void DisableSecurityMode()
        {
            _securityManager.DisableSecurityMode(_rooms);
        }
        
        public void SendCommand(CommandType commandType, string device, params object[] parameters)
        {
            var command = new Command(commandType, device, parameters);
            ExecuteCommand(command);
        }
        
        public void LogCommand(Command command)
        {
            var logstr = $"command {command.CommandType} on {command.targetName}";
            System.Console.WriteLine($"Logging {logstr}");
            logs.Add(logstr);
        }
        
        public List<string> GetLogs()
        {
            return logs;
        }
        
        public void InterpretCommand(string command)
        {
            CommandParser parser = new CommandParser();
            SmartHomeContext context = new SmartHomeContext(this);

            try
            {
                var expression = parser.Parse(command);
                if (expression != null)
                {
                    expression.Interpret(context);
                }
                else
                {
                    Console.WriteLine("Couldn't understand the command: " + command);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error interpreting command: {ex.Message}");
            }
        }
        
        public void Register(IDeviceControl device)
        {
            device.SetMediator(this);
            Console.WriteLine($"Device {device.Name} registered with the mediator");
        }
        
        public void Notify(IDeviceControl sender, string eventType, object eventData)
        {
            Device deviceSender = sender as Device;
            
            Room deviceRoom = null;
            foreach (var room in _rooms.Values)
            {
                if (room.GetDevices().Contains(sender))
                {
                    deviceRoom = room;
                    break;
                }
            }
            
            if (deviceRoom == null)
            {
                Console.WriteLine("Device not found in any room");
                return;
            }
            
            switch (eventType)
            {
                case "MotionDetected":
                    if (deviceSender != null)
                    {
                        HandleMotionDetected(deviceSender, eventData, deviceRoom);
                    }
                    break;
                case "TemperatureChanged":
                    if (deviceSender != null)
                    {
                        HandleTemperatureChanged(deviceSender, eventData, deviceRoom);
                    }
                    break;
            }
        }
        
        private void HandleMotionDetected(Device sender, object eventData, Room deviceRoom)
        {
            var devicesByType = deviceRoom.getDevicesByTypeList();
            if (devicesByType.ContainsKey("lamp"))
            {
                foreach (var device in devicesByType["lamp"])
                {
                    ExecuteCommand(new Command(CommandType.TurnOn, device.Name));
                }
            }
            
            if (_securityManager.IsEnabled)
            {
                _securityManager.OnMotionDetected(sender, eventData);
            }
        }
        
        private void HandleTemperatureChanged(Device sender, object eventData, Room deviceRoom)
        {
            var devicesByType = deviceRoom.getDevicesByTypeList();
            if (devicesByType.ContainsKey("thermostat"))
            {
                foreach (var device in devicesByType["thermostat"])
                {
                    ExecuteCommand(new Command(CommandType.TurnOff, device.Name, eventData));
                }
            }
        }
        

        private class SecurityManager
        {
            private readonly Dictionary<string, Room.RoomMemento> _securityStates;
            private readonly List<MotionSensor> _motionSensors;
            
            public SecurityManager()
            {
                _securityStates = new Dictionary<string, Room.RoomMemento>();
                _motionSensors = new List<MotionSensor>();
                IsEnabled = false;
            }
            
            public bool IsEnabled { get; private set; }
            
            public void RegisterMotionSensor(MotionSensor sensor)
            {
                if (!_motionSensors.Contains(sensor))
                {
                    _motionSensors.Add(sensor);
                }
            }
            
            public void EnableSecurityMode(SmartHomeController controller, Dictionary<string, Room> rooms)
            {
                if (IsEnabled)
                {
                    Console.WriteLine("Security mode is already enabled");
                    return;
                }
                
                foreach (var room in rooms.Values)
                {
                    var memento = room.CreateMemento();
                    _securityStates[room.Name] = memento;
                    
                    foreach (var device in room.GetDevices())
                    {
                        if (device is Lamp lamp)
                        {
                            lamp.TurnOff();
                        }
                        
                        if (device is MotionSensor sensor)
                        {
                            sensor.TurnOn();
                        }
                    }
                }
                
                IsEnabled = true;
                Console.WriteLine("Security mode enabled");
            }
            
            public void DisableSecurityMode(Dictionary<string, Room> rooms)
            {
                if (!IsEnabled)
                {
                    Console.WriteLine("Security mode is already disabled");
                    return;
                }
                
                foreach (var room in rooms.Values)
                {
                    if (_securityStates.TryGetValue(room.Name, out var memento))
                    {
                        room.RestoreMemento(memento);
                    }
                }
                
                IsEnabled = false;
                Console.WriteLine("Security mode disabled");
            }
            
            public void OnMotionDetected(Device sender, object eventData)
            {
                if (IsEnabled && (bool)eventData)
                {
                    Console.WriteLine("SECURITY ALERT: Motion detected while security system is enabled!");
                }
            }
        }

        public ICollection<Room> GetAllRooms()
        {
            return _rooms.Values;
        }
    }
}