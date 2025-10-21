namespace backend.DTOs
{
    public class GameDTO
    {
        public int Id { get; set; }
        public int PlayerXId { get; set; }
        public int PlayerOId { get; set; }
        public string Status { get; set; }  // Ongoing / Finished
        public int? WinnerId { get; set; } // nếu có
        public DateTime CreatedAt { get; set; }
    }
}
