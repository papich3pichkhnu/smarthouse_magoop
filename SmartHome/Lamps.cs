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
                _brightness=Math.Clamp(value,0,100);
                if (CurrentState is OnState && !(CurrentState is ActiveState) )
                {
                    CurrentState=new ActiveState("dimming");
                }
                System.Console.WriteLine($"Light {Name} brightness set to {value}%");
                
                //Console.WriteLine($"Brightness set to {_brightness}.");
            }
        }
        public ILampOperationStrategy? OperationStrategy {get;set;}

        public Lamp(string name, int power, int maxPower, ILampOperationStrategy lampOperationStrategy):base(name)
        {
            
            this.MaxPower=maxPower;
            this.Power = power;
            
            Console.WriteLine($"Created {Name} lamp {Power} Watt");
            this.Brightness = 50;
            this.OperationStrategy=lampOperationStrategy;

        }
        /*public override void TurnOff()
        {
            Console.WriteLine($"Turning off {Power} Watt {Name} lamp. Brightness set at {Brightness}");
        }

        public override void TurnOn()
        {
            Console.WriteLine($"Turning on {Power} Watt {Name} lamp. Brightness set at {Brightness}");
        }
        */
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
        public override void RequestStatus()
        {
            Console.WriteLine($"Light {Name} brigthness: {Brightness}%");
        }
    }

    public class LEDLamp : Lamp
    {
        private int _colorTemperature;
        public int ColorTemperature { get => _colorTemperature; set
            {
                _colorTemperature=Math.Clamp(value,1000,10000);
                
                System.Console.WriteLine($"Light {Name} color temperature set to {_colorTemperature}%");
                
            } 
        }
        public LEDLamp(string name, int power, int maxPower, ILampOperationStrategy lampOperationStrategy ,int colorTemperature) :
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

        protected override void ExecuteMainFunction()
        {
            System.Console.WriteLine($"Turning on light on lamp {Name} at brighness {Brightness} and color temperature {ColorTemperature}");
        }
        protected override void HandleSpecificCommand(Command command)
        {
            switch(command.CommandType)
            {
                case CommandType.SetBrightness:
                    if(command.Parameters.Length>0 && command.Parameters[0] is int brightness)
                    {
                        this.Brightness=brightness;
                    }
                    break;
                case CommandType.SetColorTemperatureK:
                    if(command.Parameters.Length>0 && command.Parameters[0] is int colorTempK)
                    {
                        this.ColorTemperature=colorTempK;
                    }
                    break;
                default:
                    System.Console.WriteLine($"Command {command.CommandType} not supported by light {Name}");
                    break;
            }
        }
        public override void RequestStatus()
        {
            System.Console.WriteLine($"Light {Name}, brightness: {Brightness}%, ColorTemperatureK: {ColorTemperature}K ");
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
        /*public override void TurnOff()
        {
            Console.WriteLine($"Turning off {Power} Watt {Name}. Brightness set at {Brightness}. Color - ({this.Red},{this.Green},{this.Blue})");
        }
        public override void TurnOn()
        {
            Console.WriteLine($"Turning on {Power} Watt {Name}. Brightness set at {Brightness}. Color - ({this.Red},{this.Green},{this.Blue})");

        }*/

        protected override void ExecuteMainFunction()
        {
            System.Console.WriteLine($"Turning on ({Red},{Green},{Blue}) colored light on lamp {Name} at brighness {Brightness} and color temperature {ColorTemperature}");
        }
        public override void RequestStatus()
        {
            base.RequestStatus();
            System.Console.WriteLine($"Light {Name}, brightness: {Brightness}%, ColorTemperatureK: {ColorTemperature}K, Color - ({Red},{Green},{Blue})");
        }
    }
}