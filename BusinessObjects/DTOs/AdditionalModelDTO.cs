namespace BusinessObjects
{
    public class CityResult
    {
        public string Name { get; set; }
        public string Country { get; set; }
    }

    public class JobResult
    {
        public int Total { get; set; } = 0;
        public int Success { get; set; } = 0;
        public int Failure { get; set; } = 0;
    }
}
