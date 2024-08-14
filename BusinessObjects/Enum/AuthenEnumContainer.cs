﻿namespace BusinessObjects.Enum
{
    public class AuthenEnumContainer
    {
        public enum AuthenAction
        {
            Register = 1,
            ChangePass = 2,
            ForgotPassword = 3
        }

        public enum Role
        {
            Admin = 1,
            Influencer = 2,
            Brand = 3
        }
    }
}
