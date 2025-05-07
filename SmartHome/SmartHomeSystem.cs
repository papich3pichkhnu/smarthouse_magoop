using System;
using System.Collections.Generic;

namespace SmartHome
{

    public class SmartHomeSystem
    {
        private readonly SmartHomeController _controller;
        private readonly Dictionary<string, Room> _rooms;
        private readonly SmartHomeHistory<Room.RoomMemento> _stateHistory;
        private readonly Dictionary<string, Room.RoomMemento> _namedStates;
        private readonly SecurityManager _securityManager;
        
        public SmartHomeSystem(string mainRoomName = "Living Room")
        {
            _controller = new SmartHomeController(mainRoomName);
            _rooms = new Dictionary<string, Room>
            {
                { mainRoomName, new Room(mainRoomName) }
            };
            _stateHistory = new SmartHomeHistory<Room.RoomMemento>();
            _namedStates = new Dictionary<string, Room.RoomMemento>();
            _securityManager = new SecurityManager();
        }
        
        public void AddRoom(string roomName)
        {
            if (!_rooms.ContainsKey(roomName))
            {
                _rooms[roomName] = new Room(roomName);
                Console.WriteLine($"Room '{roomName}' added to the smart home system");
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
        

        public void AddDevice(string roomName, Device device)
        {
            if (_rooms.ContainsKey(roomName))
            {
                _rooms[roomName].AddDevice(device);
                device.SetMediator(_controller);
                
                if (device is MotionSensor motionSensor)
                {
                    _securityManager.RegisterMotionSensor(motionSensor);
                }
                
                Console.WriteLine($"Device '{device.Name}' added to room '{roomName}'");
            }
            else
            {
                Console.WriteLine($"Room '{roomName}' not found. Device not added.");
            }
        }
        
        public void TurnOnAllLights()
        {
            foreach (var room in _rooms.Values)
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
            foreach (var room in _rooms.Values)
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
            foreach (var room in _rooms.Values)
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
            if (_rooms.ContainsKey(roomName))
            {
                foreach (var device in _rooms[roomName].GetDevices())
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
            foreach (var room in _rooms.Values)
            {
                var memento = room.CreateMemento();
                _stateHistory.SaveState(memento);
                _namedStates[stateName] = memento;
            }
            Console.WriteLine($"Current state saved as '{stateName}'");
        }
        
        public void RestoreState(string stateName)
        {
            if (_namedStates.TryGetValue(stateName, out var memento))
            {
                foreach (var room in _rooms.Values)
                {
                    room.RestoreMemento(memento);
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
            foreach (var room in _rooms.Values)
            {
                room.Accept(statusVisitor);
            }
            return statusVisitor.GetReport();
        }
        
        public Dictionary<string, double> GetEnergyConsumptionReport()
        {
            var energyVisitor = new EnergyConsumptionVisitor();
            foreach (var room in _rooms.Values)
            {
                room.Accept(energyVisitor);
            }
            
            return energyVisitor.GetDeviceConsumption();
        }
        
        public void EnableSecurityMode()
        {
            _securityManager.EnableSecurityMode(_controller, _rooms);
        }
        
        public void DisableSecurityMode()
        {
            _securityManager.DisableSecurityMode(_rooms);
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
                        if (device is Lamp)
                        {
                            device.TurnOff();
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
        }
    }
} 