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
        /*public override void TurnOn()
        {
            Console.WriteLine($"Turning on {Name}. ");
        }

        public override void TurnOff()
        {
            Console.WriteLine($"Turning off {Name}. ");
        }*/
        public override Device Clone()
        {
            return new Thermostat(this.Name,this.Temperature);
        }
        protected override void ExecuteMainFunction()
        {
            System.Console.WriteLine($"Setting temperature to {Temperature}");
        }
    }
}