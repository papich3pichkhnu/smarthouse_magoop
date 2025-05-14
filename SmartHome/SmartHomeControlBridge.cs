using System;
using System.Collections.Generic;

namespace SmartHome
{
    public interface ISmartHomeControlImplementation
    {
        void ExecuteAction(string targetDevice, CommandType commandType, params object[] parameters);
        string GetDeviceStatus(string targetDevice);
        List<string> GetAllDevices();
        bool Connect();
        void Disconnect();
        void ProcessNaturalLanguageCommand(string command);
    }

    public class VoiceControl : SmartHomeControlAbstraction
    {
        public VoiceControl(ISmartHomeControlImplementation implementation) : base(implementation)
        {
        }

        public override void ProcessNaturalLanguageCommand(string command)
        {
            Console.WriteLine($"Voice command received: \"{command}\"");
            
            _implementation.ProcessNaturalLanguageCommand(command);
        }

        public override void Initialize()
        {
            Console.WriteLine("Initializing voice control interface...");
            if (_implementation.Connect())
            {
                Console.WriteLine("Voice control ready. You can now speak commands.");
            }
            else
            {
                Console.WriteLine("Failed to initialize voice control.");
            }
        }

        public override void Shutdown()
        {
            Console.WriteLine("Shutting down voice control interface...");
            _implementation.Disconnect();
        }
    }

    public class MobileAppControl : SmartHomeControlAbstraction
    {
        public MobileAppControl(ISmartHomeControlImplementation implementation) : base(implementation)
        {
        }

        public void ExecuteAppCommand(string device, string commandName, params object[] parameters)
        {
            Console.WriteLine($"Mobile app command received: {commandName} for device {device}");
            
            switch (commandName.ToLower())
            {
                case "turnon":
                    ExecuteCommand(CommandType.TurnOn, device);
                    break;
                case "turnoff":
                    ExecuteCommand(CommandType.TurnOff, device);
                    break;
                case "setbrightness":
                    if (parameters.Length > 0 && parameters[0] is int brightness)
                    {
                        ExecuteCommand(CommandType.SetBrightness, device, brightness);
                    }
                    break;
                case "settemperature":
                    if (parameters.Length > 0 && parameters[0] is double temperature)
                    {
                        ExecuteCommand(CommandType.SetTemperature, device, temperature);
                    }
                    break;
                case "getstatus":
                    string status = GetStatus(device);
                    Console.WriteLine($"Mobile app: Status of {device} is {status}");
                    break;
                default:
                    Console.WriteLine($"Mobile app: Unknown command {commandName}");
                    break;
            }
        }

        public List<string> GetAvailableDevices()
        {
            return _implementation.GetAllDevices();
        }

        public override void Initialize()
        {
            Console.WriteLine("Initializing mobile app control interface...");
            if (_implementation.Connect())
            {
                Console.WriteLine("Mobile app control ready.");
            }
            else
            {
                Console.WriteLine("Failed to initialize mobile app control.");
            }
        }

        public override void Shutdown()
        {
            Console.WriteLine("Shutting down mobile app control interface...");
            _implementation.Disconnect();
        }

        public override void ProcessNaturalLanguageCommand(string command)
        {
            Console.WriteLine($"Mobile app does not process natural language. Received: {command}");
        }
    }

    public abstract class SmartHomeControlAbstraction
    {
        protected readonly ISmartHomeControlImplementation _implementation;
        
        public SmartHomeControlAbstraction(ISmartHomeControlImplementation implementation)
        {
            _implementation = implementation;
        }
        
        public void ExecuteCommand(CommandType commandType, string device, params object[] parameters)
        {
            _implementation.ExecuteAction(device, commandType, parameters);
        }
        
        public string GetStatus(string device)
        {
            return _implementation.GetDeviceStatus(device);
        }
        
        public abstract void Initialize();
        public abstract void Shutdown();
        public abstract void ProcessNaturalLanguageCommand(string command);
    }

    public class LocalControlImplementation : ISmartHomeControlImplementation
    {
        private readonly SmartHomeSystem _smartHomeSystem;
        private readonly SmartHomeController _controller;
        private bool _isConnected;
        
        public LocalControlImplementation(SmartHomeSystem smartHomeSystem)
        {
            _smartHomeSystem = smartHomeSystem;
            _controller = smartHomeSystem.Controller;
            _isConnected = false;
        }
        
        public LocalControlImplementation(SmartHomeController controller)
        {
            _controller = controller;
            _smartHomeSystem = null;
            _isConnected = false;
        }
        
        public bool Connect()
        {
            Console.WriteLine("Connecting to local smart home system...");
            _isConnected = true;
            return _isConnected;
        }
        
        public void Disconnect()
        {
            Console.WriteLine("Disconnecting from local smart home system...");
            _isConnected = false;
        }
        
        public void ExecuteAction(string targetDevice, CommandType commandType, params object[] parameters)
        {
            if (!_isConnected)
            {
                Console.WriteLine("Not connected to smart home system. Please connect first.");
                return;
            }
            
            if (_controller != null)
            {
                try {
                    var command = new Command(commandType, targetDevice, parameters);
                    _controller.ExecuteCommand(command);
                    return;
                }
                catch (Exception ex) {
                    Console.WriteLine($"Error executing command via controller: {ex.Message}");
                }
            }
            
            if (_smartHomeSystem != null)
            {
                FallbackExecuteAction(targetDevice, commandType, parameters);
            }
            else
            {
                Console.WriteLine($"Cannot execute action: no access to device '{targetDevice}'");
            }
        }
        
        private void FallbackExecuteAction(string targetDevice, CommandType commandType, params object[] parameters)
        {
            foreach (var room in GetAllRooms())
            {
                var devices = _smartHomeSystem.GetRoom(room)?.GetDevices();
                if (devices == null) continue;
                
                foreach (var device in devices)
                {
                    if (device.Name.Equals(targetDevice, StringComparison.OrdinalIgnoreCase))
                    {
                        var command = new Command(commandType, targetDevice, parameters);
                        device.ExecuteCommand(command);
                        return;
                    }
                }
            }
            
            Console.WriteLine($"Device '{targetDevice}' not found.");
        }
        
        public string GetDeviceStatus(string targetDevice)
        {
            if (!_isConnected)
            {
                return "Not connected to smart home system";
            }
            
            if (_controller != null)
            {
                var device = _controller.FindDevice(targetDevice);
                if (device != null)
                {
                    return $"State: {device.CurrentState.GetType().Name}, Connected: {device.IsConnected}";
                }
            }
            
            if (_smartHomeSystem != null)
            {
                foreach (var room in GetAllRooms())
                {
                    var devices = _smartHomeSystem.GetRoom(room)?.GetDevices();
                    if (devices == null) continue;
                    
                    foreach (var d in devices)
                    {
                        if (d.Name.Equals(targetDevice, StringComparison.OrdinalIgnoreCase))
                        {
                            return $"State: {d.CurrentState.GetType().Name}, Connected: {d.IsConnected}";
                        }
                    }
                }
            }
            
            return $"Device '{targetDevice}' not found";
        }
        
        public List<string> GetAllDevices()
        {
            var devices = new List<string>();
            
            if (!_isConnected)
            {
                return devices;
            }
            
            if (_smartHomeSystem != null)
            {
                foreach (var room in GetAllRooms())
                {
                    var roomDevices = _smartHomeSystem.GetRoom(room)?.GetDevices();
                    if (roomDevices == null) continue;
                    
                    foreach (var device in roomDevices)
                    {
                        devices.Add(device.Name);
                    }
                }
            }
            
            return devices;
        }
        
        private List<string> GetAllRooms()
        {
            return new List<string> { "Living Room", "Kitchen", "Bedroom" };
        }
        
        public void ProcessNaturalLanguageCommand(string command)
        {
            if (_controller != null)
            {
                Console.WriteLine($"Processing command with SmartHomeController: \"{command}\"");
                _controller.InterpretCommand(command);
            }
            else
            {
                Console.WriteLine("SmartHomeController is not available");
                throw new NotSupportedException("Natural language processing is not available without SmartHomeController");
            }
        }
    }

    public class RemoteControlImplementation : ISmartHomeControlImplementation
    {
        private readonly string _cloudApiUrl;
        private readonly string _apiKey;
        private bool _isConnected;
        private readonly CommandParser _parser;
        
        public RemoteControlImplementation(string cloudApiUrl, string apiKey)
        {
            _cloudApiUrl = cloudApiUrl;
            _apiKey = apiKey;
            _isConnected = false;
            _parser = new CommandParser();
        }
        
        public bool Connect()
        {
            Console.WriteLine($"Connecting to smart home cloud service at {_cloudApiUrl}...");
            Console.WriteLine("Authenticating with API key...");
            
            _isConnected = true;
            return _isConnected;
        }
        
        public void Disconnect()
        {
            Console.WriteLine("Disconnecting from smart home cloud service...");
            _isConnected = false;
        }
        
        public void ExecuteAction(string targetDevice, CommandType commandType, params object[] parameters)
        {
            if (!_isConnected)
            {
                Console.WriteLine("Not connected to cloud service. Please connect first.");
                return;
            }
            
            Console.WriteLine($"Sending {commandType} command to {targetDevice} via cloud API at {_cloudApiUrl}");
            
            System.Threading.Thread.Sleep(300);
            
            Console.WriteLine($"Command {commandType} executed on {targetDevice} via cloud service");
        }
        
        public string GetDeviceStatus(string targetDevice)
        {
            if (!_isConnected)
            {
                return "Not connected to cloud service";
            }
            
            Console.WriteLine($"Requesting status of {targetDevice} from cloud API...");
            
            System.Threading.Thread.Sleep(200);
            
            return $"State: Online, Cloud-Connected: True";
        }
        
        public List<string> GetAllDevices()
        {
            var devices = new List<string>();
            
            if (!_isConnected)
            {
                return devices;
            }
            
            Console.WriteLine("Fetching all devices from cloud API...");
            
            System.Threading.Thread.Sleep(500);
            
            devices.Add("Living Room Light");
            devices.Add("Kitchen Light");
            devices.Add("Bedroom Light");
            devices.Add("Living Room Thermostat");
            
            return devices;
        }
        
        public void ProcessNaturalLanguageCommand(string command)
        {
            if (!_isConnected)
            {
                Console.WriteLine("Not connected to cloud service. Please connect first.");
                throw new InvalidOperationException("Cannot process commands while disconnected");
            }
            
            Console.WriteLine($"Sending voice command to cloud NLP service: \"{command}\"");
            
            System.Threading.Thread.Sleep(500);
            
            try
            {
                var virtualContext = new VirtualSmartHomeContext();
                
                var expression = _parser.Parse(command);
                if (expression != null)
                {
                    expression.Interpret(virtualContext);
                    
                    foreach (var capturedCommand in virtualContext.CapturedCommands)
                    {
                        Console.WriteLine($"Cloud NLP interpreted: {capturedCommand.CommandType} for {capturedCommand.targetName}");
                        ExecuteAction(capturedCommand.targetName, capturedCommand.CommandType, capturedCommand.Parameters);
                    }
                }
                else
                {
                    Console.WriteLine("Cloud NLP could not understand the command");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing command: {ex.Message}");
                Console.WriteLine("Natural language command could not be processed");
            }
        }
    }

    public class VirtualSmartHomeContext : SmartHomeContext
    {
        public List<Command> CapturedCommands { get; } = new List<Command>();
        
        public VirtualSmartHomeContext() : base(null)
        {
        }
        
        public override void ExecuteCommand(Command command)
        {
            CapturedCommands.Add(command);
        }
        
        public override Device FindDevice(string deviceName)
        {
            return new VirtualDevice(deviceName);
        }
    }

    public class VirtualDevice : Device
    {
        public VirtualDevice(string name) : base(name)
        {
        }
        
        protected override void HandleSpecificCommand(Command command)
        {
            
        }
        
        protected override void ExecuteMainFunction()
        {
        }
        
        public override Device Clone()
        {
            return new VirtualDevice(this.Name);
        }
    }
} 