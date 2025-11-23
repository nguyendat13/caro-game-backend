using backend.Models;
using System.ComponentModel.DataAnnotations;

public enum ArticleCategory
{
    Strategy,
    Guide,
    News,
    Review,
    Story
}

public class Article : Event
{
    [Required]
    [MaxLength(250)]
    public string Title { get; set; }

    [Required]
    public string Content { get; set; }

    public string Thumbnail { get; set; }
    public string Tags { get; set; }
    public string YoutubeLink { get; set; }
    public bool IsGuide { get; set; } = false;

    public ArticleCategory Category { get; set; } = ArticleCategory.Strategy;

    public int? AuthorId { get; set; }
    public User Author { get; set; }
    public int Views { get; set; } = 0;
    public int Likes { get; set; } = 0;
}
