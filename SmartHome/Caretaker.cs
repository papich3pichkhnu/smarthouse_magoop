using System.Collections.Generic;

namespace SmartHome
{
    public class Caretaker<TMemento>
    {
        private readonly List<TMemento> _mementos = new List<TMemento>();
        private int _currentIndex = -1;

        public void SaveState(TMemento memento)
        {
            if (_currentIndex + 1 < _mementos.Count)
            {
                _mementos.RemoveRange(_currentIndex + 1, _mementos.Count - (_currentIndex + 1));
            }
            _mementos.Add(memento);
            _currentIndex++;
        }

        public TMemento Undo()
        {
            if (_currentIndex <= 0)
            {
                return default(TMemento);
            }
            _currentIndex--;
            return _mementos[_currentIndex];
        }

        public TMemento Redo()
        {
            if (_currentIndex + 1 >= _mementos.Count)
            {
                return default(TMemento);
            }
            _currentIndex++;
            return _mementos[_currentIndex];
        }

        public bool CanUndo() => _currentIndex > 0;
        public bool CanRedo() => _currentIndex + 1 < _mementos.Count;
    }
}
