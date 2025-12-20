using Prism.Models;
using Prism.Services;
using Prism.Views;

using System;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Prism.ViewModels
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        private readonly LoginService _loginService;
        private readonly MailService _mailService = new MailService();


        #region 登录属性
        private string _generatedCode; // 后台生成的随机码
        private string _verificationCode; // 用户输入的验证码
        private string _newPassword; // 用户输入的新密码
        private string _sendBtnText = "获取验证码";
        private bool _isSendBtnEnabled = true;

        public string VerificationCode
        {
            get => _verificationCode;
            set
            {
                if (_verificationCode != value)
                {
                    _verificationCode = value;
                    OnPropertyChanged(nameof(VerificationCode));
                }
            }
        }
        public string NewPassword
        {
            get => _newPassword;
            set
            {
                if (_newPassword != value)
                {
                    _newPassword = value;
                    OnPropertyChanged(nameof(NewPassword));
                }
            }
        }
        public string SendBtnText
        {
            get => _sendBtnText;
            set
            {
                if (_sendBtnText != value)
                {
                    _sendBtnText = value;
                    OnPropertyChanged(nameof(SendBtnText));
                }
            }
        }
        public bool IsSendBtnEnabled
        {
            get => _isSendBtnEnabled;
            set
            {
                if (_isSendBtnEnabled != value)
                {
                    _isSendBtnEnabled = value;
                    OnPropertyChanged(nameof(IsSendBtnEnabled));
                }
            }
        }
        #endregion



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
        public ICommand ShowResetPswCommand { get; }
        public ICommand SendCodeCommand { get; }
        public ICommand ConfirmResetCommand { get; }

        public LoginViewModel()
        {
            _loginService = new LoginService();
            LoginModel = new Login(); // 初始化模型，防止 View 报空引用

            // 3. 初始化命令（绑定异步方法）
            LoginCommand = new RelayCommand(async (o) => await ExecuteLogin());
            RegisterCommand = new RelayCommand(async (o) => await ExecuteRegister());
            RegisterToShowCommand = new RelayCommand(async (o) => await ExecuteToShowRegister());
            BackToLoginCommand = new RelayCommand(o => CloseCurWindowToLogin());
            ShowResetPswCommand = new RelayCommand(o => ShowResetWindow());
            SendCodeCommand = new RelayCommand(async _ => await ExecuteSendCode());
            ConfirmResetCommand = new RelayCommand(async _ => await ExecuteConfirmReset());
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



        private async Task ExecuteRegister()
        {
            // 1. 基础非空校验
            if (string.IsNullOrWhiteSpace(LoginModel.UserName) ||
                string.IsNullOrWhiteSpace(LoginModel.Email) ||
                string.IsNullOrWhiteSpace(LoginModel.UserPsw) ||
                string.IsNullOrWhiteSpace(LoginModel.UserConfirmPsw))
            {
                MessageBox.Show("请填写完整的注册信息");
                return;
            }
            // --- 2. 邮箱格式校验（正则表达式） ---
            // 这是一个通用的邮箱验证正则
            string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            if (!Regex.IsMatch(LoginModel.Email, emailPattern))
            {
                MessageBox.Show("邮箱格式不正确，请输入有效的邮箱地址（如：example@qq.com）");
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
                    CloseCurWindowToLogin();
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









        private async Task ExecuteSendCode()
        {
            if (string.IsNullOrWhiteSpace(LoginModel.Email))
            {
                MessageBox.Show("请先输入注册时的邮箱地址。");
                return;
            }

            // 2. 关键核心逻辑：校验用户名和邮箱是否匹配
            var matchResult = await _loginService.VerifyUserEmailMatchAsync(LoginModel.UserName, LoginModel.Email);
            if (!matchResult.IsSuccess)
            {
                // 如果不匹配，直接拦截，不发邮件
                MessageBox.Show(matchResult.Message);
                return;
            }


            // 1. 生成6位随机验证码
            _generatedCode = new Random().Next(100000, 999999).ToString();

            // 2. 按钮进入倒计时状态
            IsSendBtnEnabled = false;

            // 3. 调用你的 MailService 发送真实邮件
            var result = await _mailService.SendCodeAsync(LoginModel.Email, _generatedCode);

            if (result.success)
            {
                MessageBox.Show("验证码已发送，请查收邮箱。");
                StartTimer(); // 启动60秒倒计时方法
            }
            else
            {
                MessageBox.Show(result.message);
                IsSendBtnEnabled = true;
            }
        }

        // 倒计时逻辑
        private void StartTimer()
        {
            int seconds = 60;
            var timer = new System.Windows.Threading.DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            timer.Tick += (s, e) =>
            {
                if (seconds > 0)
                {
                    seconds--;
                    SendBtnText = $"{seconds}s后重试";
                }
                else
                {
                    timer.Stop();
                    SendBtnText = "获取验证码";
                    IsSendBtnEnabled = true;
                }
            };
            timer.Start();
        }


        //找回密码
        private async Task ExecuteConfirmReset()
        {
            // --- 1. 前端基础校验 ---
            if (string.IsNullOrWhiteSpace(LoginModel.Email))
            {
                MessageBox.Show("邮箱不能为空");
                return;
            }
            if (string.IsNullOrWhiteSpace(VerificationCode))
            {
                MessageBox.Show("请输入验证码");
                return;
            }
            if (string.IsNullOrWhiteSpace(NewPassword))
            {
                MessageBox.Show("请输入新密码");
                return;
            }

            // --- 2. 验证码校验 ---
            // _generatedCode 是你发送邮件时在后台生成的随机数
            if (VerificationCode != _generatedCode)
            {
                MessageBox.Show("验证码错误，请重新输入");
                return;
            }

            // --- 3. 调用 Service 执行修改 ---
            var result = await _loginService.UpdatePasswordByEmailAsync(LoginModel.Email, NewPassword);

            if (result.IsSuccess)
            {
                MessageBox.Show("密码已成功重置！");

                // 成功后关闭重置窗口并跳转回登录
                CloseCurWindowToLogin();
            }
            else
            {
                MessageBox.Show(result.Message);
            }
        }











        #region 退出当前界面到其他界面
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

        private void ShowResetWindow()
        {
            var resetPasswordView = new ResetPasswordView();
            resetPasswordView.Show();
            foreach (Window item in Application.Current.Windows)
            {
                if (item is Views.LoginView)
                {
                    item.Close();
                    break;
                }
            }
        }
        private void CloseCurWindowToLogin()
        {
            var loginView = new Views.LoginView();
            loginView.Show();
            foreach (Window item in Application.Current.Windows)
            {
                if (item is Views.ResetPasswordView)
                {
                    item.Close();
                    break;
                }
                else if (item is Views.RegisterView)
                {
                    item.Close();
                    break;
                }
            }
        }
        #endregion



        #region 属性更改通知
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        #endregion



    }
}