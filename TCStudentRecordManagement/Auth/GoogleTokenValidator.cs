using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Google.Apis.Auth;
using Microsoft.IdentityModel.Tokens;
using TCStudentRecordManagement.Utils;
using TCStudentRecordManagement.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Diagnostics.Eventing.Reader;

namespace TCStudentRecordManagement.Auth
{
    public class GoogleTokenValidator : ISecurityTokenValidator
    {
        // Token Handler designed for validating JWT tokens
        private readonly JwtSecurityTokenHandler _tokenHandler;
        public GoogleTokenValidator()
        {
            _tokenHandler = new JwtSecurityTokenHandler();
        }

        // JWTSecurityTokenHandler can always validate a security token
        // Ref: https://docs.microsoft.com/en-us/dotnet/api/system.identitymodel.tokens.jwt.jwtsecuritytokenhandler.canvalidatetoken?view=azure-dotnet
        bool ISecurityTokenValidator.CanValidateToken => true;

        // Initially equal to MaximumTokenSizeInBytes, but allowed to vary
        int ISecurityTokenValidator.MaximumTokenSizeInBytes { get; set; } = TokenValidationParameters.DefaultMaximumTokenSizeInBytes;

        bool ISecurityTokenValidator.CanReadToken(string securityToken)
        {
            return _tokenHandler.CanReadToken(securityToken);
        }

        public ClaimsPrincipal ValidateToken(string securityToken, TokenValidationParameters validationParameters, out SecurityToken validatedToken)
        {
            validatedToken = null;

            // Create a new list of claims that is to contain Google JWT payload data that will become part of the Identity
            List<Claim> claims = null;

            // Create a new payload object
            GoogleJsonWebSignature.Payload payload = null;

            // Validate token
            try
            {
                // GoogleJsonWebSignature class contains methods to validate the Google issued JWT
                // ValidationSettings() method contains definitions for default auth settings (clock deviation tolerances, hosted domain validation for GSuite
                // and Client ID verification. Nothing is overriden, so all settings are at default for now.
                GoogleJsonWebSignature.ValidationSettings validationSettings = new GoogleJsonWebSignature.ValidationSettings();

                // ValidateAsync() performs the validation of the JWT token. Token payload contains the targeted data as defined during the Auth Token definition
                // (scope of access to various Google services/APIs/personal information, etc. The payload data will be stored into a list of claims.
                payload = GoogleJsonWebSignature.ValidateAsync(securityToken, validationSettings).Result;

                // Create a list of claims containing Google JWT payload data
                claims = new List<Claim>()
                {
                    new Claim(ClaimTypes.NameIdentifier, payload.Name),
                    new Claim(ClaimTypes.Name, payload.Name),
                    new Claim(JwtRegisteredClaimNames.FamilyName, payload.FamilyName),
                    new Claim(JwtRegisteredClaimNames.GivenName, payload.GivenName),
                    new Claim(JwtRegisteredClaimNames.Website, payload.Picture),
                    new Claim(JwtRegisteredClaimNames.Email, payload.Email),
                    new Claim(JwtRegisteredClaimNames.Sub, payload.Subject),
                    new Claim(JwtRegisteredClaimNames.Iss, payload.Issuer),
                };

                // Check if the user exists in the local database.
                using (DataContext _DataContext = new DataContext())
                {
                    // Get DB record associated with the Email, if it exists.
                    User userData = _DataContext.Users.Where(x => x.Email == payload.Email).FirstOrDefault();

                    if (userData != null)
                    {
                        bool userIsStaff = userData.StaffData != null;
                        bool userIsSuperUser = userIsStaff ? userData.StaffData.SuperUser : false;
                        Logger.Msg<GoogleTokenValidator>($"[LOGIN] SUCCESS User {payload.Email} {(userIsSuperUser ? "(Super)" : userIsStaff ? "(Staff)" : string.Empty)}");
                    }
                    else
                    {
                        throw new SecurityTokenValidationException($"[LOGIN] FAIL User with auth token belonging to {payload.Email} not found in database");
                    }
                }
            }
            catch (Exception ex)
            {
                // TODO fix: 
                // 1. Check for the type of exception and adapt message accordingly
                Logger.Msg<GoogleTokenValidator>(new SecurityTokenValidationException($"[LOGIN] FAIL {ex.Message}"));
                throw;
            }


            // Convert token to Identity
            try
            {
                ClaimsPrincipal principal = new ClaimsPrincipal();
                principal.AddIdentity(new ClaimsIdentity(claims, "Google"));
                return principal;
            }
            catch (Exception ex)
            {
                Logger.Msg(ex);
                throw new SecurityTokenValidationException($"[LOGIN] FAIL Unable to authenticate user with auth token belonging to {payload.Email}");
            }

        }
    }

}
