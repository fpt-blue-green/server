﻿using static BusinessObjects.AuthEnumContainer;

namespace BusinessObjects
{
    public class UserTokenDTO
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Image { get; set; }
        public ERole Role { get; set; }
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
    }

    public class UserDTO
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Image { get; set; }
        public ERole Role { get; set; }
    }

    public class UserUpdatePasswordDTO
    {
        public Guid Id { get; set; }
        public string NewPassword {  get; set; }
    }


}
