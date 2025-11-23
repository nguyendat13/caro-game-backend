namespace backend.Models
{
    public class EventFeature
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public int EventId { get; set; }
        public Event Event { get; set; } = null!;
    }

}
