using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaxiNT.Libraries.Models
{
    [Table("Blogs")]
    public class Blog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required, MaxLength(250)]
        public string Title { get; set; } = string.Empty; // Tiêu đề bài viết

        [Required, MaxLength(250)]
        public string Slug { get; set; } = string.Empty; // ví dụ: /blogs/bai-viet-1

        [MaxLength(500)]
        public string? Summary { get; set; } // Tóm tắt ngắn gọn về bài viết

        [Required]
        public string Content { get; set; } = string.Empty; // Nội dung chính của bài viết (có thể là HTML hoặc Markdown)

        [Column(TypeName = "nvarchar(max)")]
        public string? CoverImageUrl { get; set; } // URL hình ảnh đại diện cho bài viết

        public string? Author { get; set; } = "Lưu Văn"; // Tên tác giả bài viết || sẽ mapping với bảng Users

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public bool IsPublished { get; set; } = true; // Trạng thái xuất bản

        public int ViewCount { get; set; } = 0; // Số lượt xem bài viết

        // SEO / tiện ích thêm (nếu bạn muốn mở rộng sau này)
        [MaxLength(160)]
        public string? MetaDescription { get; set; } // Thẻ meta SEO
                                                     // Thêm khu vực tin (ví dụ: Cần Thơ, Vĩnh Long, Việt Nam,...)
        [MaxLength(100)]
        public string? Category { get; set; }

        // Thẻ SEO / Tag (dạng "tin tức, công nghệ, startup")
        [MaxLength(250)]
        public string? Tags { get; set; }
    }
}
