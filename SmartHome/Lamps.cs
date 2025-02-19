using System;
namespace SmartHome{
    public abstract class Lamp : Device
    {
        public int Power { get; set; }
        private int _brightness;
        public int Brightness
        {
            get => _brightness;
            set
            {
                if (value < 0 || value > 100)
                    _brightness = 50;
                else _brightness = value;
                //Console.WriteLine($"Brightness set to {_brightness}.");
            }
        }

        public Lamp(string name, int power)
        {
            this.Name = name;
            this.Power = power;
            Console.WriteLine($"Created {Name} lamp {Power} Watt");
            this.Brightness = 50;

        }
        public override void TurnOff()
        {
            Console.WriteLine($"Turning off {Power} Watt {Name}. Brightness set at {Brightness}");
        }

        public override void TurnOn()
        {
            Console.WriteLine($"Turning on {Power} Watt {Name}. Brightness set at {Brightness}");
        }
    }

    public class LEDLamp : Lamp
    {
        public int ColorTemperature { get; set; }
        public LEDLamp(string name, int power, int colorTemperature) :
        base(name, power)
        {
            this.ColorTemperature = colorTemperature;
            Console.WriteLine($"Created {Name} LED lamp {Power} Watt");
            this.Brightness = 50;

        }
        public override Device Clone()
        {
            return new LEDLamp(this.Name, this.Power, this.ColorTemperature);
        }
    }

    public class RGBLamp : LEDLamp
    {
        public int Red { get; set; }
        public int Green { get; set; }
        public int Blue { get; set; }
        public RGBLamp(string name, int colorTemperature, int power, int red, int green, int blue) :
        base(name, power, colorTemperature)
        {
            this.Red = red;
            this.Green = green;
            this.Blue = blue;
            Console.WriteLine($"Created {Name} RGB lamp {Power} Watt");
            this.Brightness = 50;
        }
        public RGBLamp():this("Default rgb lamp ", 6000, 50, 255, 255, 0){ } 
        public override Device Clone()
        {
            return new RGBLamp(this.Name,this.ColorTemperature,this.Power,this.Red,this.Green,this.Blue);
        }
        public override void TurnOff()
        {
            Console.WriteLine($"Turning off {Power} Watt {Name}. Brightness set at {Brightness}. Color - ({this.Red},{this.Green},{this.Blue})");
        }
        public override void TurnOn()
        {
            Console.WriteLine($"Turning on {Power} Watt {Name}. Brightness set at {Brightness}. Color - ({this.Red},{this.Green},{this.Blue})");
        
        }
    }
}