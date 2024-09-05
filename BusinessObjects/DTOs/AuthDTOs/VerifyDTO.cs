using static BusinessObjects.AuthEnumContainer;

namespace BusinessObjects
{
    public class VerifyDTO
    {
        public string Token { get; set; }
        public EAuthAction Action { get; set; }
    }
}
