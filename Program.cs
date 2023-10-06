using Microsoft.AspNetCore.Hosting.StaticWebAssets;
using Microsoft.AspNetCore.Localization;
using MudBlazor.Services;
using Pokefeet2.Class;

var builder = WebApplication.CreateBuilder(args);

StaticWebAssetsLoader.UseStaticWebAssets(builder.Environment, builder.Configuration);

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddMudServices();
builder.Services.AddHttpClient();
builder.Services.AddScoped<PkmnFetch>();
builder.Services.AddLocalization(options => options.ResourcesPath = "Ressources");

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Error");
	app.UseHsts();
}

string[]? supportedCultures = { "en", "fr" };
var localizationOptions = new RequestLocalizationOptions()
	.AddSupportedCultures(supportedCultures)
	.AddSupportedUICultures(supportedCultures);
localizationOptions.ApplyCurrentCultureToResponseHeaders = true;
localizationOptions.RequestCultureProviders = new List<IRequestCultureProvider>
{
	new QueryStringRequestCultureProvider(),
	new CookieRequestCultureProvider(),
	new AcceptLanguageHeaderRequestCultureProvider(),
};

app.UseRequestLocalization(localizationOptions);

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();