using System;
namespace SmartHome
{
    public abstract class CommandProcessor
    {
        protected CommandProcessor _nextProcessor;
        protected SmartHomeController _controller;
        public CommandProcessor(SmartHomeController controller)
        {
            _controller = controller;
        }
        public CommandProcessor SetNext(CommandProcessor nextProcessor)
        {
            _nextProcessor=nextProcessor;
            return nextProcessor;
        }
        public virtual void ProcessCommand(Command command)
        {
            if(_nextProcessor!=null)
            {
                _nextProcessor.ProcessCommand(command);
            }
        }
    }
    public class SmartHomeController
    {
        private CommandProcessor _commandChain;
        public SmartHomeController()
        {

        }
    }
}