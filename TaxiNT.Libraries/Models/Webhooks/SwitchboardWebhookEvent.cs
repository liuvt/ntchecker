using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxiNT.Libraries.Models.Webhooks
{
    public class SwitchboardWebhookEvent
    {
    }

    public class CallLog
    {
        public int Id { get; set; } // Khóa chính
        public string Action { get; set; } = string.Empty;  // note_call | hangup_call
        public string Source { get; set; } = string.Empty;  // số gọi
        public string Destination { get; set; } = string.Empty; // số nhận
        public string UniqueId { get; set; } = string.Empty; // mã uniqueid
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // thời gian ghi log
    }
}
