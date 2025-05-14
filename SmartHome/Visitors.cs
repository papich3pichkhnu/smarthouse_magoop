using System;
using System.Collections.Generic;

namespace SmartHome
{
    public interface ISmartHomeVisitor
    {
        void Visit(Room room);
        void Visit(Device device);
        void Visit(Lamp lamp);
        void Visit(MotionSensor sensor);
        void Visit(Thermostat thermostat);
    }

    public class StatusReportVisitor : ISmartHomeVisitor
    {
        private List<string> _report = new List<string>();

        public void Visit(Room room)
        {
            _report.Add($"\nRoom: {room.Name}");
            _report.Add($"Total Devices: {room.GetDevices().Count}");
        }

        public void Visit(Device device)
        {
            _report.Add($"Device: {device.Name}");
            _report.Add($"Status: {device.CurrentState.GetType().Name}");
            _report.Add($"Connected: {device.IsConnected}");
        }

        public void Visit(Lamp lamp)
        {
            Visit((Device)lamp);
            _report.Add($"Brightness: {lamp.Brightness}%");
            _report.Add($"Power: {lamp.Power}W");
            _report.Add($"Max Power: {lamp.MaxPower}W");
        }

        public void Visit(MotionSensor sensor)
        {
            Visit((Device)sensor);
            _report.Add($"Motion Detected: {sensor.Motion}");
        }

        public void Visit(Thermostat thermostat)
        {
            Visit((Device)thermostat);
            _report.Add($"Current Temperature: {thermostat.CurrentTemperature}°C");
            _report.Add($"Target Temperature: {thermostat.TargetTemperature}°C");
        }

        public string GetReport()
        {
            return string.Join("\n", _report);
        }
    }

    public class EnergyConsumptionVisitor : ISmartHomeVisitor
    {
        private double _totalConsumption = 0;
        private Dictionary<string, double> _deviceConsumption = new Dictionary<string, double>();

        public void Visit(Room room)
        {
        }

        public void Visit(Device device)
        {
        }

        public void Visit(Lamp lamp)
        {
            if (lamp.CurrentState is OnState)
            {
                double consumption = lamp.Power * (lamp.Brightness / 100.0);
                _totalConsumption += consumption;
                _deviceConsumption[lamp.Name] = consumption;
            }
        }

        public void Visit(MotionSensor sensor)
        {
            const double SENSOR_POWER = 0.5; 
            if (sensor.CurrentState is OnState)
            {
                _totalConsumption += SENSOR_POWER;
                _deviceConsumption[sensor.Name] = SENSOR_POWER;
            }
        }

        public void Visit(Thermostat thermostat)
        {
            
        }

        public double GetTotalConsumption()
        {
            return _totalConsumption;
        }

        public Dictionary<string, double> GetDeviceConsumption()
        {
            return _deviceConsumption;
        }
    }

    public class ConfigurationValidatorVisitor : ISmartHomeVisitor
    {
        private List<string> _validationErrors = new List<string>();

        public void Visit(Room room)
        {
            if (string.IsNullOrEmpty(room.Name))
            {
                _validationErrors.Add($"Room has no name");
            }
        }

        public void Visit(Device device)
        {
            if (string.IsNullOrEmpty(device.Name))
            {
                _validationErrors.Add($"Device has no name");
            }
        }

        public void Visit(Lamp lamp)
        {
            Visit((Device)lamp);
            
            if (lamp.Power > lamp.MaxPower)
            {
                _validationErrors.Add($"Lamp {lamp.Name}: Power ({lamp.Power}W) exceeds max power ({lamp.MaxPower}W)");
            }
            
            if (lamp.Brightness < 0 || lamp.Brightness > 100)
            {
                _validationErrors.Add($"Lamp {lamp.Name}: Invalid brightness value ({lamp.Brightness}%)");
            }
        }

        public void Visit(MotionSensor sensor)
        {
            Visit((Device)sensor);
        }

        public void Visit(Thermostat thermostat)
        {
            Visit((Device)thermostat);
            
            if (thermostat.TargetTemperature < 10 || thermostat.TargetTemperature > 30)
            {
                _validationErrors.Add($"Thermostat {thermostat.Name}: Target temperature ({thermostat.TargetTemperature}°C) is outside safe range (10-30°C)");
            }
        }

        public bool IsValid()
        {
            return _validationErrors.Count == 0;
        }

        public List<string> GetValidationErrors()
        {
            return _validationErrors;
        }
    }
} 