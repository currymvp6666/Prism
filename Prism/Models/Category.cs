using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prism.Model
{
    public class Category
    {
        public int Id { get; set; } // 主键
        public string Name { get; set; } // 分类名称

        // 导航属性：一个分类下可以有多个备忘录
        public ICollection<Memo> Memos { get; set; }
    }
}
