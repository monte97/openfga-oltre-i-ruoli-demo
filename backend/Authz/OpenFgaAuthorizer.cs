using System.Security.Claims;
using OpenFga.Sdk.Client;
using OpenFga.Sdk.Client.Model;

namespace ElearningApi.Authz;

/// <summary>
/// Il modo "nuovo": decide interrogando OpenFGA sulla relazione corrente.
/// Precisa sulla singola risorsa (questo utente, questo corso) e contestuale:
/// passa current_time cosi' l'assistente a scadenza (condition non_expired) e' valutato.
/// </summary>
public sealed class OpenFgaAuthorizer : IAuthorizer
{
    private readonly OpenFgaClient _fga;

    public OpenFgaAuthorizer(OpenFgaClient fga) => _fga = fga;

    public async Task<bool> AllowedAsync(ClaimsPrincipal user, string relation, string @object)
    {
        var response = await _fga.Check(new ClientCheckRequest
        {
            User = UserRef.Of(user),
            Relation = relation,
            Object = @object,
            Context = new { current_time = DateTime.UtcNow.ToString("o") },
        });
        return response.Allowed ?? false;
    }
}
