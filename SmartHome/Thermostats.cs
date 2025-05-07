using System;
namespace SmartHome{
    public class Thermostat : Device
    {
        private double _currentTemperature;
        private double _targetTemperature;
        public double TargetTemperature { get => _targetTemperature; set{
            _targetTemperature=Math.Clamp(value,10,30);
            System.Console.WriteLine($"Thermostat {Name} target temperature set to {_targetTemperature} C");
            if(CurrentState is OnState)
            {
                if(_currentTemperature<_targetTemperature)
                    CurrentState=new ActiveState("heating");
                else if (_currentTemperature>_targetTemperature)
                    CurrentState=new ActiveState("cooling");
                else
                    CurrentState=new OnState();
            }
        }
        }
        public double CurrentTemperature {get => _currentTemperature; set{
            _currentTemperature=value;
            if(CurrentState is OnState)
            {
                if(Math.Abs(_currentTemperature-_targetTemperature)<0.5)
                {
                    if(CurrentState is ActiveState)
                        CurrentState=new OnState();
                }
                else
                {
                    if(_currentTemperature<_targetTemperature)
                        CurrentState=new ActiveState("heating");
                    else if (_currentTemperature > _targetTemperature)
                        CurrentState = new ActiveState("cooling");
                    mediator.Notify(this,"TemperatureChanged",_currentTemperature);
                }
            }
        }}
        public Thermostat():this("Default",20,20){}
        public Thermostat(string name, double currentTemperature, double targetTemperature):base(name)
        {            
            _currentTemperature=currentTemperature;
            _targetTemperature=targetTemperature;
            
            
        }

        // Override Accept method for Visitor pattern
        public override void Accept(ISmartHomeVisitor visitor)
        {
            visitor.Visit(this);
        }

        /*public override void TurnOn()
        {
            Console.WriteLine($"Turning on {Name}. ");
        }

        public override void TurnOff()
        {
            Console.WriteLine($"Turning off {Name}. ");
        }*/
        public override Device Clone()
        {
            return new Thermostat(this.Name,this.CurrentTemperature,this.TargetTemperature);
        }
        protected override void ExecuteMainFunction()
        {
            System.Console.WriteLine($"Setting temperature to {TargetTemperature}");
        }

        protected override void HandleSpecificCommand(Command command)
        {
            throw new NotImplementedException();
        }
    }
}