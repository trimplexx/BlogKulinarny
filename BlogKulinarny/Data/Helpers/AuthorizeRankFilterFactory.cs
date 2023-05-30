using Microsoft.AspNetCore.Mvc.Filters;

namespace BlogKulinarny.Data.Helpers;

public class AuthorizeRankFilterFactory : IFilterFactory
{
    private readonly int _minimumRequiredRank;

    public AuthorizeRankFilterFactory(int minimumRequiredRank)
    {
        _minimumRequiredRank = minimumRequiredRank;
    }

    public bool IsReusable => false;

    public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
    {
        var dbContext = serviceProvider.GetRequiredService<AppDbContext>();
        return new AuthorizeRankFilter(_minimumRequiredRank, dbContext);
    }
}