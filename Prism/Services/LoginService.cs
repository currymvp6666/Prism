using Prism.Data;
using Prism.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Prism.Services
{
    public class LoginService
    {
        private readonly PrismDbContext _db;
        public LoginService()
        {
            _db = new PrismDbContext(); // 简单起见直接 new，正式项目建议用依赖注入
        }
        // 验证用户
        public async Task<bool> ValidateUserAsync(string username, string password)
        {
            using (var context = new PrismDbContext())
            {
                // 实际项目中这里应对 password 进行哈希对比
                return await context.Logins.AnyAsync(u => u.UserName == username && u.UserPsw == password);
            }
        }

        public async Task<ServiceResult> RegisterUserAsync(Login registerInfo)
        {
            try
            {
                // 2. 检查用户名是否存在 (利用你配置的 HasIndex 唯一性约束)
                // 使用 ToLower() 确保用户名不区分大小写
                var exists = await _db.Logins.AnyAsync(u => u.UserName.ToLower() == registerInfo.UserName.ToLower());
                if (exists)
                {
                    return ServiceResult.Failure("用户名已存在，请换一个吧。");
                }

                // 3. 创建新的实体对象
                // 注意：不要直接把 ViewModel 传进来的对象 Add 进去，最好新建一个
                var newUser = new Login
                {
                    UserName = registerInfo.UserName,
                    UserPsw = registerInfo.UserPsw // 建议此处先手动测试，后期再加加密逻辑
                };

                // 4. 保存到数据库
                _db.Logins.Add(newUser);
                int result = await _db.SaveChangesAsync();

                if (result > 0)
                    return ServiceResult.Success();
                else
                    return ServiceResult.Failure("保存失败，请重试。");
            }
            catch (Exception ex)
            {
                // 如果数据库报错（比如服务器没开），这里能抓到具体原因
                return ServiceResult.Failure($"数据库错误: {ex.Message}");
            }
        }
    }
    public class ServiceResult
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }

        public static ServiceResult Success() => new ServiceResult { IsSuccess = true };
        public static ServiceResult Failure(string msg) => new ServiceResult { IsSuccess = false, Message = msg };
    }
}