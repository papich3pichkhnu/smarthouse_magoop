using System;
namespace SmartHome
{
    abstract public class RGBLampBuilder
    {
        public RGBLamp? RGBLamp{get;private set; }
        public void CreateRGBLamp(){
            RGBLamp = new RGBLamp();
        }
        public abstract void SetBrightness();
        public abstract void SetColor();
    
    }
    public class LampCreator{
        public RGBLamp? Create(RGBLampBuilder rgbLampBuilder)
        {
            rgbLampBuilder.CreateRGBLamp();
            rgbLampBuilder.SetBrightness();
            rgbLampBuilder.SetColor();
            return rgbLampBuilder.RGBLamp;
        }
    }

    public class BrightRedRGBLampBuilder : RGBLampBuilder
    {
        public override void SetBrightness()
        {
            if(this.RGBLamp != null)
            this.RGBLamp.Brightness=100;
        }

        public override void SetColor()
        {
            if(this.RGBLamp != null){
            this.RGBLamp.Red=255;
            this.RGBLamp.Green=0;
            this.RGBLamp.Blue=0;
            }
        }
    }
    public class SemiBrightGreenRGBLampBuilder : RGBLampBuilder
    {
        public override void SetBrightness()
        {
            if(this.RGBLamp != null)
            this.RGBLamp.Brightness=50;
        }

        public override void SetColor()
        {
            if(this.RGBLamp != null){
            this.RGBLamp.Red=0;
            this.RGBLamp.Green=200;
            this.RGBLamp.Blue=0;
            }
        }
    }
}