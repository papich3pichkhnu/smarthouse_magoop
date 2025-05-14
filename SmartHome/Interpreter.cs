using System;
namespace SmartHome
{
    public interface IExpression
    {
        void Interpret(SmartHomeContext context);

    }
    public class SmartHomeContext
    {
        public SmartHomeController Controller { get; set; }
        public Dictionary<string, object> Variables { get; set; }

        public SmartHomeContext(SmartHomeController controller)
        {
            Controller = controller;
            Variables = new Dictionary<string, object>();
        }
        public virtual IDeviceControl FindDevice(string deviceName)
        {
            return Controller.FindIDevice(deviceName);
        }
        public virtual void ExecuteCommand(Command command)
        {
            Controller.ExecuteCommand(command);
        }

    }

    public class DeviceExpression : IExpression
    {
        private string deviceName;

        public DeviceExpression(string deviceName)
        {
            this.deviceName = deviceName;
        }

        public void Interpret(SmartHomeContext context)
        {
            var device = context.FindDevice(deviceName);
            if (device != null)
            {
                context.Variables["currentDevice"] = device;
            }
            else
            {
                throw new InvalidOperationException($"Device '{deviceName}' not found");
            }
        }
    }

    public class TurnOnExpression : IExpression
    {
        public void Interpret(SmartHomeContext context)
        {
            if (context.Variables.ContainsKey("currentDevice"))
            {
                var device = context.Variables["currentDevice"] as IDeviceControl;
                if (device != null)
                {
                    var command = new Command(CommandType.TurnOn, device.Name);
                    context.ExecuteCommand(command);
                }
            }
        }
    }


    public class TurnOffExpression : IExpression
    {
        public void Interpret(SmartHomeContext context)
        {
            if (context.Variables.ContainsKey("currentDevice"))
            {
                var device = context.Variables["currentDevice"] as IDeviceControl;
                if (device != null)
                {
                    var command = new Command(CommandType.TurnOff, device.Name);
                    context.ExecuteCommand(command);
                }
            }
        }
    }

    public class EnableSecurityModeExpression : IExpression
    {
        public void Interpret(SmartHomeContext context)
        {
            var command = new Command(CommandType.EnableSecurityMode, "system");
            context.ExecuteCommand(command);
        }
    }

    public class DisableSecurityModeExpression : IExpression
    {
        public void Interpret(SmartHomeContext context)
        {
            var command = new Command(CommandType.DisableSecurityMode, "system");
            context.ExecuteCommand(command);
        }
    }


    public class SetBrightnessExpression : IExpression
    {
        private int brightness;

        public SetBrightnessExpression(int brightness)
        {
            this.brightness = brightness;
        }

        public void Interpret(SmartHomeContext context)
        {
            if (context.Variables.ContainsKey("currentDevice"))
            {
                var device = context.Variables["currentDevice"] as IDeviceControl;
                if (device != null)
                {
                    var command = new Command(CommandType.SetBrightness, device.Name, brightness);
                    context.ExecuteCommand(command);
                }
            }
        }
    }


    public class SetColorExpression : IExpression
    {
        private int red;
        private int green;
        private int blue;

        public SetColorExpression(int red, int green, int blue)
        {
            this.red = red;
            this.green = green;
            this.blue = blue;
        }

        public void Interpret(SmartHomeContext context)
        {
            if (context.Variables.ContainsKey("currentDevice"))
            {
                var device = context.Variables["currentDevice"] as IDeviceControl;
                if (device != null)
                {
                    var command = new Command(CommandType.SetColor, device.Name, red, green, blue);
                    context.ExecuteCommand(command);
                }
            }
        }
    }

    public class SetTemperatureExpression : IExpression
    {
        private double temperature;

        public SetTemperatureExpression(double temperature)
        {
            this.temperature = temperature;
        }

        public void Interpret(SmartHomeContext context)
        {
            if (context.Variables.ContainsKey("currentDevice"))
            {
                var device = context.Variables["currentDevice"] as IDeviceControl;
                if (device != null)
                {
                    var command = new Command(CommandType.SetTemperature, device.Name, temperature);
                    context.ExecuteCommand(command);
                }
            }
        }
    }

    public class SequenceExpression : IExpression
    {
        private List<IExpression> expressions = new List<IExpression>();

        public void AddExpression(IExpression expression)
        {
            expressions.Add(expression);
        }

        public void Interpret(SmartHomeContext context)
        {
            foreach (var expression in expressions)
            {
                expression.Interpret(context);
            }
        }
    }

    public class CommandParser
    {
        public IExpression Parse(string input)
        {
            input = input.ToLower();

            var tokens = Tokenize(input);

            int position = 0;

            var sequence = new SequenceExpression();

            if (tokens.Count >= 4 && tokens[0] == "jarvis" && tokens[1] == "turn")
            {
                if (tokens[2] == "on" && tokens.Count >= 5 && tokens[3] == "security" && tokens[4] == "mode")
                {
                    return new EnableSecurityModeExpression();
                }
                else if (tokens[2] == "off" && tokens.Count >= 5 && tokens[3] == "security" && tokens[4] == "mode")
                {
                    return new DisableSecurityModeExpression();
                }
            }

            while (position < tokens.Count)
            {
                var expr = ParseSingleExpression(tokens, ref position);
                if (expr != null)
                {
                    sequence.AddExpression(expr);
                }
                else
                {
                    break;
                }

                if (position < tokens.Count && tokens[position] == "and")
                {
                    position++;
                }
                else
                {
                    break;
                }
            }

            return sequence;
        }

        private List<string> Tokenize(string input)
        {
            return input.Split(new[] { ' ', ',', '.', '!', '?' }, StringSplitOptions.RemoveEmptyEntries).ToList();
        }

        private string ExtractDeviceName(List<string> tokens, ref int position, params string[] stopTokens)
        {
            if (position >= tokens.Count)
                return string.Empty;
                
            int startPos = position;
            int endPos = position;
            
            HashSet<string> defaultStopTokens = new HashSet<string> { "and", "for", "set", "to" };
            
            if (stopTokens != null && stopTokens.Length > 0)
            {
                foreach (var token in stopTokens)
                {
                    defaultStopTokens.Add(token);
                }
            }
            
            while (endPos < tokens.Count && !defaultStopTokens.Contains(tokens[endPos]))
            {
                endPos++;
            }
            
            string deviceName = string.Join(" ", tokens.GetRange(startPos, endPos - startPos));
            position = endPos; 
            
            return deviceName;
        }

        private IExpression ParseSingleExpression(List<string> tokens, ref int position)
        {
            if (position >= tokens.Count)
            {
                return null;
            }

            string token = tokens[position];
            position++;

            switch (token)
            {
                case "turn":
                    if (position < tokens.Count)
                    {
                        string action = tokens[position];
                        position++;

                        if (action == "on")
                        {
                            // Check for "turn on security mode" pattern
                            if (position + 1 < tokens.Count && tokens[position] == "security" && tokens[position + 1] == "mode")
                            {
                                position += 2; 
                                return new EnableSecurityModeExpression();
                            }
                            return ParseTurnOnExpression(tokens, ref position);
                        }
                        else if (action == "off")
                        {
                            // Check for "turn off security mode" pattern
                            if (position + 1 < tokens.Count && tokens[position] == "security" && tokens[position + 1] == "mode")
                            {
                                position += 2; 
                                return new DisableSecurityModeExpression();
                            }
                            return ParseTurnOffExpression(tokens, ref position);
                        }
                    }
                    break;

                case "set":
                    if (position < tokens.Count)
                    {
                        string property = tokens[position];
                        position++;

                        if (property == "brightness")
                        {
                            return ParseSetBrightnessExpression(tokens, ref position);
                        }
                        else if (property == "color")
                        {
                            return ParseSetColorExpression(tokens, ref position);
                        }
                        else if (property == "temperature")
                        {
                            return ParseSetTemperatureExpression(tokens, ref position);
                        }
                    }
                    break;

                default:
                    var deviceExpression = new DeviceExpression(token);
                    return deviceExpression;
            }

            return null;
        }
    
    private IExpression ParseTurnOnExpression(List<string> tokens, ref int position)
    {
        if (position < tokens.Count)
        {
            string deviceName = ExtractDeviceName(tokens, ref position);
            
            var deviceExpression = new DeviceExpression(deviceName);
            
            var sequence = new SequenceExpression();
            sequence.AddExpression(deviceExpression);
            sequence.AddExpression(new TurnOnExpression());
            
            return sequence;
        }
        
        return null;
    }
    
    private IExpression ParseTurnOffExpression(List<string> tokens, ref int position)
    {
        if (position < tokens.Count)
        {
            string deviceName = ExtractDeviceName(tokens, ref position);
            
            var deviceExpression = new DeviceExpression(deviceName);
            
            var sequence = new SequenceExpression();
            sequence.AddExpression(deviceExpression);
            sequence.AddExpression(new TurnOffExpression());
            
            return sequence;
        }
        
        return null;
    }
    
    private IExpression ParseSetBrightnessExpression(List<string> tokens, ref int position)
    {
        if (position < tokens.Count && tokens[position] == "to")
        {
            position++;
        }
        
        if (position < tokens.Count)
        {
            string brightnessStr = tokens[position];
            position++;
            
            brightnessStr = brightnessStr.TrimEnd('%');
            
            if (int.TryParse(brightnessStr, out int brightness))
            {
                if (position < tokens.Count && tokens[position] == "for")
                {
                    position++;
                    if (position < tokens.Count)
                    {
                        string deviceName = ExtractDeviceName(tokens, ref position);
                        
                        var deviceExpression = new DeviceExpression(deviceName);
                        
                        var sequence = new SequenceExpression();
                        sequence.AddExpression(deviceExpression);
                        sequence.AddExpression(new SetBrightnessExpression(brightness));
                        
                        return sequence;
                    }
                }
                else
                {
                    return new SetBrightnessExpression(brightness);
                }
            }
        }
        
        return null;
    }
    
    private IExpression ParseSetColorExpression(List<string> tokens, ref int position)
    {
        if (position < tokens.Count && tokens[position] == "to")
        {
            position++;
        }
        
        if (position < tokens.Count)
        {
            string colorName = tokens[position];
            position++;
            
            var (red, green, blue) = ParseColorName(colorName);
            
            if (position < tokens.Count && tokens[position] == "for")
            {
                position++;
                if (position < tokens.Count)
                {
                    string deviceName = ExtractDeviceName(tokens, ref position);
                    
                    var deviceExpression = new DeviceExpression(deviceName);
                    
                    var sequence = new SequenceExpression();
                    sequence.AddExpression(deviceExpression);
                    sequence.AddExpression(new SetColorExpression(red, green, blue));
                    
                    return sequence;
                }
            }
            else
            {
                return new SetColorExpression(red, green, blue);
            }
        }
        
        return null;
    }
    
    private (int, int, int) ParseColorName(string colorName)
    {
        switch (colorName.ToLower())
        {
            case "red": return (255, 0, 0);
            case "green": return (0, 255, 0);
            case "blue": return (0, 0, 255);
            case "yellow": return (255, 255, 0);
            case "cyan": return (0, 255, 255);
            case "magenta": return (255, 0, 255);
            case "white": return (255, 255, 255);
            case "black": return (0, 0, 0);
            default: return (128, 128, 128);
        }
    }
    
    private IExpression ParseSetTemperatureExpression(List<string> tokens, ref int position)
    {
        if (position < tokens.Count && tokens[position] == "to")
        {
            position++;
        }
        
        if (position < tokens.Count)
        {
            string tempStr = tokens[position];
            position++;
            
            if (double.TryParse(tempStr, out double temperature))
            {
                if (position < tokens.Count && tokens[position] == "for")
                {
                    position++;
                    if (position < tokens.Count)
                    {
                        string deviceName = ExtractDeviceName(tokens, ref position);
                        
                        var deviceExpression = new DeviceExpression(deviceName);
                        
                        var sequence = new SequenceExpression();
                        sequence.AddExpression(deviceExpression);
                        sequence.AddExpression(new SetTemperatureExpression(temperature));
                        
                        return sequence;
                    }
                }
                else
                {
                    return new SetTemperatureExpression(temperature);
                }
            }
        }
        
        return null;
    }
    }
}