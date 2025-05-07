using System;
namespace SmartHome
{
    public class ConcreteVisitor : IVisitor
    {
        public void Visit(Room room)
        {
            Console.WriteLine($"Visiting room: {room.Name}");
            // Additional operations on Room can be added here
        }

        public void Visit(Device device)
        {
            Console.WriteLine($"Visiting device: {device.Name}");
            // Additional operations on Device can be added here
        }

        // Optionally, add more visit methods for specific device types if needed
        // public void Visit(Lamp lamp) { ... }
        // public void Visit(MotionSensor sensor) { ... }
        // public void Visit(Thermostat thermostat) { ... }
    }
}
