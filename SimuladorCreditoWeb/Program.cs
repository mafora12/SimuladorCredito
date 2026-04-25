using SimuladorCredito.Services;

using SimuladorCredito.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddSingleton<CreditoFactory>();
builder.Services.AddSingleton<SimuladorFacade>();

builder.WebHost.UseUrls("http://0.0.0.0:10000");

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}
app.UseDeveloperExceptionPage();
app.UseHttpsRedirection();

app.UseStaticFiles(); 

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages(); 

app.Run();