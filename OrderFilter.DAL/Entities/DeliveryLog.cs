using OrderFilter.DAL.Enums;
using System.ComponentModel.DataAnnotations;

namespace OrderFilter.DAL.Entities
{
    public class DeliveryLog
    {
        public long Id { get; set; }
        public LogType Type { get; set; }
        public string Message { get; set; }
        public DateTime TimeStamp { get; set; }
        public string Source { get; set; }
    }
}
