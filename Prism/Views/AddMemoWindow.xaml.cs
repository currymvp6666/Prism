using Prism.Model;

using System;
using System.Windows;

namespace Prism.Views
{
    public partial class AddMemoWindow : Window
    {
        // 对外暴露标题和内容
        public string MemoTitle { get; set; }
        public string MemoContent { get; set; }

        // 可选：当前编辑的Memo对象
        private Memo _editingMemo;

        public AddMemoWindow()
        {
            InitializeComponent();
        }

        // 构造函数：传入已有Memo，用于编辑
        public AddMemoWindow(Memo memo) : this()
        {
            _editingMemo = memo;
            TitleTextBox.Text = memo.Title;
            ContentTextBox.Text = memo.Content;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            MemoTitle = TitleTextBox.Text.Trim();
            MemoContent = ContentTextBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(MemoTitle))
            {
                MessageBox.Show("标题不能为空！");
                return;
            }

            if (_editingMemo != null)
            {
                // 编辑模式：更新原对象
                _editingMemo.Title = MemoTitle;
                _editingMemo.Content = MemoContent;
                _editingMemo.UpdateTime = DateTime.Now;
            }

            DialogResult = true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
