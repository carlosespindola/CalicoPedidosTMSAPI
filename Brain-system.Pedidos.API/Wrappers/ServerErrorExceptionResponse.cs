using System;
using System.Collections.Generic;
using System.Text;

namespace BrainSystem.Auth.API.Wrappers
{
    public class ServerErrorExceptionResponse
    {
        public string Error { get; set; }
        public string Trace { get; set; } 
        public string InnerException { get; set; }
    }
}
