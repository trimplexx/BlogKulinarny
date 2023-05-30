namespace BlogKulinarny.Data.Helpers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text;

public class AuthorizeRankFilter : ActionFilterAttribute
{
    private readonly int _minimumRequiredRank;
    private readonly AppDbContext _dbContext;

    public AuthorizeRankFilter(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public AuthorizeRankFilter(int minimumRequiredRank, AppDbContext dbContext)
    {
        _minimumRequiredRank = minimumRequiredRank;
        _dbContext = dbContext;
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        int? userRank = null;
        if (context.HttpContext.Session.TryGetValue("UserId", out byte[] userIdBytes)
            && int.TryParse(Encoding.UTF8.GetString(userIdBytes), out int userId))
        {
            var user = _dbContext.users.FirstOrDefault(u => u.Id == userId);
            if (user != null)
            {
                userRank = (int?)user.rank;
            }
        }

        if (userRank.HasValue && userRank.Value >= _minimumRequiredRank)
        {
            base.OnActionExecuting(context);
        }
        else
        {
            context.Result = new RedirectToActionResult("NoAccess", "Home", null);
        }
    }
}