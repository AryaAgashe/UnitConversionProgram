using Microsoft.AspNetCore.Mvc;
using UnitConversionApi.Models;
using UnitConversionApi.Services;
namespace UnitConversionApi.Controllers;
[ApiController]
[Route("api/[controller]")]
public class ConversionController : ControllerBase
{
    private readonly IConversionService _conversionService;
    public ConversionController(
        IConversionService conversionService)
    {
        _conversionService = conversionService;
    }
    [HttpPost]
    public ActionResult<ConversionResponse> Convert(
        [FromBody] ConversionRequest request)
    {
        if (!double.IsFinite(request.Value))
        {
            return BadRequest(new
            {
                error = "Value must be a finite number."
            });
        }
        if (string.IsNullOrWhiteSpace(request.FromUnit) ||
            string.IsNullOrWhiteSpace(request.ToUnit))
        {
            return BadRequest(new
            {
                error = "FromUnit and ToUnit are required."
            });
        }
        try
        {
            double convertedValue = _conversionService.Convert(
                request.Value,
                request.FromUnit,
                request.ToUnit);
            return Ok(new ConversionResponse
            {
                OriginalValue = request.Value,
                FromUnit = request.FromUnit,
                ConvertedValue = convertedValue,
                ToUnit = request.ToUnit
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                error = ex.Message
            });
        }
    }
}


using UnitConversionApi.Services;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddSingleton<
    IConversionService,
    ConversionService>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();