using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;
using TaxiNT.Data.Models;

namespace TaxiNT.Controllers;

// Các đường dẫn có thể đánh dấu google bot không được truy cập hoặc được truy cập
[AllowAnonymous]
[Route("robots.txt")]
[ApiController]
public class RobotsController : ControllerBase
{
    private readonly ILogger<RobotsController> logger;
    public RobotsController(ILogger<RobotsController> _logger)
    {
        this.logger = _logger;
    }

    [HttpGet]
    [ResponseCache(Duration = 86400)]
    public IActionResult Index()
    {
        var sb = new StringBuilder();

        sb.AppendLine("User-agent: *");
        sb.AppendLine("Allow: /");
        sb.AppendLine("");

        sb.AppendLine("# Không cho phép bot truy cập các vùng nội bộ");
        sb.AppendLine("Disallow: /admin/");
        sb.AppendLine("Disallow: /checkers/");
        sb.AppendLine("Disallow: /api/");
        sb.AppendLine("Disallow: /_framework/");
        sb.AppendLine("");

        sb.AppendLine("# Khai báo sitemap cho Google");
        var baseUrl = $"{Request.Scheme}://{Request.Host}";
        sb.AppendLine($"Sitemap: {baseUrl}/sitemap.xml");
        sb.AppendLine($"Sitemap: {baseUrl}/video-sitemap.xml");

        return Content(sb.ToString(), "text/plain", Encoding.UTF8);
    }
}

// Sở đồ web cho Google
[AllowAnonymous]
[Route("sitemap.xml")]
[ApiController]
public class SitemapController : ControllerBase
{
    //Get API Server
    private readonly ILogger<SitemapController> logger;
    public SitemapController(ILogger<SitemapController> _logger)
    {
        this.logger = _logger;
    }

    [HttpGet]
    [ResponseCache(Duration = 86400)] // Cache 1 ngày cho Google bot
    public IActionResult Index()
    {
        var now = DateTime.UtcNow.ToString("yyyy-MM-dd");
        var baseUrl = $"{Request.Scheme}://{Request.Host}";
        var staticUrls = new[]
        {
            new { loc = baseUrl, priority = "1.0", changefreq = "daily" },
            new { loc = $"{baseUrl}/blogs", priority = "0.6", changefreq = "yearly" },
            //new { loc = "https://taxinamthang.com/gioi-thieu", priority = "0.8", changefreq = "monthly" },
        };

        var sb = new StringBuilder();
        sb.AppendLine(@"<?xml version=""1.0"" encoding=""utf-8""?>");
        sb.AppendLine(@"<urlset xmlns=""http://www.sitemaps.org/schemas/sitemap/0.9"">");

        foreach (var url in staticUrls)
        {
            sb.AppendLine("  <url>");
            sb.AppendLine($"    <loc>{url.loc}</loc>");
            sb.AppendLine($"    <lastmod>{now}</lastmod>");
            sb.AppendLine($"    <changefreq>{url.changefreq}</changefreq>");
            sb.AppendLine($"    <priority>{url.priority}</priority>");
            sb.AppendLine("  </url>");
        }

        // Khi nào có các bài viết mới sử dụng để sinh ra slug id bài viết tự động, hiện tại chưa có nên không dùng
        /*
         * foreach (var post in _blogService.GetAll())
        {
            sb.AppendLine("  <url>");
            sb.AppendLine($"    <loc>{baseUrl}/blogs/{post.Slug}</loc>");
            sb.AppendLine($"    <lastmod>{post.LastUpdated:yyyy-MM-dd}</lastmod>");
            sb.AppendLine("    <changefreq>monthly</changefreq>");
            sb.AppendLine("    <priority>0.8</priority>");
            sb.AppendLine("  </url>");
        }
        */

        sb.AppendLine("</urlset>");
        return Content(sb.ToString(), "application/xml", Encoding.UTF8);
    }
}

// Sở đồ Video cho Google. Đọc nội dung file json để lấy thông tin video chạy SEO
[AllowAnonymous]
[Route("video-sitemap.xml")]
[ApiController]
public class VideoSitemapController : ControllerBase
{
    private readonly ILogger<VideoSitemapController> _logger;
    public VideoSitemapController(ILogger<VideoSitemapController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    [ResponseCache(Duration = 86400)]
    public IActionResult Index()
    {
        var baseUrl = $"{Request.Scheme}://{Request.Host}";
        var videoDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "videos");
        var sb = new StringBuilder();

        sb.AppendLine(@"<?xml version=""1.0"" encoding=""utf-8""?>");
        sb.AppendLine(@"<urlset xmlns=""http://www.sitemaps.org/schemas/sitemap/0.9"" xmlns:video=""http://www.google.com/schemas/sitemap-video/1.1"">");

        if (!Directory.Exists(videoDir))
        {
            _logger.LogWarning("Video folder not found: {dir}", videoDir);
        }
        else
        {
            foreach (var videoFile in Directory.GetFiles(videoDir, "*.mp4"))
            {
                var fileInfo = new FileInfo(videoFile);
                var fileName = Path.GetFileNameWithoutExtension(videoFile);
                var metaPath = Path.Combine(videoDir, $"{fileName}.json");

                // --- default metadata
                var meta = new VideoMetaSeo
                {
                    Title = fileName.Replace('-', ' '),
                    Description = fileName.Replace('-', ' '),
                    Thumbnail = $"/thumbnails/{fileName}.jpg",
                    Duration = 90,
                    Tags = new[] { "Taxi Nam Thắng", "Xe điện Nam Thắng" },
                    Category = "Transportation",
                    Uploader = "Taxi Nam Thắng",
                    View_Count = 0,
                    Publication_Date = fileInfo.LastWriteTimeUtc
                };

                // --- đọc file JSON (ưu tiên .json, .json.br, .json.gz)
                string? json = null;
                try
                {
                    if (System.IO.File.Exists(metaPath))
                    {
                        json = System.IO.File.ReadAllText(metaPath, Encoding.UTF8);
                    }
                    else if (System.IO.File.Exists(metaPath + ".br"))
                    {
                        using var fs = System.IO.File.OpenRead(metaPath + ".br");
                        using var brotli = new System.IO.Compression.BrotliStream(fs, System.IO.Compression.CompressionMode.Decompress);
                        using var reader = new StreamReader(brotli);
                        json = reader.ReadToEnd();
                    }
                    else if (System.IO.File.Exists(metaPath + ".gz"))
                    {
                        using var fs = System.IO.File.OpenRead(metaPath + ".gz");
                        using var gzip = new System.IO.Compression.GZipStream(fs, System.IO.Compression.CompressionMode.Decompress);
                        using var reader = new StreamReader(gzip);
                        json = reader.ReadToEnd();
                    }

                    if (!string.IsNullOrWhiteSpace(json))
                    {
                        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                        var customMeta = JsonSerializer.Deserialize<VideoMetaSeo>(json, options);
                        if (customMeta != null)
                        {
                            if (customMeta.Publication_Date == DateTime.MinValue)
                                customMeta.Publication_Date = fileInfo.LastWriteTimeUtc;

                            meta = customMeta;
                            _logger.LogInformation("Loaded meta for {file}: {title}", fileName, meta.Title);
                        }
                    }
                    else
                    {
                        _logger.LogWarning("No metadata file found for {file}", fileName);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error reading meta JSON for {file}", fileName);
                }

                // --- thumbnail absolute URL
                var thumbUrl = meta.Thumbnail?.Trim() ?? string.Empty;
                if (!Uri.IsWellFormedUriString(thumbUrl, UriKind.Absolute))
                {
                    if (!thumbUrl.StartsWith("/"))
                        thumbUrl = "/" + thumbUrl;
                    thumbUrl = baseUrl.TrimEnd('/') + thumbUrl;
                }

                // --- sitemap entry
                var videoUrl = $"{baseUrl}/videos/{fileInfo.Name}";
                var lastMod = fileInfo.LastWriteTimeUtc.ToString("yyyy-MM-ddTHH:mm:ssZ");
                var pubDate = meta.Publication_Date == DateTime.MinValue
                    ? fileInfo.LastWriteTimeUtc
                    : meta.Publication_Date;

                sb.AppendLine("  <url>");
                sb.AppendLine($"    <loc>{Escape(meta.locationVideoShow)}</loc>");
                sb.AppendLine($"    <lastmod>{lastMod}</lastmod>");
                sb.AppendLine("    <video:video>");
                sb.AppendLine($"      <video:thumbnail_loc>{Escape(thumbUrl)}</video:thumbnail_loc>");
                sb.AppendLine($"      <video:title>{Escape(meta.Title)}</video:title>");
                sb.AppendLine($"      <video:description>{Escape(meta.Description)}</video:description>");
                sb.AppendLine($"      <video:content_loc>{Escape(videoUrl)}</video:content_loc>");
                sb.AppendLine($"      <video:player_loc>{Escape($"{baseUrl}/videos/{fileName}")}</video:player_loc>");
                sb.AppendLine($"      <video:duration>{meta.Duration}</video:duration>");
                sb.AppendLine($"      <video:publication_date>{pubDate:yyyy-MM-ddTHH:mm:ssZ}</video:publication_date>");
                sb.AppendLine("      <video:family_friendly>yes</video:family_friendly>");

                if (meta.Tags != null)
                {
                    foreach (var tag in meta.Tags)
                        sb.AppendLine($"      <video:tag>{Escape(tag)}</video:tag>");
                }

                sb.AppendLine($"      <video:category>{Escape(meta.Category ?? "Transportation")}</video:category>");
                sb.AppendLine($"      <video:uploader>{Escape(meta.Uploader ?? "Taxi Nam Thắng")}</video:uploader>");
                sb.AppendLine($"      <video:view_count>{meta.View_Count}</video:view_count>");
                sb.AppendLine("    </video:video>");
                sb.AppendLine("  </url>");
            }
        }

        sb.AppendLine("</urlset>");
        return Content(sb.ToString(), "application/xml", Encoding.UTF8);
    }

    /** System.Security.SecurityElement.Escape sẽ thay các ký tự nhạy cảm bằng entity tương ứng trong XML
        & → &amp;
        < → &lt;
        > → &gt;
        " → &quot;
        ' → &apos;
    */
    private static string Escape(string? text)
        => System.Security.SecurityElement.Escape(text ?? string.Empty);
}

// file quản cáo ads.txt cho Google: google.com, pub-4772558027240339, DIRECT, f08c47fec0942fa0
[AllowAnonymous]
[Route("ads.txt")]
[ApiController]
public class AdsController : ControllerBase
{
    [HttpGet]
    [ResponseCache(Duration = 86400)]
    public IActionResult Index()
    {
        const string content = "google.com, pub-4772558027240339, DIRECT, f08c47fec0942fa0";
        return Content(content, "text/plain; charset=utf-8");
    }
}

// file xác thực googlef870cd40329510dd.html cho Google: google-site-verification: googlef870cd40329510dd.html
[AllowAnonymous]
[Route("googlef870cd40329510dd.html")]
[ApiController]
public class GoogleVerificationController : ControllerBase
{
    [HttpGet]
    [ResponseCache(Duration = 86400)]
    public IActionResult Verify()
    {
        const string content = "google-site-verification: googlef870cd40329510dd.html";
        return Content(content, "text/plain; charset=utf-8");
    }
}
