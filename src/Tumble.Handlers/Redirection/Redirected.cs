using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Tumble.Handlers.Redirection
{
    public class Redirected
    {
        public string Source { get; private set; }
        public string Destination { get; private set; }

        public Redirected(HttpRequestMessage source, Uri destination)
        {
            Source = source.RequestUri.ToString();
            Destination = destination.ToString();
        }
    }
   
   
}
