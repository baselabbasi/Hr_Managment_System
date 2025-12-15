using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HrMangmentSystem_Application.Config
{
    public class OpenAiOptions
    {
        public string ApiKey { get; set; } = null!;
        public string Model { get; set; } = null!;
        public string BaseUrl { get; set; }  
    }
}
