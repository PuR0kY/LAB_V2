namespace LAB_V2.Jobs
{
    public class FromToJob
    {
        public string Name { get; set; }
        public string From { get; set; }
        public string To { get; set; }

        public static FromToJob CreateNew()
        {
            return new FromToJob()
            {
                Name = "New Job",
                From = "",
                To = ""
            };
        }
    }
}
