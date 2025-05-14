using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartHome
{
    public interface IDeviceControl
    {
        string Name { get; }
        bool IsConnected { get; set;}
        void TurnOn();
        void TurnOff();
        void ExecuteCommand(Command command);
        DeviceState CurrentState { get; }
        void SetMediator(ISmartHomeMediator mediator);
        void RequestStatus();
    }

    public class RemoteDeviceProxy : IDeviceControl
    {
        private Device _realDevice;
        private readonly string _deviceId;
        private readonly string _accessToken;
        private readonly bool _logCommands;
        private readonly Dictionary<CommandType, AccessLevel> _commandAccessLevels;
        private AccessLevel _currentAccessLevel = AccessLevel.Basic;
        private ISmartHomeMediator _mediator;

        public string Name => _realDevice?.Name ?? $"Remote Device ({_deviceId})";
        
        public DeviceState CurrentState => _realDevice?.CurrentState ?? new OffState();

        public bool IsConnected { get; set; } = false;

        public void SetMediator(ISmartHomeMediator mediator)
        {
            _mediator = mediator;
            if (_realDevice != null)
            {
                _realDevice.SetMediator(mediator);
            }
        }

        public RemoteDeviceProxy(Device realDevice, string accessToken, bool logCommands = true)
        {
            _realDevice = realDevice;
            _deviceId = realDevice.Name;
            _accessToken = accessToken;
            _logCommands = logCommands;
            _commandAccessLevels = InitializeCommandAccessLevels();
        }

        public RemoteDeviceProxy(string deviceId, string accessToken, bool logCommands = true)
        {
            _realDevice = null;
            _deviceId = deviceId;
            _accessToken = accessToken;
            _logCommands = logCommands;
            _commandAccessLevels = InitializeCommandAccessLevels();
        }

        private Dictionary<CommandType, AccessLevel> InitializeCommandAccessLevels()
        {
            return new Dictionary<CommandType, AccessLevel>
            {
                { CommandType.TurnOn, AccessLevel.Basic },
                { CommandType.TurnOff, AccessLevel.Basic },
                { CommandType.Status, AccessLevel.Basic },
                { CommandType.SetBrightness, AccessLevel.Standard },
                { CommandType.SetColor, AccessLevel.Standard },
                { CommandType.SetTemperature, AccessLevel.Standard },
                { CommandType.SetColorTemperatureK, AccessLevel.Standard },
                { CommandType.EnableSecurityMode, AccessLevel.Admin },
                { CommandType.DisableSecurityMode, AccessLevel.Admin }
            };
        }

        public void SetAccessLevel(AccessLevel level)
        {
            _currentAccessLevel = level;
            Console.WriteLine($"Access level for {Name} set to {level}");
        }

        private void EnsureDeviceLoaded()
        {
            if (_realDevice == null)
            {
                Console.WriteLine($"Connecting to remote device {_deviceId}...");
                Task.Delay(500).Wait();
                
                _realDevice = new RemoteDeviceAdapter(_deviceId);
                
                if (_mediator != null)
                {
                    _mediator.Register(_realDevice);
                }
                
                Console.WriteLine($"Connected to remote device {_deviceId}");
            }
        }

        private bool CheckAccessPermission(CommandType commandType)
        {
            if (!_commandAccessLevels.TryGetValue(commandType, out var requiredLevel))
            {
                requiredLevel = AccessLevel.Admin; 
            }

            bool hasAccess = (int)_currentAccessLevel >= (int)requiredLevel;
            
            if (!hasAccess)
            {
                Console.WriteLine($"Access denied: {commandType} requires {requiredLevel} access level, but current level is {_currentAccessLevel}");
            }
            
            return hasAccess;
        }

        private bool AuthenticateRequest()
        {
            bool isAuthenticated = !string.IsNullOrEmpty(_accessToken);
            
            if (!isAuthenticated)
            {
                Console.WriteLine("Authentication failed: Invalid access token");
            }
            
            return isAuthenticated;
        }

        public void TurnOn()
        {
            if (!AuthenticateRequest() || !CheckAccessPermission(CommandType.TurnOn))
            {
                return;
            }
            
            if (_logCommands)
            {
                Console.WriteLine($"[LOG] Turn On command received for {_deviceId} at {DateTime.Now}");
            }
            
            try
            {
                EnsureDeviceLoaded();
                
                Console.WriteLine($"Sending TurnOn command to {_deviceId}...");
                Task.Delay(200).Wait();
                
                _realDevice.TurnOn();
                
                Console.WriteLine($"[LOG] Device {_deviceId} turned on successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to turn on device {_deviceId}: {ex.Message}");
            }
        }

        public void TurnOff()
        {
            if (!AuthenticateRequest() || !CheckAccessPermission(CommandType.TurnOff))
            {
                return;
            }
            
            if (_logCommands)
            {
                Console.WriteLine($"[LOG] Turn Off command received for {_deviceId} at {DateTime.Now}");
            }
            
            try
            {
                EnsureDeviceLoaded();
                
                Console.WriteLine($"Sending TurnOff command to {_deviceId}...");
                Task.Delay(200).Wait();
                
                _realDevice.TurnOff();
                
                Console.WriteLine($"[LOG] Device {_deviceId} turned off successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to turn off device {_deviceId}: {ex.Message}");
            }
        }

        public void ExecuteCommand(Command command)
        {
            if (!AuthenticateRequest() || !CheckAccessPermission(command.CommandType))
            {
                return;
            }
            
            if (_logCommands)
            {
                string paramInfo = command.Parameters != null && command.Parameters.Length > 0 
                    ? string.Join(", ", command.Parameters) 
                    : "none";
                
                Console.WriteLine($"[LOG] Command {command.CommandType} received for {_deviceId} with parameters: {paramInfo} at {DateTime.Now}");
            }
            
            try
            {
                EnsureDeviceLoaded();
                
                Console.WriteLine($"Sending {command.CommandType} command to {_deviceId}...");
                Task.Delay(200).Wait();
                
                _realDevice.ExecuteCommand(command);
                
                Console.WriteLine($"[LOG] Command {command.CommandType} executed successfully on device {_deviceId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to execute command {command.CommandType} on device {_deviceId}: {ex.Message}");
            }
        }

        public void RequestStatus()
        {
            if (!AuthenticateRequest() || !CheckAccessPermission(CommandType.Status))
            {
                return;
            }
            
            if (_logCommands)
            {
                Console.WriteLine($"[LOG] Status request received for {_deviceId} at {DateTime.Now}");
            }
            
            try
            {
                EnsureDeviceLoaded();
                
                Console.WriteLine($"Sending Status request to {_deviceId}...");
                Task.Delay(200).Wait();
                
                _realDevice.RequestStatus();
                
                Console.WriteLine($"[LOG] Status request executed successfully on device {_deviceId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to request status from device {_deviceId}: {ex.Message}");
            }
        }
    }

    public class RemoteDeviceAdapter : Device
    {
        public RemoteDeviceAdapter(string name) : base(name)
        {
            ConnectToNetwork();
        }

        protected override void HandleSpecificCommand(Command command)
        {
            Console.WriteLine($"Remote device handling command: {command.CommandType}");
        }

        protected override void ExecuteMainFunction()
        {
            Console.WriteLine("Remote device executing main function");
        }

        public override Device Clone()
        {
            return new RemoteDeviceAdapter(this.Name);
        }
    }

    public enum AccessLevel
    {
        Basic = 0,
        Standard = 1,
        Admin = 2
    }
} 