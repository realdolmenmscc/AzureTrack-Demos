using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SuperApp.Models
{
    public sealed class ClientResult
    {
        public string OsName { get; set; }
        public string BrowserName { get; set; }
        public int CombinationCount { get; set; }
    }
}
