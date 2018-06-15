using System;

namespace Betty.Options
{
    public class MailerOptions
    {
        public string Server { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Pwd { get; set; }
        public string Suffix { get; set; }
        public string Receiver { get; set; }
    }
}
