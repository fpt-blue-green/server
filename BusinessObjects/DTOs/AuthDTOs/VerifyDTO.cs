using static BusinessObjects.Enum.AuthEnumContainer;

namespace BusinessObjects.DTOs.AuthDTOs
{
    public class VerifyDTO
    {
        public string Token { get; set; }
        public EAuthAction Action { get; set; }
    }
}
