namespace Collector.AspnetCore.Proxy.OAuth
{
    internal class LoggedInUser
    {
        public LoggedInUser(string countryCode, string customerNumber, string name)
        {
            CountryCode = countryCode;
            CustomerNumber = customerNumber;
            Name = name;
        }

        public string CountryCode { get; }
        public string CustomerNumber { get; }
        public string Name { get; }
    }
}