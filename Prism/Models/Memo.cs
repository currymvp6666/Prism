using System;
using System.ComponentModel;

namespace Prism.Model
{
    public class Memo : INotifyPropertyChanged
    {
        public int Id { get; set; }

        private string _title;
        public string Title
        {
            get => _title;
            set
            {
                if (_title != value)
                {
                    _title = value;
                    OnPropertyChanged(nameof(Title));
                }
            }
        }

        private string _content;
        public string Content
        {
            get => _content;
            set
            {
                if (_content != value)
                {
                    _content = value;
                    OnPropertyChanged(nameof(Content));
                }
            }
        }

        public int CategoryId { get; set; }
        public DateTime ReminderTime { get; set; }

        public DateTime CreateTime { get; set; } = DateTime.Now;

        private DateTime _updateTime = DateTime.Now;
        public DateTime UpdateTime
        {
            get => _updateTime;
            set
            {
                _updateTime = value;
                OnPropertyChanged(nameof(UpdateTime));
            }
        }

        // 导航属性
        public Category Category { get; set; }

        // 🔔 通知机制
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
