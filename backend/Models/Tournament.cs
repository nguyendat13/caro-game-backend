using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models
{
    [Table("Tournaments")]
    public class Tournament : Event
    {
        public string Game { get; set; }
        public string Format { get; set; }
        public int MaxPlayers { get; set; }
        public string Prize { get; set; }
        public DateTime StartDate { get; set; }
        public TimeSpan StartTime { get; set; }
    }

}
