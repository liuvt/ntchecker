using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxiNT.Libraries.Models.Zalos
{
    public class ZCustomer
    {
        public string pid { get; set; } = string.Empty; // id bạn bè hoặc là id nhóm
        public string page_pid { get; set; } = string.Empty; // id bot
    }
}
