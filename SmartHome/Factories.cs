using System;
namespace SmartHome{
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
            return new LEDLamp("Led lamp",100,100,null, 6000);
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
            return new RGBLamp("Led lamp",100,100,null,8000,255,255,0);
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
            return new Thermostat("Thermostat",20);
        }
    }
}