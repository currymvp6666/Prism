using CommunityToolkit.Mvvm.Input;

using Prism.Model;
using Prism.Services;
using Prism.Views;

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq; // ★ 必须
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Prism.ViewModel
{
    public class MemoViewModel : INotifyPropertyChanged
    {
        //——————————————————————————————————————————————————
        private readonly MemoService _memoService;
        private readonly TodoService _todoService;

        public ObservableCollection<Memo> Memos { get; set; } = new();
        public ObservableCollection<TodoItem> TodoItems { get; set; } = new();
        public ObservableCollection<ActivityItem> RecentActivities { get; set; } = new();

        public ICommand ShowAddMemoDialogCommand { get; }
        public ICommand ShowAddTodoDialogCommand { get; }
        public ICommand EditMemoCommand { get; }
        public ICommand EditTodoCommand { get; }   // ★ 补上

        //——————————————————————————————————————————————————
        public MemoViewModel()
        {
            _memoService = new MemoService();
            _todoService = new TodoService();

            ShowAddMemoDialogCommand = new RelayCommand(async () => await ShowAddMemoDialogAsync());
            ShowAddTodoDialogCommand = new RelayCommand(async () => await ShowAddTodoDialogAsync());
            EditMemoCommand = new RelayCommand<Memo>(async m => await EditMemoAsync(m));
            EditTodoCommand = new RelayCommand<TodoItem>(async t => await EditTodoAsync(t));

            Memos.CollectionChanged += (_, __) => UpdateStatistics();
            TodoItems.CollectionChanged += (_, __) => UpdateStatistics();

            _ = LoadMemosAsync();
            _ = LoadTodoItemsAsync();
        }

        //——————————————————————————————————————————————————
        #region 统计属性
        private int _totalCount;
        public int TotalCount
        {
            get => _totalCount;
            set { _totalCount = value; OnPropertyChanged(nameof(TotalCount)); }
        }

        private int _completedCount;
        public int CompletedCount
        {
            get => _completedCount;
            set
            {
                _completedCount = value;
                OnPropertyChanged(nameof(CompletedCount));
                OnPropertyChanged(nameof(CompletionRate));
            }
        }

        public double CompletionRate =>
            TotalCount == 0 ? 0 : Math.Round((double)CompletedCount / TotalCount * 100, 1);

        private int _memoCount;
        public int MemoCount
        {
            get => _memoCount;
            set { _memoCount = value; OnPropertyChanged(nameof(MemoCount)); }
        }

        private string _currentViewTitle = "仪表盘";
        public string CurrentViewTitle
        {
            get => _currentViewTitle;
            set { _currentViewTitle = value; OnPropertyChanged(nameof(CurrentViewTitle)); }
        }

        public string CurrentDate => DateTime.Now.ToString("yyyy年MM月dd日 dddd");
        #endregion

        //——————————————————————————————————————————————————
        #region 数据加载
        private async Task LoadMemosAsync()
        {
            var memos = await _memoService.GetAllMemosAsync();
            Memos.Clear();
            foreach (var m in memos)
                Memos.Add(m);
        }

        private async Task LoadTodoItemsAsync()
        {
            var todos = await _todoService.GetAllTodosAsync();
            TodoItems.Clear();
            foreach (var t in todos)
            {
                t.PropertyChanged += Todo_PropertyChanged;
                TodoItems.Add(t);
            }
        }
        #endregion

        //——————————————————————————————————————————————————
        #region Todo 状态变化
        private void Todo_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(TodoItem.IsCompleted))
            {
                var todo = sender as TodoItem;

                AddRecentActivity(
                    todo.IsCompleted
                        ? $"完成了任务: {todo.Title}"
                        : $"取消完成: {todo.Title}",
                    todo.IsCompleted ? "#4CAF50" : "#FF9800");

                UpdateStatistics();
            }
        }
        #endregion

        //——————————————————————————————————————————————————
        #region 统计
        private void UpdateStatistics()
        {
            MemoCount = Memos.Count;
            TotalCount = TodoItems.Count;                    // ✔ 只算 Todo
            CompletedCount = TodoItems.Count(t => t.IsCompleted);
        }
        #endregion

        //——————————————————————————————————————————————————
        #region 新增
        private async Task ShowAddMemoDialogAsync()
        {
            var window = new AddMemoWindow { Owner = Application.Current.MainWindow };
            if (window.ShowDialog() == true)
            {
                var memo = new Memo
                {
                    Title = window.MemoTitle,
                    Content = window.MemoContent,
                    CategoryId = 1,
                    ReminderTime = DateTime.Now,
                    CreateTime = DateTime.Now,
                    UpdateTime = DateTime.Now
                };

                await _memoService.AddMemoAsync(memo);
                Memos.Insert(0, memo);

                AddRecentActivity($"+ 添加了备忘录: {memo.Title}", "#2196F3");
            }
        }

        private async Task ShowAddTodoDialogAsync()
        {
            var window = new AddTodoWindow { Owner = Application.Current.MainWindow };
            if (window.ShowDialog() == true)
            {
                var todo = new TodoItem
                {
                    Title = window.TodoTitle,
                    Description = window.Description,
                    DueDate = window.DueDate, // ★ 安全
                    CategoryId = 1,
                    IsCompleted = false,
                    CreatedTime = DateTime.Now
                };

                todo.PropertyChanged += Todo_PropertyChanged;
                await _todoService.AddTodoAsync(todo);
                TodoItems.Insert(0, todo);

                AddRecentActivity($"+ 添加了待办事项: {todo.Title}", "#4CAF50");
            }
        }
        #endregion

        //——————————————————————————————————————————————————
        #region 编辑
        private async Task EditMemoAsync(Memo memo)
        {
            if (memo == null) return;

            var window = new AddMemoWindow(memo) { Owner = Application.Current.MainWindow };
            if (window.ShowDialog() == true)
            {
                await _memoService.UpdateMemoAsync(memo);
                AddRecentActivity($"✎ 修改了备忘录: {memo.Title}", "#FF9800");
            }
        }

        private async Task EditTodoAsync(TodoItem todo)
        {
            if (todo == null) return;

            var window = new AddTodoWindow(todo) { Owner = Application.Current.MainWindow };
            if (window.ShowDialog() == true)
            {
                await _todoService.UpdateTodoAsync(todo);
                AddRecentActivity($"✎ 修改了待办事项: {todo.Title}", "#FF9800");
            }
        }
        #endregion

        //——————————————————————————————————————————————————
        #region 最近活动
        private void AddRecentActivity(string text, string color)
        {
            RecentActivities.Insert(0, new ActivityItem
            {
                Text = text,
                Color = color,
                Time = DateTime.Now
            });

            if (RecentActivities.Count > 5)
                RecentActivities.RemoveAt(RecentActivities.Count - 1);
        }
        #endregion

        //——————————————————————————————————————————————————
        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        #endregion
    }

    public class ActivityItem
    {
        public string Text { get; set; }
        public string Color { get; set; }
        public DateTime Time { get; set; }
    }
}
