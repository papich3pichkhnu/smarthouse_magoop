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

        public MotionSensor(string name):base(name)
        {
            
            this.observers=new List<IObserver>();
            this.Motion=false;
            System.Console.WriteLine($"Creating motion sensor {this.Name}");
        }

        public override Device Clone()
        {
            return new MotionSensor(this.Name);
        }

        // Override Accept method for Visitor pattern
        public override void Accept(ISmartHomeVisitor visitor)
        {
            visitor.Visit(this);
        }

        /*public override void TurnOff()
        {            
            Console.WriteLine($"Turning off Motion sensor {this.Name}. ");
        }

        public override void TurnOn()
        {
            Console.WriteLine($"Turning on Motion sensor {this.Name}. ");
        }*/
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
            mediator.Notify(this,"MotionDetected",null);       
        }
        protected override void ExecuteMainFunction()
        {
            System.Console.WriteLine("Motion sensor is running");

        }

        protected override void HandleSpecificCommand(Command command)
        {
            throw new NotImplementedException();
        }
    }
}