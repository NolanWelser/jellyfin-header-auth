using System.Net.Mime;
using System.Threading.Tasks;
using Jellyfin.Data.Entities;
using Jellyfin.Data.Enums;
using MediaBrowser.Common;
using MediaBrowser.Controller.Authentication;
using MediaBrowser.Controller.Library;
using MediaBrowser.Controller.Session;
using MediaBrowser.Model.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.HeaderAuth;

public class HeaderAuthenticationProviderPlugin : IAuthenticationProvider
    {
        private readonly ILogger<HeaderAuthenticationProviderPlugin> _logger;
        private readonly IApplicationHost _applicationHost;
        private readonly IUserManager _userManager;
        private readonly ISessionManager _sessionManager;
        public HeaderAuthenticationProviderPlugin(IApplicationHost applicationHost, ILogger<HeaderAuthenticationProviderPlugin> logger)
        {
            _logger = logger;
            _applicationHost = applicationHost;
        }

        public string Name => "Header-Authentication";

        public bool IsEnabled => true;

        public async Task<ProviderAuthenticationResult> Authenticate(string username, string password)
        {
            if (Request.Headers.TryGetValue(HeaderPlugin.Instance.Configuration.LoginHeader, out var headerUsername))
            {
                User user = null;
                user = _userManager.GetUserByName(headerUsername);

                if (user == null)
                {
                    _logger.LogInformation("Header user doesn't exist, creating...");
                    user = await _userManager.CreateUserAsync(headerUsername).ConfigureAwait(false);
                    user.SetPermission(PermissionKind.IsAdministrator, false);
                    user.SetPermission(PermissionKind.EnableAllFolders, true);
                }

                user.AuthenticationProviderId = GetType().FullName;
                await _userManager.UpdateUserAsync(user).ConfigureAwait(false);

                var authRequest = new AuthenticationRequest();
                authRequest.UserId = user.Id;
                authRequest.Username = user.Username;
                _logger.LogInformation("Auth request created...");

                return await _sessionManager.AuthenticateNewSession(authRequest).ConfigureAwait(false);
            }
            return BadRequest("Something went wrong");
        }

        public bool HasPassword(User user)
        {
            return false;
        }

        public Task ChangePassword(User user, string newPassword)
        {
            return Task.CompletedTask;
        }
    }