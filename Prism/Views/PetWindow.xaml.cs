using System.Windows;
using System.Windows.Input;

using Prism.ViewModel;

namespace Prism.Views
{
    public partial class PetWindow : Window
    {
        public PetWindow()
        {
            InitializeComponent();
            this.DataContext = new PetViewModel();

            // 定位到右下角
            this.Left = SystemParameters.WorkArea.Width - this.Width - 50;
            this.Top = SystemParameters.WorkArea.Height - this.Height - 50;
        }

        // 窗口拖动
        private void Pet_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // 调用缩放命令（可选）
            if (DataContext is PetViewModel vm) vm.PetPressCommand.Execute(null);

            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void Pet_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // 恢复缩放
            if (DataContext is PetViewModel vm) vm.PetReleaseCommand.Execute(null);
        }
    }
}