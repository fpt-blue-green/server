using BusinessObjects.Models;
using System.Text.Json.Serialization;
using static BusinessObjects.Enum.AuthenEnumContainer;

namespace BusinessObjects.ModelsDTO.UserDTOs
{
    public class UserTokenDTO
    {
        public Guid Id { get; set; }
        public string? Email { get; set; }
        public ERole Role{ get; set; }
        public string? DisplayName { get; set; }
    }
}
