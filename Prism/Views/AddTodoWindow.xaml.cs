using Prism.Model;

using System;
using System.Windows;

namespace Prism.Views
{
    public partial class AddTodoWindow : Window
    {
        public string TodoTitle { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }

        private TodoItem _editingTodo;

        public AddTodoWindow()
        {
            InitializeComponent();
            DueDatePicker.SelectedDate = DateTime.Now;
        }

        // 编辑模式
        public AddTodoWindow(TodoItem todo) : this()
        {
            _editingTodo = todo;
            TitleTextBox.Text = todo.Title;
            DescriptionTextBox.Text = todo.Description;
            DueDatePicker.SelectedDate = todo.DueDate;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            TodoTitle = TitleTextBox.Text.Trim();
            Description = DescriptionTextBox.Text.Trim();
            DueDate = DueDatePicker.SelectedDate ?? DateTime.Now;

            if (string.IsNullOrWhiteSpace(TodoTitle))
            {
                MessageBox.Show("标题不能为空！");
                return;
            }

            if (_editingTodo != null)
            {
                _editingTodo.Title = TodoTitle;
                _editingTodo.Description = Description;
                _editingTodo.DueDate = DueDate;
            }

            DialogResult = true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
