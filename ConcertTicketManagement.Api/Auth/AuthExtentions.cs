namespace ConcertTicketManagement.Api.Auth
{
    public static class IdentityExtensions
    {
        public static Guid GetUserId(this HttpContext context)
        {
            var userId = context.User.Claims.SingleOrDefault(x => x.Type == "userid");

            if (Guid.TryParse(userId?.Value, out var parsedId))
            {
                return parsedId;
            }

            // Hardcoding for now, should be replaced with proper user Id retrieval logic
            return Guid.Parse("ad3356e4-57df-4ca3-b102-0f07c50d14af");
        }
    }
}
