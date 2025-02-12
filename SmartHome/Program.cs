using System;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

namespace SmartHome
{
    public abstract class Device
    {
        public abstract void TurnOn();

        public abstract void TurnOff();
        public string Name { get; set; } = "Device";
    }

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
    }

    interface LampSpecification
    {
        void Display();
    }
    class KitchenLampSpecification : LampSpecification
    {
        public void Display()
        {
            Console.WriteLine("Kitchen lamp specification: Led 100W, 50% brightness");
        }
    }

    class LivingRoomLampSpecification : LampSpecification
    {
        public void Display()
        {
            Console.WriteLine("Living room lamp specification: RGB 100W, 100% brightness");
        }
    }
    public class Thermostat : Device
    {
        public int Temperature { get; set; }
        public Thermostat(string name)
        {
            this.Name = name;
            this.Temperature = 20;
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
    }

    abstract class DeviceFactory
    {
        public string Name { get; set; }
        public DeviceFactory(string name)
        {
            this.Name = name;
        }
    }

    abstract class LampFactory : DeviceFactory
    {
        public LampFactory(string name) : base(name)
        {
            //Console.WriteLine($"{name} lamp factory created.");
        }
        public abstract Lamp CreateLamp();

        public abstract LampSpecification CreateLampSpecification();
    }
    class KitchenLampFactory : LampFactory{
        public KitchenLampFactory(string name) : base(name){
            Console.WriteLine($"{name} kitchen lamp factory created.");
        }
        public override Lamp CreateLamp(){
            return new LEDLamp("Led lamp",100,6000);
        }
        public override LampSpecification CreateLampSpecification(){
            return new KitchenLampSpecification();
        }
    }
    class LivingRoomLampFactory : LampFactory{
        public LivingRoomLampFactory(string name) : base(name){
            Console.WriteLine($"{name} living room lamp factory created.");
        }
        public override Lamp CreateLamp(){
            return new RGBLamp("Led lamp",100,8000,255,255,0);
        }
        public override LampSpecification CreateLampSpecification(){
            return new LivingRoomLampSpecification();
        }
    }
    class ThermostatFactory : DeviceFactory
    {
        public ThermostatFactory(string name) : base(name)
        {
            Console.WriteLine($"{name} thermostat factory.");
        }
        public Thermostat CreateThermostat()
        {
            return new Thermostat("Thermostat");
        }
    }

    public class MainProgram
    {
        public static void Main(string[] args)
        {
            LampFactory fact = new KitchenLampFactory("RZTK");
            Device kitchenLamp = fact.CreateLamp();
            LampSpecification kitchenLampSpec=fact.CreateLampSpecification();
            kitchenLamp.TurnOn();
            ((Lamp)kitchenLamp).Brightness=100;
            kitchenLamp.TurnOff();

            fact = new LivingRoomLampFactory("China");
            Device livingRoomLamp = fact.CreateLamp();
            LampSpecification livingRoomLampSpec = fact.CreateLampSpecification();
            livingRoomLamp.TurnOn();
            ((Lamp)livingRoomLamp).Brightness = 100;
            livingRoomLamp.TurnOff();

            ThermostatFactory fact1 = new ThermostatFactory("China1");
            Device thermostat = fact1.CreateThermostat();
            thermostat.TurnOn();
            ((Thermostat)thermostat).Temperature=25;
            thermostat.TurnOff();
        }

    }
}