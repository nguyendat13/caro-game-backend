using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models
{
    [Table("CommunityEvents")]
    public class CommunityEvent : Event
    {
        public string Location { get; set; }
        public DateTime EventDate { get; set; }
        public string Host { get; set; }
    }

}
