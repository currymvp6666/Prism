using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prism.Model
{
    public class Category
    {
        public int Id { get; set; }              // 主键
        public string Name { get; set; }         // 分类名（默认 / 工作 / 学习）

        // 导航属性
        public ICollection<Memo> Memos { get; set; }
        public ICollection<TodoItem> TodoItems { get; set; }
    }
}

