using System;
namespace SmartHome
{
    public interface ISmartHomeIterator
    {
        bool HasNext();
        Device Next();
        void Reset();

    }
    public class SmartHomeIterator:ISmartHomeIterator
    {
        private Room _room;
        private int _currentIndex=0;
        public SmartHomeIterator(Room room)
        {
            _room=room;
        }
        public bool HasNext()=>_currentIndex<_room.GetDevices().Count;
        public Device Next() => _room.GetDevices()[_currentIndex++];
        public void Reset()=> _currentIndex=0;
    }
}