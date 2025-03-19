using System;
using System.Collections.Generic;
namespace SmartHome
{
    public class MotionSensor:Device, IObservable
    {
        public bool Motion {get;set;}
        private List<IObserver> observers = new List<IObserver>();
        public MotionSensor():this("Default")
        {
            
        }

        public MotionSensor(string name)
        {
            this.Name=name;
            this.observers=new List<IObserver>();
            this.Motion=false;
            System.Console.WriteLine($"Creating motion sensor {this.Name}");
        }

        public override Device Clone()
        {
            return new MotionSensor(this.Name);
        }

        public override void TurnOff()
        {            
            Console.WriteLine($"Turning off Motion sensor {this.Name}. ");
        }

        public override void TurnOn()
        {
            Console.WriteLine($"Turning on Motion sensor {this.Name}. ");
        }
        public void AddObserver(IObserver o)
        {
            this.observers.Add(o);
        }
        public void RemoveObserver(IObserver o)
        {
            this.observers.Remove(o);
        }
        public void NotifyObservers()
        {
            foreach (var observer in observers)
            {
                observer.Update(this.Motion);
            }
        }
        public void DetectMotion()
        {            
            this.Motion=true;
            this.NotifyObservers();            
        }
        protected override void ExecuteMainFunction()
        {
            System.Console.WriteLine("Motion sensor is running");

        }

    }
}