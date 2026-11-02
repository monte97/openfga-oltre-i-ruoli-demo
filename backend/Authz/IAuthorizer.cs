using System.Security.Claims;

namespace ElearningApi.Authz;

/// <summary>
/// Decide se l'utente ha una relazione (es. "viewer", "editor") su un oggetto (es. "course:42").
/// Tre implementazioni: RBAC (ruoli nel JWT), OpenFGA (ReBAC), DualCheck (entrambi, per la migrazione).
/// </summary>
public interface IAuthorizer
{
    Task<bool> AllowedAsync(ClaimsPrincipal user, string relation, string @object);
}

/// <summary>Id utente OpenFGA a partire dal token: usiamo lo username (preferred_username) per id leggibili.</summary>
public static class UserRef
{
    public static string Of(ClaimsPrincipal user)
    {
        var username = user.FindFirstValue("preferred_username") ?? user.FindFirstValue(ClaimTypes.Name);
        return $"user:{username}";
    }
}
