using CommunityToolkit.Mvvm.Input;

using Prism.Views;

using System;
using System.ComponentModel;
using System.Windows.Input;

using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Prism.ViewModel
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {


        private object _currentView;
        public object CurrentView
        {
            get => _currentView;
            set
            {
                _currentView = value;
                OnPropertyChanged(nameof(CurrentView));
            }
        }


        public ICommand NavigateCommand { get; }

        public MainWindowViewModel()
        {
            // 默认显示 Dashboard
            CurrentView = new DashboardView();

            NavigateCommand = new RelayCommand(async p =>
            {
                Navigate(p as string);
                await Task.CompletedTask;
            });

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
        #region 属性变更
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        #endregion
    }
}
