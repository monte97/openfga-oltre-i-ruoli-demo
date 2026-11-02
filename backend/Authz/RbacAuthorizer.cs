using System.Security.Claims;

namespace ElearningApi.Authz;

/// <summary>
/// Il modo "vecchio": decide dai ruoli nel JWT. Il limite e' qui:
/// il ruolo (author/student) non sa DI QUALE corso. Chi ha il ruolo "student"
/// viene lasciato passare su qualunque corso, chi ha "author" modifica qualunque corso.
/// </summary>
public sealed class RbacAuthorizer : IAuthorizer
{
    public Task<bool> AllowedAsync(ClaimsPrincipal user, string relation, string @object)
    {
        // Mappa grossolana relazione -> ruolo, indipendente dalla risorsa specifica.
        var allowed = relation switch
        {
            "viewer" => user.IsInRole("student") || user.IsInRole("author"),
            "editor" => user.IsInRole("author"),
            _ => false,
        };
        return Task.FromResult(allowed);
    }
}
