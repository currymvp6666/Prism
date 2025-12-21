using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prism.Models
{
    public class AiCommand
    {
        public string Action { get; set; }   // add_memo / add_todo / chat
        public string Title { get; set; }
        public string Content { get; set; }
    }
}

