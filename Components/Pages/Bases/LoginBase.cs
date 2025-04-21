namespace ntchecker.Components.Pages.Bases;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using ntchecker.Data.Entities;
using ntchecker.Services.Interfaces;

public class LoginBase : ComponentBase
{
    [Inject]
    private IAuthenService authenService { get; set; }
    [Inject]
    private NavigationManager nav { get; set; }
    [Inject]
    private ISnackbar snackBar { get; set; }
    protected string token { get; set; }
    protected string ErrorMessage { get; set; }
    protected override async Task OnInitializedAsync()
    {
        try
        {
            // Check authentication state
            if (await authenService.CheckAuthenState()) nav.NavigateTo("/", true);
            await base.OnInitializedAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
    }

    #region Private to handler with data                             
    // Login
    private async Task LoginHandler(LoginUserDto _models)
    {
        try
        {
            await authenService.Login(_models);
            snackBar.Add($"Đăng nhập thành công.", Severity.Success);

            // Dừng 3s sau khi chuyển hướng
            Thread.Sleep(TimeSpan.FromSeconds(3));

            // Chuyển về trang chủ
            nav.NavigateTo("/", true);
        }
        catch (Exception ex)
        {
            await ClearEditForm();
            snackBar.Add(ex.Message, Severity.Error);
        }
    }
    #endregion

    #region EditFrom to login
    protected LoginUserDto models = new LoginUserDto();
    protected bool _processing = false;
    protected string textResult;

    // Submit
    protected async void OnValidSubmit(EditContext editContext)
    {
        _processing = true;
        // Do something
        await LoginHandler(models);
        StateHasChanged();
    }

    // Clean models
    protected async Task ClearEditForm()
    {
        models = new LoginUserDto();
        _processing = false;
        StateHasChanged();
    }
    #endregion

    #region MudTextField Password
    protected bool isShowPassword = false;
    protected InputType PasswordInput = InputType.Password;
    protected string PasswordInputIcon = Icons.Material.Filled.VisibilityOff;

    protected async Task ShowPasswordEvent()
    {
        if (isShowPassword)
        {
            isShowPassword = false;
            PasswordInputIcon = Icons.Material.Filled.VisibilityOff;
            PasswordInput = InputType.Password;
        }
        else
        {
            isShowPassword = true;
            PasswordInputIcon = Icons.Material.Filled.Visibility;
            PasswordInput = InputType.Text;
        }
    }
    #endregion
}