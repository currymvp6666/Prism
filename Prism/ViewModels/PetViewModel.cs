using Prism.Model;
using Prism.Models;
using Prism.Services;

using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Prism.ViewModel
{
    public class PetViewModel : INotifyPropertyChanged
    {
        private readonly DeepSeekService _deepSeekService;
        private readonly MemoService _memoService;

        // --- 属性绑定 ---
        private string _aiText = "你好呀！我是坤坤~";
        private string _userInput;
        private bool _isChatVisible = false;
        private bool _isInputEnabled = true;
        private double _scale = 1.0;

        public string AiText { get => _aiText; set { _aiText = value; OnPropertyChanged(); } }
        public string UserInput { get => _userInput; set { _userInput = value; OnPropertyChanged(); } }
        public bool IsChatVisible { get => _isChatVisible; set { _isChatVisible = value; OnPropertyChanged(); } }
        public bool IsInputEnabled { get => _isInputEnabled; set { _isInputEnabled = value; OnPropertyChanged(); } }
        public double Scale { get => _scale; set { _scale = value; OnPropertyChanged(); } }

        // --- 使用你提供的 RelayCommand ---
        public ICommand SendCommand { get; }
        public ICommand ToggleChatCommand { get; }
        public ICommand PetPressCommand { get; }
        public ICommand PetReleaseCommand { get; }

        public PetViewModel()
        {
            _deepSeekService = new DeepSeekService();
            _memoService = new MemoService();

            // 异步命令：处理 AI 对话
            SendCommand = new RelayCommand(async _ => await SendAsync());

            // 同步命令：切换 UI 状态
            ToggleChatCommand = new RelayCommand(obj => IsChatVisible = !IsChatVisible);
            PetPressCommand = new RelayCommand(obj => Scale = 0.95);
            PetReleaseCommand = new RelayCommand(obj => Scale = 1.0);
        }

        private async Task SendAsync()
        {
            var input = UserInput;
            UserInput = "";
            IsInputEnabled = false;
            AiText = "正在处理中... 🎤";

            try
            {
                // 1. 调用 DeepSeek 解析指令
                // 假设 ParseCommandAsync 返回 (AiCommand? cmd, string chatReply)
                var (command, reply) = await _deepSeekService.ParseCommandAsync(input);

                if (command != null && command.Action == "add_memo")
                {
                    // 2. 识别到备忘录动作，调用 MemoService
                    await _memoService.AddMemoAsync(new Memo
                    {
                        Title = string.IsNullOrWhiteSpace(command.Title) ? "AI 自动生成的备忘" : command.Title,
                        Content = command.Content ?? input, // 如果没解析出内容，就把用户原话存进去
                        CreateTime = DateTime.Now,
                        UpdateTime = DateTime.Now,
                        CategoryId = 1 // 默认分类
                    });

                    AiText = $"哎哟不错哦！已经帮你记下了：\n【{command.Title}】";
                }
                else
                {
                    // 3. 只是普通对话
                    AiText = reply;
                }
            }
            catch (Exception ex)
            {
                AiText = "出错了，你干嘛~ 😅 " + ex.Message;
            }
            finally
            {
                IsInputEnabled = true;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}