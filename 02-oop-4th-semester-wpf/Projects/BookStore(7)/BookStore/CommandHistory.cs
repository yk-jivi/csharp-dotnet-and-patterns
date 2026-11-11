using System.Collections.Generic;

namespace BookStore
{
    public interface IUndoRedoCommand
    {
        void Execute();
        void Undo();
    }

    public class AddBookCommand : IUndoRedoCommand
    {
        private readonly MainWindow _window;
        private readonly Book _book;

        public AddBookCommand(MainWindow window, Book book)
        {
            _window = window;
            _book = book;
        }

        public void Execute()
        {
            _window.AddBookDirect(_book);
        }

        public void Undo()
        {
            _window.RemoveBookDirect(_book.Id);
        }
    }

    public class RemoveBookCommand : IUndoRedoCommand
    {
        private readonly MainWindow _window;
        private readonly Book _book;

        public RemoveBookCommand(MainWindow window, Book book)
        {
            _window = window;
            _book = book;
        }

        public void Execute()
        {
            _window.RemoveBookDirect(_book.Id);
        }

        public void Undo()
        {
            _window.AddBookDirect(_book);
        }
    }

    public class EditBookCommand : IUndoRedoCommand
    {
        private readonly MainWindow _window;
        private readonly Book _oldBook;
        private readonly Book _newBook;

        public EditBookCommand(MainWindow window, Book oldBook, Book newBook)
        {
            _window = window;
            _oldBook = oldBook;
            _newBook = newBook;
        }

        public void Execute()
        {
            _window.EditBookDirect(_newBook);
        }

        public void Undo()
        {
            _window.EditBookDirect(_oldBook);
        }
    }

    public class CommandHistory
    {
        private Stack<IUndoRedoCommand> _undoStack = new Stack<IUndoRedoCommand>();
        private Stack<IUndoRedoCommand> _redoStack = new Stack<IUndoRedoCommand>();

        public void ExecuteCommand(IUndoRedoCommand command)
        {
            command.Execute();
            _undoStack.Push(command);
            _redoStack.Clear();
        }

        public void Undo()
        {
            if (_undoStack.Count > 0)
            {
                var command = _undoStack.Pop();
                command.Undo();
                _redoStack.Push(command);
            }
        }

        public void Redo()
        {
            if (_redoStack.Count > 0)
            {
                var command = _redoStack.Pop();
                command.Execute();
                _undoStack.Push(command);
            }
        }

        public bool CanUndo => _undoStack.Count > 0;
        public bool CanRedo => _redoStack.Count > 0;
    }
}