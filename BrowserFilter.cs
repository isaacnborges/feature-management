using Microsoft.FeatureManagement;

namespace feature_flags
{
    [FilterAlias("Browser")]
    public class BrowserFilter : IFeatureFilter
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public BrowserFilter(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public Task<bool> EvaluateAsync(FeatureFilterEvaluationContext context)
        {
            if (_httpContextAccessor.HttpContext != null)
            {
                var userAgent = _httpContextAccessor.HttpContext.Request.Headers["User-Agent"].ToString();
                var settings = context.Parameters.Get<BrowserFilterSettings>();
                return Task.FromResult(settings.Allowed.Any(a => userAgent.Contains(a)));
            }
            return Task.FromResult(false);
        }
    }

    public class BrowserFilterSettings
    {
        public string[] Allowed { get; set; }
    }
}
