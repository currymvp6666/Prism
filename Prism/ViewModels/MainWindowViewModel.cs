using Prism.Views;

using System;
using System.ComponentModel;
using System.Windows.Input;

using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Prism.ViewModel
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private object _currentView;
        public object CurrentView
        {
            get => _currentView;
            set
            {
                _currentView = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentView)));
            }
        }

        public ICommand NavigateCommand { get; }

        public MainWindowViewModel()
        {
            // 默认显示 Dashboard
            CurrentView = new DashboardView();

            NavigateCommand = new RelayCommand<string>(Navigate);
        }

        private void Navigate(string page)
        {
            switch (page)
            {
                case "Dashboard":
                    CurrentView = new DashboardView();
                    break;
                case "Todo":
                    CurrentView = new TodoView();
                    break;
                case "Memo":
                    CurrentView = new MemoView();
                    break;
                    //case "Calendar":
                    //    CurrentView = new CalendarView();
                    //    break;
                    //case "Settings":
                    //    CurrentView = new SettingsView();
                    //    break;
            }
        }
    }

    // 简单 RelayCommand
    public class RelayCommand<T> : ICommand
    {
        private readonly Action<T> _execute;
        public RelayCommand(Action<T> execute) => _execute = execute;
        public bool CanExecute(object parameter) => true;
        public event EventHandler CanExecuteChanged;
        public void Execute(object parameter) => _execute((T)parameter);
    }
}
