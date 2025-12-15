using Prism.Data;
using Prism.Model;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Prism.Services
{
    public class TodoService
    {
        // 获取所有待办事项
        public async Task<List<TodoItem>> GetAllTodosAsync()
        {
            using var db = new PrismDbContext();
            return await db.TodoItems
                           .Include(t => t.Category) // 如果需要分类信息
                           .OrderByDescending(t => t.CreatedTime)
                           .ToListAsync();
        }

        // 添加新的待办事项
        public async Task AddTodoAsync(TodoItem todo)
        {
            using var db = new PrismDbContext();
            db.TodoItems.Add(todo);
            await db.SaveChangesAsync();
        }

        // 删除待办事项
        public async Task DeleteTodoAsync(int todoId)
        {
            using var db = new PrismDbContext();
            var todo = await db.TodoItems.FindAsync(todoId);
            if (todo != null)
            {
                db.TodoItems.Remove(todo);
                await db.SaveChangesAsync();
            }
        }

        // 更新待办事项（如完成状态）
        public async Task UpdateTodoAsync(TodoItem todo)
        {
            using var db = new PrismDbContext();
            db.TodoItems.Update(todo);
            await db.SaveChangesAsync();
        }

        // 根据分类获取待办事项
        public async Task<List<TodoItem>> GetTodosByCategoryAsync(int categoryId)
        {
            using var db = new PrismDbContext();
            return await db.TodoItems
                           .Where(t => t.CategoryId == categoryId)
                           .OrderByDescending(t => t.CreatedTime)
                           .ToListAsync();
        }
    }
}
