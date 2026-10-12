namespace UnitConversionApi.Services;
public interface IConversionService
{
    double Convert(
        double value,
        string fromUnit,
        string toUnit);
}

namespace UnitConversionApi.Services;
public class ConversionService : IConversionService
{
    private readonly Dictionary<string, string> _unitCategories =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ["meters"] = "length",
            ["kilometers"] = "length",
            ["centimeters"] = "length",
            ["millimeters"] = "length",
            ["feet"] = "length",
            ["inches"] = "length",
            ["miles"] = "length",
            ["celsius"] = "temperature",
            ["fahrenheit"] = "temperature",
            ["kelvin"] = "temperature",
            ["kilograms"] = "weight",
            ["grams"] = "weight",
            ["milligrams"] = "weight",
            ["pounds"] = "weight",
            ["ounces"] = "weight"
        };
    private readonly Dictionary<string, double> _lengthFactors =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ["meters"] = 1.0,
            ["kilometers"] = 1000.0,
            ["centimeters"] = 0.01,
            ["millimeters"] = 0.001,
            ["feet"] = 0.3048,
            ["inches"] = 0.0254,
            ["miles"] = 1609.344
        };
    private readonly Dictionary<string, double> _weightFactors =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ["kilograms"] = 1.0,
            ["grams"] = 0.001,
            ["milligrams"] = 0.000001,
            ["pounds"] = 0.45359237,
            ["ounces"] = 0.028349523125
        };
    public double Convert(
        double value,
        string fromUnit,
        string toUnit)
    {
        fromUnit = NormalizeUnit(fromUnit);
        toUnit = NormalizeUnit(toUnit);
        if (!_unitCategories.TryGetValue(
                fromUnit, out string? fromCategory))
        {
            throw new ArgumentException(
                $"Unsupported source unit: {fromUnit}");
        }
        if (!_unitCategories.TryGetValue(
                toUnit, out string? toCategory))
        {
            throw new ArgumentException(
                $"Unsupported target unit: {toUnit}");
        }
        if (fromCategory != toCategory)
        {
            throw new ArgumentException(
                "Cannot convert between different unit categories.");
        }
        double result;
        switch (fromCategory)
        {
            case "length":
                result = ConvertLength(value, fromUnit, toUnit);
                break;
            case "weight":
                result = ConvertWeight(value, fromUnit, toUnit);
                break;
            case "temperature":
                result = ConvertTemperature(value, fromUnit, toUnit);
                break;
            default:
                throw new ArgumentException(
                    "Unsupported conversion category.");
        }
        if (!double.IsFinite(result))
        {
            throw new ArgumentException(
                "The converted value is outside the supported range.");
        }
        return Math.Round(result, 6);
    }
    private double ConvertLength(
        double value,
        string fromUnit,
        string toUnit)
    {
        double meters = value * _lengthFactors[fromUnit];
        return meters / _lengthFactors[toUnit];
    }
    private double ConvertWeight(
        double value,
        string fromUnit,
        string toUnit)
    {
        double kilograms = value * _weightFactors[fromUnit];
        return kilograms / _weightFactors[toUnit];
    }
    private double ConvertTemperature(
        double value,
        string fromUnit,
        string toUnit)
    {
        double celsius = fromUnit switch
        {
            "celsius" => value,
            "fahrenheit" => (value - 32) * 5 / 9,
            "kelvin" => value - 273.15,
            _ => throw new ArgumentException(
                "Unsupported temperature unit.")
        };
        return toUnit switch
        {
            "celsius" => celsius,
            "fahrenheit" => celsius * 9 / 5 + 32,
            "kelvin" => celsius + 273.15,
            _ => throw new ArgumentException(
                "Unsupported temperature unit.")
        };
    }
    private static string NormalizeUnit(string unit)
    {
        return unit.Trim().ToLowerInvariant() switch
        {
            "m" => "meters",
            "meter" => "meters",
            "km" => "kilometers",
            "kilometer" => "kilometers",
            "cm" => "centimeters",
            "centimeter" => "centimeters",
            "mm" => "millimeters",
            "millimeter" => "millimeters",
            "ft" => "feet",
            "foot" => "feet",
            "in" => "inches",
            "inch" => "inches",
            "mi" => "miles",
            "mile" => "miles",
            "c" or "°c" => "celsius",
            "f" or "°f" => "fahrenheit",
            "k" => "kelvin",
            "kg" or "kilogram" => "kilograms",
            "g" or "gram" => "grams",
            "mg" => "milligrams",
            "lb" or "lbs" or "pound" => "pounds",
            "oz" or "ounce" => "ounces",
            var normalized => normalized
        };
    }
}
