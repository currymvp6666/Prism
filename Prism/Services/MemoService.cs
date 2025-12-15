using Prism.Data;
using Prism.Model;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Prism.Services
{
    public class MemoService
    {
        // 获取所有备忘录
        public async Task<List<Memo>> GetAllMemosAsync()
        {
            using var db = new PrismDbContext();
            return await db.Memos
                .Include(m => m.Category) // 加载分类信息
                .OrderByDescending(m => m.CreateTime)
                .ToListAsync();
        }

        // 添加新备忘录
        public async Task AddMemoAsync(Memo memo)
        {
            using var db = new PrismDbContext();
            db.Memos.Add(memo);
            await db.SaveChangesAsync();
        }

        // 更新备忘录
        public async Task UpdateMemoAsync(Memo memo)
        {
            using var db = new PrismDbContext();
            db.Memos.Update(memo);
            await db.SaveChangesAsync();
        }

        // 删除备忘录
        public async Task DeleteMemoAsync(int memoId)
        {
            using var db = new PrismDbContext();
            var memo = await db.Memos.FindAsync(memoId);
            if (memo != null)
            {
                db.Memos.Remove(memo);
                await db.SaveChangesAsync();
            }
        }
    }
}
