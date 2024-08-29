namespace AdFusionAPI.APIConfig
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class NoAuthRequiredAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public class AdminRequiredAttribute : Attribute
    {
    }
}
