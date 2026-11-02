using System.Security.Claims;

namespace ElearningApi.Authz;

public sealed class AuthzOptions
{
    /// <summary>"rbac" | "openfga" | "dual". La fonte di verita' della decisione durante la migrazione.</summary>
    public string SourceOfTruth { get; set; } = "rbac";
}

/// <summary>
/// Il pattern della migrazione: durante la migrazione esegue SIA il check RBAC SIA quello OpenFGA,
/// logga i disallineamenti (rete di sicurezza), e ritorna la fonte di verita' configurata.
/// Flusso: si parte con SourceOfTruth=rbac (comportamento invariato + log dei mismatch),
/// quando i log sono puliti si passa a openfga e si rimuove RBAC. Feature per feature.
/// </summary>
public sealed class DualCheckAuthorizer : IAuthorizer
{
    private readonly RbacAuthorizer _rbac;
    private readonly OpenFgaAuthorizer _fga;
    private readonly AuthzOptions _opts;
    private readonly ILogger<DualCheckAuthorizer> _log;

    public DualCheckAuthorizer(
        RbacAuthorizer rbac, OpenFgaAuthorizer fga, AuthzOptions opts, ILogger<DualCheckAuthorizer> log)
    {
        _rbac = rbac;
        _fga = fga;
        _opts = opts;
        _log = log;
    }

    public async Task<bool> AllowedAsync(ClaimsPrincipal user, string relation, string @object)
    {
        var rbac = await _rbac.AllowedAsync(user, relation, @object);
        var fga = await _fga.AllowedAsync(user, relation, @object);

        if (rbac != fga)
        {
            _log.LogWarning(
                "dual-check mismatch: {User} {Relation} {Object} -> rbac={Rbac} openfga={Fga}",
                UserRef.Of(user), relation, @object, rbac, fga);
        }

        return _opts.SourceOfTruth switch
        {
            "openfga" => fga,
            "dual" => rbac && fga,   // per confronto, non usato in migrazione
            _ => rbac,               // "rbac": fonte di verita' finche' i log non sono puliti
        };
    }
}
