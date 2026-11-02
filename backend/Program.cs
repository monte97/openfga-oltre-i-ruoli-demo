using System.Security.Claims;
using System.Text;
using System.Text.Json;
using ElearningApi.Authz;
using ElearningApi.Endpoints;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Tokens;
using OpenFga.Sdk.Client;

var builder = WebApplication.CreateBuilder(args);

// --- Config ---
var openfgaUrl = builder.Configuration["OPENFGA:APIURL"] ?? "http://openfga:8080";
var storeId = builder.Configuration["OPENFGA:STOREID"]
    ?? throw new InvalidOperationException("OPENFGA__STOREID non impostato: lo scrive il servizio init.");
var sourceOfTruth = builder.Configuration["AUTHZ:SOURCEOFTRUTH"] ?? "rbac";
var devKey = builder.Configuration["JWT:DEVKEY"]
    ?? throw new InvalidOperationException("JWT__DEVKEY non impostata.");

// --- AuthN: JWT HS256 firmato con una chiave locale di sviluppo ---
// In produzione il token arriva dall'identity provider (vedi l'articolo su Keycloak e OpenFGA).
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.MapInboundClaims = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(devKey)),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
        };
    });
builder.Services.AddAuthorization();

// I ruoli stanno in realm_access.roles, come nei token di Keycloak.
// Li portiamo in claim di tipo Role cosi' User.IsInRole(...) funziona.
builder.Services.AddSingleton<IClaimsTransformation, RealmRolesClaimsTransformation>();

// --- OpenFGA client ---
builder.Services.AddSingleton(new OpenFgaClient(new ClientConfiguration
{
    ApiUrl = openfgaUrl,
    StoreId = storeId,
}));

// --- Authorizer ---
builder.Services.AddSingleton(new AuthzOptions { SourceOfTruth = sourceOfTruth });
builder.Services.AddScoped<RbacAuthorizer>();
builder.Services.AddScoped<OpenFgaAuthorizer>();
builder.Services.AddScoped<IAuthorizer, DualCheckAuthorizer>();

var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => $"elearning-api: demo RBAC->ReBAC, fonte di verita': {sourceOfTruth}");
app.MapCourses();

app.Run();


/// <summary>Estrae i ruoli da realm_access.roles e li aggiunge come claim Role.</summary>
public sealed class RealmRolesClaimsTransformation : IClaimsTransformation
{
    public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        if (principal.Identity is not ClaimsIdentity identity)
            return Task.FromResult(principal);

        var json = principal.FindFirst("realm_access")?.Value;
        if (string.IsNullOrEmpty(json)) return Task.FromResult(principal);

        try
        {
            using var doc = JsonDocument.Parse(json);
            if (doc.RootElement.TryGetProperty("roles", out var roles))
                foreach (var r in roles.EnumerateArray())
                    identity.AddClaim(new Claim(ClaimTypes.Role, r.GetString()!));
        }
        catch (JsonException) { /* token malformato: nessun ruolo */ }

        return Task.FromResult(principal);
    }
}
