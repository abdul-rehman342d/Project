using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Web;
using Common;
using LibNeeo.Activation;

namespace UtilityService.Controllers
{
    [RoutePrefix("Authentication")]
    public class AuthenticationController : ApiController
    {
        public class tokenModel
        {
            public string uID { get; set; }
            public string key { get; set; }
            public string token { get; set; }
        }

        [Route("createToken")]
        [HttpPost]
        public HttpResponseMessage UserLogin([FromBody]tokenModel currentModel)
        {
            string token="";
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
            if (NeeoUtility.IsPhoneNumberInInternationalFormat(currentModel.uID))
            {
                NeeoUtility.SetServiceResponseHeaders(CustomHttpStatusCode.InvalidNumber);

            }
            if (!NeeoUtility.ValidatePhoneNumber(NeeoUtility.FormatAsIntlPhoneNumber(currentModel.uID)))
            {
                NeeoUtility.SetServiceResponseHeaders(CustomHttpStatusCode.InvalidNumber);
                
            }
            if (NeeoActivation.CheckUserAlreadyRegistered(currentModel.uID))
            {
                token = GenerateToken(currentModel.uID);
                return Request.CreateResponse(HttpStatusCode.OK, token);
            }
            else
            {
                token = "-1";
                return Request.CreateResponse(HttpStatusCode.Unauthorized, token);

            }
        }

        [Route("ValidateToken")]
        [HttpPost]
        public HttpResponseMessage ValidateToken([FromBody]tokenModel currentModel)
        {
            string username = null;
            ClaimsPrincipal principal = GetPrincipal(currentModel.token);
            if (principal == null)
                return null;
            ClaimsIdentity identity = null;
            try
            {
                identity = (ClaimsIdentity)principal.Identity;
            }
            catch (NullReferenceException)
            {
                return null;
            }
            Claim usernameClaim = identity.FindFirst(ClaimTypes.Name);
            username = usernameClaim.Value;
           
            return Request.CreateResponse(HttpStatusCode.OK, username);
        }

        private static string Secret = "ERMN05OPLoDvbTTa/QihCFPSNPkwLNBTbVZHUAnYc5iRYaWz9emMXgkdP3wGcgYX7URUBcFP+oi2+bXr6CUYTR==";
        private static string GenerateToken(string username)
        {
            byte[] key = Convert.FromBase64String(Secret);
            SymmetricSecurityKey securityKey = new SymmetricSecurityKey(key);
            SecurityTokenDescriptor descriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] {
                      new Claim(ClaimTypes.Name, username)}),
                Expires = DateTime.UtcNow.AddMinutes(30),
                SigningCredentials = new SigningCredentials(securityKey,
                SecurityAlgorithms.HmacSha256Signature)
            };

            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            JwtSecurityToken token = handler.CreateJwtSecurityToken(descriptor);
            return handler.WriteToken(token);
        }
        private static ClaimsPrincipal GetPrincipal(string token)
        {
            try
            {
                JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
                JwtSecurityToken jwtToken = (JwtSecurityToken)tokenHandler.ReadToken(token);
                if (jwtToken == null)
                    return null;
                byte[] key = Convert.FromBase64String(Secret);
                TokenValidationParameters parameters = new TokenValidationParameters()
                {
                    RequireExpirationTime = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };
                SecurityToken securityToken;
                ClaimsPrincipal principal = tokenHandler.ValidateToken(token,
                      parameters, out securityToken);
                return principal;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
      
    }
}
