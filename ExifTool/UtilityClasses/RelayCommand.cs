using System;
using System.Diagnostics;
using System.Windows.Input;

namespace ExifTool.UtilityClasses
{
  public class RelayCommand : ICommand
  {
    readonly Func<Boolean> _canExecute;
    readonly Action _execute;

    // Konstruktoren
    public RelayCommand(Action execute)
      : this(execute, null)
    {
    }

    public RelayCommand(Action execute, Func<Boolean> canExecute)
    {
      if (execute == null)
      {
        throw new ArgumentNullException("Exception: execute ist 'null'");
      }

      _execute = execute;
      _canExecute = canExecute;
    }

    #region ICommand Members

    [DebuggerStepThrough]
    public bool CanExecute(object parameter)
    {
      return _canExecute == null ? true : _canExecute();
    }

    public void Execute(object parameter)
    {
      _execute();
    }

    public event EventHandler CanExecuteChanged
    {
      add
      {

        if (_canExecute != null)
        {
          CommandManager.RequerySuggested += value;
        }
      }
      remove
      {

        if (_canExecute != null)
        {
          CommandManager.RequerySuggested -= value;
        }
      }
    }

    #endregion
  }
}
