using Chinook.Services.Interfaces;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace Chinook.Services
{
    public class UserService : IUserService
    {
        /// <summary>
        /// Authenticatio nState Provider
        /// </summary>
        private readonly AuthenticationStateProvider _authenticationStateProvider;
        /// <summary>
        /// Logger
        /// </summary>
        private readonly ILogger<PlaylistPageService> _logger;

        public UserService(
            AuthenticationStateProvider authenticationStateProvider, 
            ILogger<PlaylistPageService> logger
        )
        {
            _authenticationStateProvider = authenticationStateProvider;
            _logger = logger;
        }

        /// <summary>
        /// Get user id.
        /// </summary>
        /// <returns>User id.</returns>
        public async Task<string> GetUserId()
        {
            try
            {
                var authenticationState = await _authenticationStateProvider.GetAuthenticationStateAsync();
                var user = authenticationState.User;

                if (!user.Identity.IsAuthenticated)
                {
                    return null;
                }

                var userIdClaim = user.FindFirst(u => u.Type.Contains(ClaimTypes.NameIdentifier));

                if (userIdClaim != null)
                {
                    return userIdClaim.Value;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting the user ID.");
                throw;
            }
        }
    }
}
