using System;
namespace SmartHome{
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
}