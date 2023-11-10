using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace RevengineEditor.Utilities
{
    public interface IUndoRedo
    {
        string Name { get; }
        void Undo();
        void Redo();
    }

    public class UndoRedoAction : IUndoRedo
    {
        private Action _undoAction;
        private Action _redoAction;

        public string Name { get; }

        public UndoRedoAction(string name)
        {
            Name = name;
        }

        public UndoRedoAction(Action undo, Action redo, string name)
        {
            Debug.Assert(undo != null && redo != null);
            _undoAction = undo;
            _redoAction = redo;
            Name = name;
        }

        public void Undo() => _undoAction();

        public void Redo() => _redoAction();
    }

    public class UndoRedo
    {
        private readonly ObservableCollection<IUndoRedo> _undoList = new ObservableCollection<IUndoRedo>();
        private readonly ObservableCollection<IUndoRedo> _redoList = new ObservableCollection<IUndoRedo>();

        public ReadOnlyObservableCollection<IUndoRedo> UndoList { get; }
        public ReadOnlyObservableCollection<IUndoRedo> RedoList { get; }

        public UndoRedo()
        {
            RedoList = new ReadOnlyObservableCollection<IUndoRedo>(_redoList);
            UndoList = new ReadOnlyObservableCollection<IUndoRedo>(_undoList);
        }

        /// <summary>
        /// Reset the Undo and Redo lists
        /// </summary>
        public void Reset()
        {
            _redoList.Clear();
            _undoList.Clear();
        }

        /// <summary>
        /// Add a command to the Undo list
        /// </summary>
        /// <param name="cmd"></param>
        public void Add(IUndoRedo cmd)
        {
            // Add the command to the undo list
            _undoList.Add(cmd);

            // Clear the redo list
            _redoList.Clear();
        }

        /// <summary>
        /// Undo a command
        /// </summary>
        public void Undo()
        {
            if(_undoList.Any())
            {
                // Get the last command in the undo list
                IUndoRedo cmd = _undoList.Last();

                // Remove it from the list
                _undoList.RemoveAt(_undoList.Count - 1);

                // Undo it
                cmd.Undo();

                // Add it to the front of the redo list
                _redoList.Insert(0, cmd);
            }
        }

        /// <summary>
        /// Redo a command
        /// </summary>
        public void Redo()
        {
            if(_redoList.Any())
            {
                // Get the first command in the redo list
                IUndoRedo cmd = _redoList.First();

                // Remove it from the list
                _redoList.RemoveAt(0);

                // Redo it
                cmd.Redo();

                // Add it to the end of the undo list
                _undoList.Add(cmd);
            }
        }
    }
}
