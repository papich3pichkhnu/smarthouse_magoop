﻿using System;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

namespace SmartHome
{ 
    
    public class MainProgram
    {
        public static void Main(string[] args)
        {
            // Factories
            /*LampFactory fact = new KitchenLampFactory("RZTK");
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
            thermostat.TurnOff();*/

            //Builders, Prototypes

            /*LampCreator lampCreator = new LampCreator();
            RGBLampBuilder builder = new BrightRedRGBLampBuilder();
            RGBLamp? brightRed = lampCreator.Create(builder);
            brightRed?.TurnOn();
            brightRed?.TurnOff();

            RGBLamp? anotherBrightRed=(RGBLamp)brightRed?.Clone();
            anotherBrightRed.Name="New rgb red lamp";
            anotherBrightRed?.TurnOn();
            anotherBrightRed?.TurnOff();

            builder= new SemiBrightGreenRGBLampBuilder();
            RGBLamp? semiBrightGreen = lampCreator.Create(builder);
            semiBrightGreen.TurnOn();
            semiBrightGreen.TurnOff();*/

            //Strategy

            /*LEDLamp ledLamp = new LEDLamp("Lamp",100,100,null,6000);
            
            ledLamp.OperationStrategy=new NormalMode();
            ledLamp.ApplyStrategy();
            ledLamp.TurnOn();*/

            //Observer

            /*MotionSensor motionSensor = new MotionSensor();
            SecuritySystem securitySystem = new SecuritySystem();
            motionSensor.AddObserver(ledLamp);
            motionSensor.AddObserver(securitySystem);
            motionSensor.DetectMotion();*/

            //MacroCommand

            LEDLamp ledLamp = new LEDLamp("Lamp",100,100,null,6000);
            
            ledLamp.OperationStrategy=new NormalMode();
            ledLamp.ApplyStrategy();

            List<ICommand> commands = new List<ICommand>
            {
                new TurnOffCommand(ledLamp),
                new SetHalfBrightnessCommand(ledLamp),
                new TurnOnCommand(ledLamp)
            };
            ICommand mc = new MacroCommand(commands);
            RemoteController rc=new RemoteController();
            rc.AddCommand(mc);
            rc.ExecuteCommands();            

            //TemplateMethod
            System.Console.WriteLine("--------------------------------");

            RGBLamp rGBLamp = new RGBLamp("RGB Lamp", 100, 100, null, 6000,100,100,100);
            Thermostat thermostat = new Thermostat("Thermostat",20);
            MotionSensor motionSensor = new MotionSensor();
            System.Console.WriteLine("--------------------------------");
            System.Console.WriteLine("Template command");
            System.Console.WriteLine("--------------------------------");
            List<ICommand> commands1=new List<ICommand>
            {
                new FunctionCommand(rGBLamp.OperateDevice),
                new FunctionCommand(thermostat.OperateDevice),
                new FunctionCommand(motionSensor.OperateDevice)
            };
            ICommand mc1 = new MacroCommand(commands1);
            rc.ClearCommands();
            rc.AddCommand(mc1);
            rc.ExecuteCommands();            

            
        }

    }
}