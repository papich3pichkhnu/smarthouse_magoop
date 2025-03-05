using System;
namespace SmartHome
{

    public interface ILampOperationStrategy
    {
        void Execute(Lamp lamp);
    }
    public class EnergySaverMode : ILampOperationStrategy
    {
        public void Execute(Lamp lamp)
        {
            Console.WriteLine("EnergySaver Mode. Lowering brightness and power of devices.");
            lamp.Brightness=30;
            lamp.Power= (int)(lamp.MaxPower*0.3);
        }
    }

    public class NormalMode : ILampOperationStrategy
    {
        public void Execute(Lamp lamp)
        {
            Console.WriteLine("Normal mode.");
            lamp.Brightness = 70;
            lamp.Power = (int)(lamp.MaxPower * 0.7);
        }
    }

}