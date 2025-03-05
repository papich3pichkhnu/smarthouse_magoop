using System;
namespace SmartHome
{
    public abstract class Lamp : Device,IObserver
    {
        public int MaxPower { get; set; }
        public int _power;
        public int Power
        {
            get => _power;
            set
            {
                if (value > MaxPower)
                {
                    value = MaxPower;
                }
                _power = value;

            }
        }
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
        public ILampOperationStrategy OperationStrategy {get;set;}

        public Lamp(string name, int power, int maxPower, ILampOperationStrategy lampOperationStrategy)
        {
            this.Name = name;
            this.MaxPower=maxPower;
            this.Power = power;
            
            Console.WriteLine($"Created {Name} lamp {Power} Watt");
            this.Brightness = 50;
            this.OperationStrategy=lampOperationStrategy;

        }
        public override void TurnOff()
        {
            Console.WriteLine($"Turning off {Power} Watt {Name}. Brightness set at {Brightness}");
        }

        public override void TurnOn()
        {
            Console.WriteLine($"Turning on {Power} Watt {Name}. Brightness set at {Brightness}");
        }
        
        public void ApplyStrategy()
        {
            OperationStrategy?.Execute(this);
        }

        public void Update(object ob)
        {
            bool val=(bool)(ob);
            if (val)
            {
                this.Power = this.MaxPower;
                Console.WriteLine($"Motion detected. Turning on lights");
            }               

        }
    }

    public class LEDLamp : Lamp
    {
        public int ColorTemperature { get; set; }
        public LEDLamp(string name, int power, int maxPower, ILampOperationStrategy? lampOperationStrategy ,int colorTemperature) :
        base(name, power,maxPower, lampOperationStrategy)
        {
            this.ColorTemperature = colorTemperature;
            Console.WriteLine($"Created {Name} LED lamp {Power} Watt");
            this.Brightness = 50;

        }
        public override Device Clone()
        {
            return new LEDLamp(this.Name, this.Power, this.MaxPower, this.OperationStrategy,this.ColorTemperature);
        }
    }

    public class RGBLamp : LEDLamp
    {
        public int Red { get; set; }
        public int Green { get; set; }
        public int Blue { get; set; }
        public RGBLamp(string name,  int power, int maxPower, ILampOperationStrategy lampOperationStrategy, int colorTemperature,int red, int green, int blue) :
        base(name, power, maxPower, lampOperationStrategy, colorTemperature)
        {
            this.Red = red;
            this.Green = green;
            this.Blue = blue;
            Console.WriteLine($"Created {Name} RGB lamp {Power} Watt");
            this.Brightness = 50;
        }
        public RGBLamp() : this("Default rgb lamp ",  50, 100, null, 6000,255, 255, 0) { }
        public override Device Clone()
        {
            return new RGBLamp(this.Name,  this.Power, this.MaxPower, this.OperationStrategy, this.ColorTemperature,this.Red, this.Green, this.Blue);
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