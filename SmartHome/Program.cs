using System;
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

            /*LEDLamp ledLamp = new LEDLamp("Lamp",100,100,null,6000);
            
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
            rc.ExecuteCommands();         */

            //TemplateMethod
            /*System.Console.WriteLine("--------------------------------");

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
            rc.ExecuteCommands();         */

            //Iterator, State, ChainOfResponsibilty
            /* System.Console.WriteLine("--------------------------------");            
             RGBLamp rGBLamp = new RGBLamp("RGBLamp", 100, 100, null, 6000,100,100,100);
             Thermostat thermostat = new Thermostat("Thermostat",20,20);
             MotionSensor motionSensor = new MotionSensor("MotionSensor");

             Room room = new Room("Living room");
             room.AddDevice(rGBLamp);
             room.AddDevice(thermostat);
             room.AddDevice(motionSensor);

             var iterator=room.CreateIterator();
             System.Console.WriteLine($"devices in room {room.Name}");
             while(iterator.HasNext())
             {
                 var device = iterator.Next();
                 device.TurnOn();
             }

             SmartHomeController smartHomeController=new SmartHomeController("Living room");

             rGBLamp.SetMediator(smartHomeController);
             thermostat.SetMediator(smartHomeController);
             motionSensor.SetMediator(smartHomeController);

             smartHomeController.SendCommand(CommandType.Status,"RGBLamp");

             var thermostat1=smartHomeController.FindDevice("Thermostat") as Thermostat;
             thermostat1.CurrentTemperature = 22.0;

             smartHomeController.InterpretCommand("turn on RGBLamp and turn on thermostat and set brightness to 75% for rgblamp and set color to magenta for rgblamp");
             System.Console.WriteLine($"RGBLamp Color({rGBLamp.Red},{rGBLamp.Green},{rGBLamp.Blue})");
             System.Console.WriteLine($"RGBLamp brightness {rGBLamp.Brightness}%");

             System.Console.WriteLine("------------");
             motionSensor.DetectMotion();
         */

            // Memento 

            /*RGBLamp rGBLamp = new RGBLamp("RGBLamp", 100, 100, null, 6000, 100, 100, 100);
            Thermostat thermostat = new Thermostat("Thermostat", 20, 20);
            MotionSensor motionSensor = new MotionSensor("MotionSensor");

            Room room = new Room("Living room");
            room.AddDevice(rGBLamp);
            room.AddDevice(thermostat);
            room.AddDevice(motionSensor);


            SmartHomeHistory<Room.RoomMemento> caretaker = new SmartHomeHistory<Room.RoomMemento>();


            caretaker.SaveState(room.CreateMemento());
            System.Console.WriteLine("Initial state saved(all off).");

            rGBLamp.TurnOn();
            thermostat.TurnOn();
            motionSensor.TurnOn();

            System.Console.WriteLine("Devices turned on.");

            caretaker.SaveState(room.CreateMemento());
            System.Console.WriteLine("New state saved(all on).");

            rGBLamp.TurnOff();
            thermostat.TurnOff();
            motionSensor.TurnOff();

            System.Console.WriteLine("Devices turned off.");
            caretaker.SaveState(room.CreateMemento());
            System.Console.WriteLine("New state saved(all off).");

            if (caretaker.CanUndo())
            {
                var previousState = caretaker.Undo();
                room.RestoreMemento(previousState);
                System.Console.WriteLine("State restored to previous (undo).");
            }


            if (caretaker.CanUndo())
            {
                var initialState = caretaker.Undo();
                room.RestoreMemento(initialState);
                System.Console.WriteLine("State restored to initial (undo).");
            }


            if (caretaker.CanRedo())
            {
                var redoState = caretaker.Redo();
                room.RestoreMemento(redoState);
                System.Console.WriteLine("State redone.");

                
            }*/

            /*System.Console.WriteLine("----------------visitor----------------");

            Room livingRoom = new Room("Living Room");
            
            LEDLamp mainLight = new LEDLamp("Main Light", 100, 100, new NormalMode(), 6000);
            livingRoom.AddDevice(mainLight);
            
            mainLight.TurnOn();
            
            Console.WriteLine("\n=== Status Report ===");
            var statusVisitor = new StatusReportVisitor();
            livingRoom.Accept(statusVisitor);
            string report = statusVisitor.GetReport();
            Console.WriteLine(report);
            
            Console.WriteLine("\n=== Energy Consumption Report ===");
            var energyVisitor = new EnergyConsumptionVisitor();
            livingRoom.Accept(energyVisitor);
            double totalConsumption = energyVisitor.GetTotalConsumption();
            Console.WriteLine($"Total Energy Consumption: {totalConsumption:F2}W");
            foreach (var device in energyVisitor.GetDeviceConsumption())
            {
                Console.WriteLine($"{device.Key}: {device.Value:F2}W");
            }
            
            Console.WriteLine("\n=== Configuration Validation ===");
            var validatorVisitor = new ConfigurationValidatorVisitor();
            livingRoom.Accept(validatorVisitor);
            
            if (validatorVisitor.IsValid())
            {
                Console.WriteLine("All device configurations are valid!");
            }
            else
            {
                Console.WriteLine("Configuration issues found:");
                foreach (var error in validatorVisitor.GetValidationErrors())
                {
                    Console.WriteLine($"- {error}");
                }
            }
            
            Console.WriteLine("\n=== Testing Invalid Configuration ===");
            LEDLamp invalidLamp = new LEDLamp("Invalid Lamp", 150, 100, new NormalMode(), 6000);
            invalidLamp.Brightness = 120; 
            Room testRoom = new Room("");
            testRoom.AddDevice(invalidLamp);*/
            
            // Facade 
            Console.WriteLine("\n====== Facade  ======\n");
            
            SmartHomeSystem smartHome = new SmartHomeSystem("Living Room");
            
            smartHome.AddRoom("Kitchen");
            smartHome.AddRoom("Bedroom");
            
            LEDLamp livingRoomLight = new LEDLamp("Living Room Light", 60, 100, new NormalMode(), 5000);
            RGBLamp bedroomLight = new RGBLamp("Bedroom Light", 40, 80, new NormalMode(), 4000, 255, 255, 255);
            LEDLamp kitchenLight = new LEDLamp("Kitchen Light", 70, 100, new NormalMode(), 6000);
            Thermostat livingRoomThermostat = new Thermostat("Living Room Thermostat", 22, 22);
            MotionSensor entranceMotionSensor = new MotionSensor("Entrance Sensor");
            
            smartHome.AddDevice("Living Room", livingRoomLight);
            smartHome.AddDevice("Living Room", livingRoomThermostat);
            smartHome.AddDevice("Living Room", entranceMotionSensor);
            smartHome.AddDevice("Bedroom", bedroomLight);
            smartHome.AddDevice("Kitchen", kitchenLight);


            smartHome.TurnOnAllLights();

            smartHome.SetAllLightsBrightness(75);

            smartHome.SetTemperature("Living Room", 24.5);

            Console.WriteLine("\n--- Status Report ---");
            string statusReport = smartHome.GetStatusReport();
            Console.WriteLine(statusReport);
            
            smartHome.SaveCurrentState("EveningMode");
            

            smartHome.ExecuteCommand("jarvis turn on security mode");
            
            entranceMotionSensor.DetectMotion();
            
            smartHome.ExecuteCommand("jarvis turn off security mode");
            
            smartHome.ExecuteCommand("turn on kitchen light and set brightness to 50%");
            

            var energyReport = smartHome.GetEnergyConsumptionReport();
            double totalConsumption = 0;
            foreach (var device in energyReport)
            {
                Console.WriteLine($"{device.Key}: {device.Value:F2}W");
                totalConsumption += device.Value;
            }
            Console.WriteLine($"Total Energy Consumption: {totalConsumption:F2}W");

            Console.WriteLine("\n--- Testing Security Mode with Direct Methods ---");
            smartHome.EnableSecurityMode();
            entranceMotionSensor.DetectMotion();
            smartHome.DisableSecurityMode();
            
            Console.WriteLine("\n--- Restore Saved State ---");
            smartHome.RestoreState("EveningMode");

        }
    }
}