namespace Patient.Application.Common.Interfaces;

/// <summary>
/// Service interface for accessing authenticated user identity claims.
/// </summary>
public interface ICurrentUserService
{
    Guid UserId { get; }
}