using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Prism.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    
    public partial class MainWindow : Window
    {
        private PetWindow _pet;
        public MainWindow()
        {
            InitializeComponent();
            // 启动主界面时，同时启动宠物
            _pet = new PetWindow();
            _pet.Show();

            // 可选：当主窗口关闭时，宠物也跟着关闭
            this.Closed += (s, e) => _pet.Close();
        }
    }
}