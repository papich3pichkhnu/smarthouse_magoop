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

RGBLamp rGBLamp = new RGBLamp("RGBLamp", 100, 100, null, 6000, 100, 100, 100);
Thermostat thermostat = new Thermostat("Thermostat", 20, 20);
MotionSensor motionSensor = new MotionSensor("MotionSensor");

Room room = new Room("Living room");
room.AddDevice(rGBLamp);
room.AddDevice(thermostat);
room.AddDevice(motionSensor);


Caretaker<Room.RoomMemento> caretaker = new Caretaker<Room.RoomMemento>();


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
}
            
            
        }

    }
}