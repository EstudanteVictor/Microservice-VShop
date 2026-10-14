using Duende.IdentityModel;
using Duende.IdentityServer.Extensions;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using VShop.IdentityServer.Data;

namespace VShop.IdentityServer.Services;

public class ProfileAppService : IProfileService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUserClaimsPrincipalFactory<ApplicationUser> _userClaimsPrincipalFactory;
    private readonly RoleManager<IdentityRole> _roleManager;

    public ProfileAppService(UserManager<ApplicationUser> userManager,
        IUserClaimsPrincipalFactory<ApplicationUser> userClaimsPrincipalFactory,
        RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _userClaimsPrincipalFactory = userClaimsPrincipalFactory;
        _roleManager = roleManager;
    }

    public async Task GetProfileDataAsync(ProfileDataRequestContext context, CancellationToken ct)
    {
        //id do usuario no IS
        string id = context.Subject.GetSubjectId();
        //localiza usuario por id
        ApplicationUser user = await _userManager.FindByIdAsync(id);

        //cria ClaimsPrincipal para o usuario
        ClaimsPrincipal userClaims = await _userClaimsPrincipalFactory.CreateAsync(user);

        //define coleção de claims para usuario 
        //e inclui o sobrenome e o nome do usuário
        List<Claim> claims = userClaims.Claims.ToList();
        claims.Add(new Claim(JwtClaimTypes.FamilyName, user.LastName));
        claims.Add(new Claim(JwtClaimTypes.GivenName, user.FirstName));

        //se o userManager do identity suportar role
        if (_userManager.SupportsUserRole)
        {
            //obterm lista dos nomes das roles para o usuario
            IList<string> roles = await _userManager.GetRolesAsync(user);

            foreach(string role in roles)
            {
                claims.Add(new Claim(JwtClaimTypes.Role, role));
                if (_roleManager.SupportsRoleClaims)
                {
                    IdentityRole identityRole = await _roleManager.FindByNameAsync(role);
                    if(identityRole != null)
                    {
                        claims.AddRange(await _roleManager.GetClaimsAsync(identityRole));
                    }
                }
            }
            //retorna as claims no contexto
            context.IssuedClaims = claims;
        }
    }

    public async Task IsActiveAsync(IsActiveContext context, CancellationToken ct)
    {
        //obtem ID do usuario no IS
        string userId = context.Subject.GetSubjectId();
        //localiza usuario
        ApplicationUser user = await _userManager.FindByIdAsync(userId);

        //verifica se esta ativo
        context.IsActive = user is not null;
    }
}
