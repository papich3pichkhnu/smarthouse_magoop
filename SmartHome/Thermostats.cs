using System;
namespace SmartHome{
    public class Thermostat : Device
    {
        public int Temperature { get; set; }
        public Thermostat():this("Default",20){}
        public Thermostat(string name, int temperature)
        {
            this.Name = name;
            this.Temperature = temperature;
            
            Console.WriteLine($"Created thermostat {Name}");

        }
        public override void TurnOn()
        {
            Console.WriteLine($"Turning on {Name}. Temperature set at {Temperature}.");
        }

        public override void TurnOff()
        {
            Console.WriteLine($"Turning off {Name}. Temperature set at {Temperature}.");
        }
        public override Device Clone()
        {
            return new Thermostat(this.Name,this.Temperature);
        }
    }
}