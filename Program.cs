using graphql_preprocessor.Controllers;
using GraphqlPreprocessor.Components;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddControllers(options =>
{
    // Support for plain text as input
    options.InputFormatters.Insert(options.InputFormatters.Count, new TextPlainInputFormatter());
});
builder.Services.AddMudServices();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseRouting();
app.UseSwagger();
app.UseSwaggerUI();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
