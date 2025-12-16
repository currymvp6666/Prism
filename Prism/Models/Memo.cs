using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Prism.Model
{
    public class Memo : INotifyPropertyChanged
    {
        public int Id { get; set; }

        private string _title;

        private string _content;
        public int CategoryId { get; set; }
        public DateTime ReminderTime { get; set; }

        public DateTime CreateTime { get; set; } = DateTime.Now;

        private DateTime _updateTime = DateTime.Now;
        [NotMapped]
        private bool _isVisible = true;

        
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

        public DateTime UpdateTime
        {
            get => _updateTime;
            set
            {
                _updateTime = value;
                OnPropertyChanged(nameof(UpdateTime));
            }
        }
        [NotMapped]
        public bool IsVisible
        {
            get => _isVisible;
            set
            {
                if (_isVisible != value)
                {
                    _isVisible = value;
                    OnPropertyChanged(nameof(IsVisible));
                }
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
