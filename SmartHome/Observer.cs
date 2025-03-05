using System;
namespace SmartHome
{

    public interface IObserver
    {
        void Update(Object ob);
        
    }
    public interface IObservable
    {
        void AddObserver(IObserver o);
        void RemoveObserver(IObserver o);
        void NotifyObservers();        
    }
    public class SecuritySystem : Device, IObserver
    {
        public SecuritySystem(string name)
        {
            this.Name=name;
            System.Console.WriteLine($"Creating new Security System {this.Name}");
        }
        public SecuritySystem():this("Default"){}
        public override Device Clone()
        {
            return new SecuritySystem(this.Name);
        }

        public override void TurnOff()
        {
            Console.WriteLine($"Turning off Security System {this.Name}. ");
        }

        public override void TurnOn()
        {
            Console.WriteLine($"Turning on Security System {this.Name}. ");
        }

        public void Update(object ob)
        {
            bool val=(bool)(ob);
            if (val)
            {
                Console.WriteLine($"Security System detected motion");
            }
            else
            {
                Console.WriteLine($"No motion");
            }
        }
    }
}