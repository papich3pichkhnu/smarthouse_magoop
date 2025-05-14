using System;
using System.Collections.Generic;
using System.Linq;

namespace SmartHome
{

    public class SmartHomeSystem
    {
        private readonly SmartHomeController _controller;
        private readonly SmartHomeHistory<SmartHomeMemento> _stateHistory;
        private readonly Dictionary<string, SmartHomeMemento> _namedStates;
        private readonly SecurityManager _securityManager;
        private readonly List<RemoteDeviceProxy> _remoteDevices;
        private readonly string _systemAccessToken;
        
        public SmartHomeController Controller => _controller;
        
        public SmartHomeSystem(string mainRoomName = "Living Room")
        {
            _controller = new SmartHomeController(mainRoomName);
            _stateHistory = new SmartHomeHistory<SmartHomeMemento>();
            _namedStates = new Dictionary<string, SmartHomeMemento>();
            _securityManager = new SecurityManager();
            _remoteDevices = new List<RemoteDeviceProxy>();
            _systemAccessToken = GenerateSystemAccessToken();
        }
        
        private string GenerateSystemAccessToken()
        {
            return Guid.NewGuid().ToString("N");
        }
        
        public void AddRoom(string roomName)
        {
            _controller.AddRoom(roomName);
        }
        
        public Room GetRoom(string roomName)
        {
            return _controller.GetRoom(roomName);
        }
        
        public void AddRemoteDevice(string deviceId, string roomName, AccessLevel initialAccessLevel = AccessLevel.Basic)
        {
            var room = _controller.GetRoom(roomName);
            if (room == null)
            {
                Console.WriteLine($"Room '{roomName}' not found. Remote device not added.");
                return;
            }
            
            var remoteDeviceProxy = new RemoteDeviceProxy(deviceId, _systemAccessToken);
            remoteDeviceProxy.SetAccessLevel(initialAccessLevel);
            
            _remoteDevices.Add(remoteDeviceProxy);
            
            _controller.AddDevice(remoteDeviceProxy, roomName);
            
            Console.WriteLine($"Remote device '{deviceId}' registered and will be connected when needed");
        }
        
        public void AddDeviceWithProxy(string roomName, Device device, AccessLevel accessLevel = AccessLevel.Standard)
        {
            var room = _controller.GetRoom(roomName);
            if (room == null)
            {
                Console.WriteLine($"Room '{roomName}' not found. Device not added.");
                return;
            }
            
            var remoteDeviceProxy = new RemoteDeviceProxy(device, _systemAccessToken);
            remoteDeviceProxy.SetAccessLevel(accessLevel);
            
            _remoteDevices.Add(remoteDeviceProxy);
            _controller.AddDevice(remoteDeviceProxy, roomName);
            
            Console.WriteLine($"Device '{device.Name}' added to room '{roomName}' with proxy access control");
        }

        public void AddDevice(string roomName, Device device)
        {
            _controller.AddDevice(device, roomName);
            Console.WriteLine($"Device '{device.Name}' added to room '{roomName}'");
        }
        
        public void SetRemoteDeviceAccessLevel(string deviceId, AccessLevel accessLevel)
        {
            var remoteDevice = _remoteDevices.Find(d => d.Name.Contains(deviceId));
            if (remoteDevice != null)
            {
                remoteDevice.SetAccessLevel(accessLevel);
                Console.WriteLine($"Access level for device '{deviceId}' set to {accessLevel}");
            }
            else
            {
                Console.WriteLine($"Remote device '{deviceId}' not found");
            }
        }
        
        public void TurnOnAllLights()
        {
            foreach (var room in _controller.GetAllRooms())
            {
                foreach (var device in room.GetDevices())
                {
                    if (device is Lamp)
                    {
                        device.TurnOn();
                    }
                }
            }
            Console.WriteLine("All lights turned on");
        }
        
        public void TurnOffAllLights()
        {
            foreach (var room in _controller.GetAllRooms())
            {
                foreach (var device in room.GetDevices())
                {
                    if (device is Lamp)
                    {
                        device.TurnOff();
                    }
                }
            }
            Console.WriteLine("All lights turned off");
        }
        
        public void SetAllLightsBrightness(int brightness)
        {
            foreach (var room in _controller.GetAllRooms())
            {
                foreach (var device in room.GetDevices())
                {
                    if (device is Lamp lamp)
                    {
                        lamp.Brightness = brightness;
                    }
                }
            }
            Console.WriteLine($"All lights brightness set to {brightness}%");
        }
        
        public void SetTemperature(string roomName, double temperature)
        {
            var room = _controller.GetRoom(roomName);
            if (room != null)
            {
                foreach (var device in room.GetDevices())
                {
                    if (device is Thermostat thermostat)
                    {
                        thermostat.TargetTemperature = temperature;
                        Console.WriteLine($"Temperature in '{roomName}' set to {temperature}°C");
                        return;
                    }
                }
                Console.WriteLine($"No thermostat found in room '{roomName}'");
            }
            else
            {
                Console.WriteLine($"Room '{roomName}' not found");
            }
        }
        
        public void SaveCurrentState(string stateName)
        {
            var smartHomeMemento = new SmartHomeMemento();
            
            foreach (var room in _controller.GetAllRooms())
            {
                var roomMemento = room.CreateMemento();
                smartHomeMemento.AddRoomMemento(room.Name, roomMemento);
            }
            
            _stateHistory.SaveState(smartHomeMemento);
            _namedStates[stateName] = smartHomeMemento;
            
            Console.WriteLine($"Current state saved as '{stateName}'");
        }
        
        public void RestoreState(string stateName)
        {
            if (_namedStates.TryGetValue(stateName, out var smartHomeMemento))
            {
                foreach (var room in _controller.GetAllRooms())
                {
                    if (smartHomeMemento.TryGetRoomMemento(room.Name, out var roomMemento))
                    {
                        room.RestoreMemento(roomMemento);
                    }
                }
                Console.WriteLine($"State '{stateName}' restored");
            }
            else
            {
                Console.WriteLine($"State '{stateName}' not found");
            }
        }
        
        public void ExecuteCommand(string naturalLanguageCommand)
        {
            _controller.InterpretCommand(naturalLanguageCommand);
        }
        
        public string GetStatusReport()
        {
            var statusVisitor = new StatusReportVisitor();
            foreach (var room in _controller.GetAllRooms())
            {
                room.Accept(statusVisitor);
            }
            return statusVisitor.GetReport();
        }
        
        public Dictionary<string, double> GetEnergyConsumptionReport()
        {
            var energyVisitor = new EnergyConsumptionVisitor();
            foreach (var room in _controller.GetAllRooms())
            {
                room.Accept(energyVisitor);
            }
            
            return energyVisitor.GetDeviceConsumption();
        }
        
        public void EnableSecurityMode()
        {
            _securityManager.EnableSecurityMode(_controller);
        }
        
        public void DisableSecurityMode()
        {
            _controller.DisableSecurityMode();
            _securityManager.DisableSecurityMode();
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
            
            public void EnableSecurityMode(SmartHomeController controller)
            {
                if (IsEnabled)
                {
                    Console.WriteLine("Security mode is already enabled");
                    return;
                }
                
                controller.EnableSecurityMode();
                
                IsEnabled = true;
                Console.WriteLine("Security mode enabled");
            }
            
            public void DisableSecurityMode()
            {
                if (!IsEnabled)
                {
                    Console.WriteLine("Security mode is already disabled");
                    return;
                }
                
                IsEnabled = false;
                Console.WriteLine("Security mode disabled");
            }
        }
    }
    
    public class SmartHomeMemento
    {
        private readonly Dictionary<string, Room.RoomMemento> _roomMementos;
        
        public SmartHomeMemento()
        {
            _roomMementos = new Dictionary<string, Room.RoomMemento>();
        }
        
        public void AddRoomMemento(string roomName, Room.RoomMemento memento)
        {
            _roomMementos[roomName] = memento;
        }
        
        public bool TryGetRoomMemento(string roomName, out Room.RoomMemento memento)
        {
            return _roomMementos.TryGetValue(roomName, out memento);
        }
        
        public IReadOnlyDictionary<string, Room.RoomMemento> GetAllRoomMementos()
        {
            return _roomMementos;
        }
    }
} 