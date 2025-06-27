using Microsoft.AspNetCore.Components;

namespace SimpleTomlValidation.Layout;

public partial class MainLayout
{
    [Inject]
    public IConfiguration? Configuration { get; set; }

    private string? githubUrl;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        githubUrl = Configuration?["githubUrl"];
    }
}
