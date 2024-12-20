using GeneticAlgorithmWebApp2;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

//app.MapGet("/", () => "Hello World!");

app.UseStaticFiles();

app.MapPut("/experiments", (ExperimentParameters parameters) =>
{
    lock (ServerData.Lock)
    {
        var result = "json here";
        return Results.Ok(result);
    }
});

app.MapPost("/experiments", (Guid id) =>
{
    lock (ServerData.Lock)
    {
        var result = "json here";
        return Results.Ok(result);
    }
});

app.MapFallbackToFile("clientpage.html");

app.Run();
public record ExperimentParameters(
    int NodesAmount,
    int Epochs,
    int PopulationSize,
    double MutationProbability,
    double CrossoverProbability,
    double SurvivorsPart
);