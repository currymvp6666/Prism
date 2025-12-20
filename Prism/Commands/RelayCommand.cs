using System;
using System.Threading.Tasks;
using System.Windows.Input;

public class RelayCommand : ICommand
{
    private readonly Func<object, Task> _executeAsync;
    private readonly Action<object> _executeSync;
    private readonly Func<object, bool> _canExecute;

    public event EventHandler CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }

    // 1. 构造函数：支持异步方法 (Func<object, Task>)
    public RelayCommand(Func<object, Task> executeAsync, Func<object, bool> canExecute = null)
    {
        _executeAsync = executeAsync;
        _canExecute = canExecute;
    }

    // 2. 构造函数：支持同步方法 (Action<object>)
    public RelayCommand(Action<object> executeSync, Func<object, bool> canExecute = null)
    {
        _executeSync = executeSync;
        _canExecute = canExecute;
    }

    public bool CanExecute(object parameter) => _canExecute?.Invoke(parameter) ?? true;

    public async void Execute(object parameter)
    {
        if (_executeAsync != null)
        {
            await _executeAsync(parameter);
        }
        else
        {
            _executeSync?.Invoke(parameter);
        }
    }

    // 手动触发验证状态更新
    public void RaiseCanExecuteChanged()
    {
        CommandManager.InvalidateRequerySuggested();
    }
}