using DataTransferObject.Helpers;

namespace Agif_V2.Middleware
{
    public class SessionCheckMiddleware
    {
        private readonly RequestDelegate _next;

        public SessionCheckMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Retrieve SessionUserDTO from the session
            var sessionUser = Helpers.SessionExtensions.GetObject<SessionUserDTO>(context.Session, "User");

            // Check if sessionUser is null (i.e., user is not logged in)
            if (sessionUser == null)
            {
                // Redirect to login page if the user is not logged in
                context.Response.Redirect("/Account/FinalLogout"); // Adjust the path to your login page
                return; // Stop further processing of the request
            }

            // If session exists, continue to the next middleware or request
            await _next(context);
        }
    }

}
