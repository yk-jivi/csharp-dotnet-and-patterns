using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookStore
{
    public interface IUndoRedoCommand
    {
        void Execute();
        void Undo();
    }

    public class AddBookCommand : IUndoRedoCommand
    {
        private readonly MainViewModel _viewModel;
        private readonly Book _book;

        public AddBookCommand(MainViewModel viewModel, Book book)
        {
            _viewModel = viewModel;
            _book = book;
        }

        public void Execute() => _viewModel.AddBookDirect(_book);
        public void Undo() => _viewModel.RemoveBookDirect(_book.Id);
    }

    public class RemoveBookCommand : IUndoRedoCommand
    {
        private readonly MainViewModel _viewModel;
        private readonly Book _book;

        public RemoveBookCommand(MainViewModel viewModel, Book book)
        {
            _viewModel = viewModel;
            _book = book;
        }

        public void Execute() => _viewModel.RemoveBookDirect(_book.Id);
        public void Undo() => _viewModel.AddBookDirect(_book);
    }

    public class EditBookCommand : IUndoRedoCommand
    {
        private readonly MainViewModel _viewModel;
        private readonly Book _oldBook;
        private readonly Book _newBook;

        public EditBookCommand(MainViewModel viewModel, Book oldBook, Book newBook)
        {
            _viewModel = viewModel;
            _oldBook = oldBook;
            _newBook = newBook;
        }

        public void Execute() => _viewModel.EditBookDirect(_newBook);
        public void Undo() => _viewModel.EditBookDirect(_oldBook);
    }

    public class CommandHistory
    {
        private Stack<IUndoRedoCommand> _undoStack = new Stack<IUndoRedoCommand>();
        private Stack<IUndoRedoCommand> _redoStack = new Stack<IUndoRedoCommand>();
        private readonly MainViewModel _viewModel;

        public CommandHistory(MainViewModel viewModel)
        {
            _viewModel = viewModel;
        }

        public void ExecuteCommand(IUndoRedoCommand command)
        {
            command.Execute();
            _undoStack.Push(command);
            _redoStack.Clear();
            _viewModel.FilterBooks();
        }

        public void Undo()
        {
            if (_undoStack.Count > 0)
            {
                var command = _undoStack.Pop();
                command.Undo();
                _redoStack.Push(command);
            }
            _viewModel.FilterBooks();
        }

        public void Redo()
        {
            if (_redoStack.Count > 0)
            {
                var command = _redoStack.Pop();
                command.Execute();
                _undoStack.Push(command);
            }
            _viewModel.FilterBooks();
        }

        public bool CanUndo => _undoStack.Count > 0;
        public bool CanRedo => _redoStack.Count > 0;
    }
}