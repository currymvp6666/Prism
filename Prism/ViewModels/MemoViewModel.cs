using CommunityToolkit.Mvvm.ComponentModel; // MVVM Toolkit

using Prism.Data;
using Prism.Model;

using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace Prism.ViewModel
{
    public partial class MemoViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<Memo> memos;

        [ObservableProperty]
        private Memo selectedMemo;

        public MemoViewModel()
        {
            LoadMemos();
        }

        public void LoadMemos()
        {
            using var db = new PrismDbContext();
            Memos = new ObservableCollection<Memo>(db.Memos.ToList());
        }
    }
}
