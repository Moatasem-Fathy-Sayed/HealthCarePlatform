namespace Healthcare.SharedKernel.Common;

// Represents a standardized application error with a code and description
public record Error(string Code, string Description)
{
    // Empty error constant for successful results
    public static readonly Error None = new(string.Empty, string.Empty);

    // Common error for null parameter values
    public static readonly Error NullValue = new("Error.NullValue", "A null value was provided.");
}