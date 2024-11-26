namespace AdFusionAPI.APIConfig
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class AuthRequiredAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public class AdminRequiredAttribute : AuthRequiredAttribute
    {
    }

    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public class InfluencerRequiredAttribute : AuthRequiredAttribute
    {
    }

    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public class BrandRequiredAttribute : AuthRequiredAttribute
    {
    }
}
