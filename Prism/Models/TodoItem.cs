using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Prism.Model
{
    public class TodoItem : INotifyPropertyChanged
    {
        public int Id { get; set; }

        private string _title;

        private string _description;
        public int CategoryId { get; set; }
        public DateTime CreatedTime { get; set; }

        public Category Category { get; set; }

        private DateTime _dueDate;

        private bool _isCompleted;
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

        
        public string Description
        {
            get => _description;
            set
            {
                _description = value;
                OnPropertyChanged(nameof(Description));
            }
        }


        public DateTime DueDate
        {
            get => _dueDate;
            set
            {
                _dueDate = value;
                OnPropertyChanged(nameof(DueDate));
            }
        }


        public bool IsCompleted
        {
            get => _isCompleted;
            set
            {
                if (_isCompleted != value)
                {
                    _isCompleted = value;
                    OnPropertyChanged(nameof(IsCompleted));
                }
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

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
