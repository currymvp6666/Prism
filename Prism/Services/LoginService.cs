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
                var UserName_exists = await _db.Logins.AnyAsync(u => u.UserName.ToLower() == registerInfo.UserName.ToLower());
                if (UserName_exists)
                {
                    return ServiceResult.Failure("用户名已存在，请换一个吧。");
                }
                var Email_exists = await _db.Logins.AnyAsync(u => u.Email.ToLower() == registerInfo.Email.ToLower());
                if (Email_exists)
                {
                    return ServiceResult.Failure("用户名已存在，请换一个吧。");
                }

                // 3. 创建新的实体对象
                // 注意：不要直接把 ViewModel 传进来的对象 Add 进去，最好新建一个
                var newUser = new Login
                {
                    UserName = registerInfo.UserName,
                    Email = registerInfo.Email,
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
        public async Task<ServiceResult> UpdatePasswordByEmailAsync(string email, string newPassword)
        {
            try
            {
                // 1. 在数据库中查找该邮箱对应的用户
                var user = await _db.Logins.FirstOrDefaultAsync(u => u.Email == email);

                if (user == null)
                {
                    return ServiceResult.Failure("未找到该邮箱绑定的用户。");
                }

                // 2. 修改密码（直接明文覆盖）
                user.UserPsw = newPassword;

                // 3. 提交修改到数据库
                await _db.SaveChangesAsync();

                return ServiceResult.Success();
            }
            catch (Exception ex)
            {
                return ServiceResult.Failure($"数据库更新失败: {ex.Message}");
            }
        }

        public async Task<ServiceResult> VerifyUserEmailMatchAsync(string userName, string email)
        {
            try
            {
                // 查找用户名和邮箱同时匹配的记录
                var user = await _db.Logins.FirstOrDefaultAsync(u => u.UserName == userName && u.Email == email);

                if (user == null)
                {
                    return ServiceResult.Failure("用户名与绑定的邮箱不匹配，请核对信息。");
                }

                return ServiceResult.Success();
            }
            catch (Exception ex)
            {
                return ServiceResult.Failure($"数据库查询异常: {ex.Message}");
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