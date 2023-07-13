using System.Net;
using System.Net.Sockets;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();

builder.Services.Configure<ForwardedHeadersOptions>(option => {
option.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
});

var app = builder.Build();
app.UseForwardedHeaders();
// Configure the HTTP request pipeline.
// if (!app.Environment.IsDevelopment())
// {
//     app.UseExceptionHandler("/Home/Error");
//     // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
//     app.UseHsts();
// }

// app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();


 
app.UseEndpoints(endpoints =>
{
    app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
    
    endpoints.MapGet("/log", async context =>
    {
        context.Response.ContentType = "text/plain";

        // Host info
        var name = Dns.GetHostName(); // get container id
        var ip = Dns.GetHostEntry(name).AddressList.FirstOrDefault(x => x.AddressFamily == AddressFamily.InterNetwork);
        Console.WriteLine($"Host Name: { Environment.MachineName} \t {name}\t {ip}");
        await context.Response.WriteAsync($"Host Name: {Environment.MachineName}{Environment.NewLine}");
        await context.Response.WriteAsync(Environment.NewLine);

        // Request method, scheme, and path
        await context.Response.WriteAsync($"Request Method: {context.Request.Method}{Environment.NewLine}");
        await context.Response.WriteAsync($"Request Scheme: {context.Request.Scheme}{Environment.NewLine}");
        await context.Response.WriteAsync($"Request URL: {context.Request.GetDisplayUrl()}{Environment.NewLine}");
        await context.Response.WriteAsync($"Request Path: {context.Request.Path}{Environment.NewLine}");

        // Headers
        await context.Response.WriteAsync($"Request Headers:{Environment.NewLine}");
        foreach (var (key, value) in context.Request.Headers)
        {
            await context.Response.WriteAsync($"\t {key}: {value}{Environment.NewLine}");
        }
        await context.Response.WriteAsync(Environment.NewLine);

        // Connection: RemoteIp
        await context.Response.WriteAsync($"Request Remote IP: {context.Connection.RemoteIpAddress}");
    });
});
app.Run();