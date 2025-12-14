using System;

namespace Prism.Model
{
    public class Memo
    {
        public int Id { get; set; } // 主键
        public string Title { get; set; } // 标题
        public string Content { get; set; } // 内容
        public int CategoryId { get; set; } // 外键，关联分类
        public DateTime ReminderTime { get; set; } // 提醒时间
        public DateTime CreateTime { get; set; } = DateTime.Now; // 创建时间
        public DateTime UpdateTime { get; set; } = DateTime.Now; // 更新时间

        // 导航属性
        public Category Category { get; set; }
    }
}
