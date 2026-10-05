using HrAssistant.Agents;
using HrAssistant.Models;
using HrAssistant.Services;
using HrAssistant.Tools;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AngularDevelopment", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// Services

builder.Services.AddScoped<IEmployeeService, EmployeeService>();

builder.Services.AddScoped<ILeaveService, LeaveService>();

builder.Services.AddScoped<PdfService>();

builder.Services.AddScoped<TextChunkService>();

builder.Services.AddHttpClient<NvidiaEmbeddingService>();

builder.Services.AddScoped<VectorStoreService>();

builder.Services.AddSingleton<VectorStoreService>();


// Tools

builder.Services.AddScoped<EmployeeTools>();

builder.Services.AddScoped<LeaveTools>();

builder.Services.AddScoped<PolicyTools>();


// Tool Registry

builder.Services.AddScoped<HrToolRegistry>();


// NVIDIA Chat Client

builder.Services.AddSingleton(provider =>
{
    return NvidiaAgentFactory.Create(
        builder.Configuration);
});


// HR Agent

builder.Services.AddScoped<HrAgent>();

builder.Services.AddSingleton<IValidateOptions<AuthenticationOptions>, AuthenticationOptionsValidator>();
builder.Services.AddOptions<AuthenticationOptions>()
    .Bind(builder.Configuration.GetSection(AuthenticationOptions.SectionName))
    .ValidateOnStart();
builder.Services.AddSingleton<IUserAuthenticationService, InMemoryUserAuthenticationService>();


builder.Services.AddControllers();

builder.Services.AddOpenApi();


var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

using (var scope = app.Services.CreateScope())
{
    var pdfService = scope.ServiceProvider.GetRequiredService<PdfService>();
    var chunkService = scope.ServiceProvider.GetRequiredService<TextChunkService>();
    var vectorStore = scope.ServiceProvider.GetRequiredService<VectorStoreService>();
    var documentsDirectory = Path.Combine(app.Environment.ContentRootPath, "Documents");

    foreach (var pdfPath in Directory.EnumerateFiles(
                 documentsDirectory,
                 "*.pdf",
                 SearchOption.TopDirectoryOnly))
    {
        try
        {
            var text = pdfService.ExtractText(pdfPath);
            var chunks = chunkService.Split(text);

            if (chunks.Count == 0)
            {
                app.Logger.LogWarning("No text was extracted from policy document {DocumentPath}.", pdfPath);
                continue;
            }

            await vectorStore.AddDocumentAsync(chunks, Path.GetFileName(pdfPath));
            app.Logger.LogInformation("Indexed policy document {DocumentPath} with {ChunkCount} chunks.", pdfPath, chunks.Count);
        }
        catch (Exception exception)
        {
            app.Logger.LogError(exception, "Failed to index policy document {DocumentPath}.", pdfPath);
        }
    }
}

app.UseHttpsRedirection();

app.UseCors("AngularDevelopment");

app.UseAuthorization();

app.MapControllers();


app.Run();