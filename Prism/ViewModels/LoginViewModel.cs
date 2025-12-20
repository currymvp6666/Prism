using Prism.Services;
using Prism.Models;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Prism.Views;

namespace Prism.ViewModels
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        private readonly LoginService _loginService;

        // 1. 声明数据模型（供 View 绑定）
        private Login _loginModel;
        public Login LoginModel
        {
            get => _loginModel;
            set
            {
                _loginModel = value;
                OnPropertyChanged(nameof(LoginModel));
            }
        }

        // 2. 声明命令
        public ICommand LoginCommand { get; }
        public ICommand RegisterCommand { get; }
        public ICommand RegisterToShowCommand { get; }
        public ICommand BackToLoginCommand { get; }

        public LoginViewModel()
        {
            _loginService = new LoginService();
            LoginModel = new Login(); // 初始化模型，防止 View 报空引用

            // 3. 初始化命令（绑定异步方法）
            LoginCommand = new RelayCommand(async (o) => await ExecuteLogin());
            RegisterCommand = new RelayCommand(async (o) => await ExecuteRegister());
            RegisterToShowCommand = new RelayCommand(async (o) => await ExecuteToShowRegister());
            BackToLoginCommand = new RelayCommand(async (o) => await CloseRegisterWindowasync());
        }

        // 4. 登录逻辑
        private async Task ExecuteLogin()
        {
            if (string.IsNullOrEmpty(LoginModel.UserName) || string.IsNullOrEmpty(LoginModel.UserPsw))
            {
                MessageBox.Show("请输入用户名和密码");
                return;
            }

            // 调用 Service 进行验证
            bool isValid = await _loginService.ValidateUserAsync(LoginModel.UserName, LoginModel.UserPsw);

            if (isValid)
            {
                Task.Run(() => MessageBox.Show("登录成功！"));
                var mainWindow = new Views.MainWindow();

                // 2. 显示主窗体
                mainWindow.Show();

                // 3. 关闭当前的登录窗体
                // 在 MVVM 中直接操作 Window 有多种方式，最简单的是查找当前激活的窗口
                foreach (Window item in Application.Current.Windows)
                {
                    if (item is Views.LoginView)
                    {
                        item.Close();
                        break;
                    }
                }
            }
            else
            {
                MessageBox.Show("用户名或密码错误");
            }
        }

        private async Task ExecuteToShowRegister()
        {
            var registerWindow = new RegisterView();
            registerWindow.Show();
            foreach (Window item in Application.Current.Windows)
            {
                if (item is Views.LoginView)
                {
                    item.Close();
                    break;
                }
            }
        }

        private async Task ExecuteRegister()
        {
            // 1. 基础非空校验
            if (string.IsNullOrWhiteSpace(LoginModel.UserName) ||
                string.IsNullOrWhiteSpace(LoginModel.UserPsw) ||
                string.IsNullOrWhiteSpace(LoginModel.UserConfirmPsw))
            {
                MessageBox.Show("请填写完整的注册信息");
                return;
            }
            // 2. 确认密码比对 (这是最重要的一步)
            if (LoginModel.UserPsw != LoginModel.UserConfirmPsw)
            {
                MessageBox.Show("两次输入的密码不一致，请重新输入");
                LoginModel.UserConfirmPsw = string.Empty;
                return;
            }

            // --- 2. 执行注册请求 ---
            // 可以在这里设置一个 IsLoading 状态，让按钮禁用，防止重复点击
            var result = await _loginService.RegisterUserAsync(LoginModel);

            // --- 3. 根据结果处理 UI ---
            if (result.IsSuccess)
            {
                // 成功：异步提示并强制回退到登录
                Task.Run(() => MessageBox.Show("注册成功！", "恭喜", MessageBoxButton.OK, MessageBoxImage.Information));

                App.Current.Dispatcher.Invoke(() =>
                {
                    // 弹出新的登录窗体
                    var loginView = new Views.LoginView();
                    loginView.Show();

                    // 关闭当前注册窗体
                    CloseRegisterWindow();
                });
            }
            else
            {
                // 失败：精准提示错误原因，不关闭窗体，方便用户修改
                if(result.Message== "用户名已存在，请换一个吧。")
                {
                    LoginModel.UserName = string.Empty;
                    LoginModel.UserPsw = string.Empty;
                    LoginModel.UserConfirmPsw = string.Empty;
                    MessageBox.Show(result.Message, "注册失败", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async Task CloseRegisterWindowasync()
        {
            await Task.Delay(0);
            var loginView = new Views.LoginView();
            loginView.Show();
            CloseRegisterWindow();
        }

        private void CloseRegisterWindow()
        {
            // 查找当前的 RegisterView 并关闭
            var win = App.Current.Windows.OfType<Window>().FirstOrDefault(w => w is Views.RegisterView);
            win?.Close();
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        #endregion
    }
}