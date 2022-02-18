using System;
using System.Windows.Input;

namespace _2_assemble
{
    internal class CommandBase : ICommand
    {
        private readonly Action _action;

        public event EventHandler? CanExecuteChanged;

        public CommandBase(Action action)
        {
            _action = action;
        }

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            _action();
        }

        protected void OnCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, new EventArgs());
        }

    }
}