﻿namespace BusinessObjects  
{
    public static class AuthEnumContainer
    {
        public enum EAuthAction
        {
            Register = 1,
            ChangePass = 2,
            ForgotPassword = 3
        }

        public enum ERole
        {
            Admin = 1,
            Influencer = 2,
            Brand = 3
        }

        public enum EAccountProvider
        {
            AdFusionAccount = 1,
            Google,
            Facebook
        }
    }
}
