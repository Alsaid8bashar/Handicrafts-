using System.IO;
using System.Net;

namespace handi_crafts.Models

{
    public class MailModel
    {
        public string ToEmail { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
    }
}
