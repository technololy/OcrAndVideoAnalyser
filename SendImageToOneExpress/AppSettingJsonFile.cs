using System;

namespace SendImageToOneExpress
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public class LogLevel    {
        public string Default { get; set; } 
    }

    public class Logging    {
        public LogLevel LogLevel { get; set; } 
    }

    public class ConnectionStrings    {
        public string DbConn { get; set; } 
    }

    public class AppSettings    {
        public string CamsURL { get; set; } 
        public string OneExpressAPI { get; set; } 
    }

    public class Root    {
        public Logging Logging { get; set; } 
        public string AllowedHosts { get; set; } 
        public ConnectionStrings ConnectionStrings { get; set; } 
        public AppSettings AppSettings { get; set; } 
    }
    
}