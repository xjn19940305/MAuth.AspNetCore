using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAuth.AspNetCore.Models.Options
{
    public class FileSystemOptions
    {
        public string Endpoint { get; set; }
        public string Bucket { get; set; }
        public string AccessKey { get; set; }
        public string SecretKey { get; set; }
        public string BasePath { get; set; }
        public string CdnPath { get; set; }
    }
}