using Prism.Data;
using Prism.Model;
using Prism.Common;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace Prism.ViewModel
{
    public class MemoViewModel
    {
        public ObservableCollection<Memo> Memos { get; set; } = new ObservableCollection<Memo>();
        public Memo SelectedMemo { get; set; }

        public string NewTitle { get; set; }
        public string NewContent { get; set; }

        public ICommand AddMemoCommand { get; }

        public MemoViewModel()
        {
            AddMemoCommand = new RelayCommand(AddMemo);
            LoadMemos();
        }

        private void LoadMemos()
        {
            using var db = new PrismDbContext();
            var list = db.Memos.OrderByDescending(m => m.CreateTime).ToList();
            Memos.Clear();
            foreach (var memo in list)
                Memos.Add(memo);
        }

        private void AddMemo()
        {
            if (string.IsNullOrWhiteSpace(NewTitle)) return;

            using var db = new PrismDbContext();
            var memo = new Memo
            {
                Title = NewTitle,
                Content = NewContent,
                CategoryId = 1,
                ReminderTime = DateTime.Now,
                CreateTime = DateTime.Now,
                UpdateTime = DateTime.Now
            };
            db.Memos.Add(memo);
            db.SaveChanges();

            Memos.Insert(0, memo);

            NewTitle = "";
            NewContent = "";
        }
    }
}
