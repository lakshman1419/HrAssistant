using HrAssistant.Agents;
using HrAssistant.Services;
using HrAssistant.Tools;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AngularDevelopment", policy =>
    {
        policy.WithOrigins("http://localhost:56675")
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

    var text = pdfService.ExtractText("Documents/Leave-Policy.pdf");
    var chunks = chunkService.Split(text);

    await vectorStore.AddDocumentAsync(chunks);
}

app.UseHttpsRedirection();

app.UseCors("AngularDevelopment");

app.UseAuthorization();

app.MapControllers();


app.Run();