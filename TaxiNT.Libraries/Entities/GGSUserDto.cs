namespace TaxiNT.Libraries.Entities;

public class GGSUserLoginDto
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public bool IsRemembered { get; set; } = false; // "true" or "false"
}
