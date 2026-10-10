using GuestAPI.Repository;
using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace GuestAPI.Providers
{
    public class AuthProvider : OAuthAuthorizationServerProvider
    {
        public async override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.Validated();
        }

        public async override Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            using (UserRepo repo = new UserRepo())
            {
                var user = repo.ValidateUser(context.UserName, context.Password);
                if (user == null)
                {
                    context.SetError("Invalid grant", "username or password is incorrect");
                }
                var identity = new ClaimsIdentity(context.Options.AuthenticationType);

                identity.AddClaim(new Claim(ClaimTypes.Name, user.Username));
                identity.AddClaim(new Claim(ClaimTypes.Email, user.Email));

                foreach (var role in user.Roles.Split(','))
                {
                    identity.AddClaim(new Claim(ClaimTypes.Role, role.Trim()));
                }
                context.Validated(identity);
            }
        }
    }
}