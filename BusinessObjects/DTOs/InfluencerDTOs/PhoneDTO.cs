namespace BusinessObjects
{
    public class VerifyPhoneDTO
    {
        public string Phone { get; set; }
        public string OTP { get; set; }
    }

    public class SendPhoneDTO
    {
        public string Phone { get; set; }
    }
}
