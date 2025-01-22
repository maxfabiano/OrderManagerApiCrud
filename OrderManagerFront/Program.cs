using MudBlazor.Services;
using OrderManagerFront.Components;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages(); // Resolve o erro relacionado a Razor Pages
builder.Services.AddServerSideBlazor(); // Adiciona suporte a Blazor Server
builder.Services.AddMudServices(); // Adiciona serviços do MudBlazor

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddHttpClient(); 
builder.Services.AddAntiforgery(options =>
{
    options.HeaderName = "X-CSRF-TOKEN";
} );
var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.UseStaticFiles();
app.UseAntiforgery();
app.MapBlazorHub(); // Mapeia o hub do Blazor Server
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();


