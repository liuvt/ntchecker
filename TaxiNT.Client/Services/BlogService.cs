using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using TaxiNT.Client.Services.Interfaces;
using TaxiNT.Libraries.Models;
using TaxiNT.Libraries.Models.GGSheets;

namespace TaxiNT.Client.Services;
public class BlogService : IBlogService
{
    private readonly HttpClient _http;

    //Constructor
    public BlogService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<Blog>> GetAllAsync()
    {
        return await _http.GetFromJsonAsync<List<Blog>>("api/blog") ?? new();
    }

    public async Task<Blog?> GetByIdAsync(int id)
    {
        return await _http.GetFromJsonAsync<Blog>($"api/blog/{id}");
    }

    public async Task<Blog?> GetBySlugAsync(string slug)
    {
        return await _http.GetFromJsonAsync<Blog>($"api/blog/get/{slug}");
    }

    public async Task<List<Blog>> GetRelatedAsync(string category, int excludeId)
    {
        return await _http.GetFromJsonAsync<List<Blog>>($"api/blog/related/{category}/{excludeId}") ?? new();
    }

    public async Task<List<Blog>> GetMostPopularAsync()
    {
        return await _http.GetFromJsonAsync<List<Blog>>("api/blog/popular") ?? new();
    }

    public async Task<Blog?> CreateAsync(Blog blog)
    {
        var response = await _http.PostAsJsonAsync("api/blog", blog);
        if (!response.IsSuccessStatusCode) return null;
        return await response.Content.ReadFromJsonAsync<Blog>();
    }

    public async Task<bool> UpdateAsync(Blog blog)
    {
        var response = await _http.PutAsJsonAsync($"api/blog/{blog.Id}", blog);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var response = await _http.DeleteAsync($"api/blog/{id}");
        return response.IsSuccessStatusCode;
    }
}
