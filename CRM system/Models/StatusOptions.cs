using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRM_system.Models
{
    public static class StatusOptions
    {
        public static readonly List<string> All = new()
        {
            "Новая",
            "Завершена",
            "Отменена"
        };
    }
}
