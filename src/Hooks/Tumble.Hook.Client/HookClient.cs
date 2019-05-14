using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Tumble.Hook.Client
{
    public class HookClient : IHookClient, IDisposable
    {
        private Socket _socket = null;
      
        public HookClient()
        {

        }

        
        public async Task<bool> ConnectAsync(string host, int port)
        {
            IPHostEntry hostEntry = Dns.GetHostEntry(host);
            foreach(IPAddress address in hostEntry.AddressList)
            {
                IPEndPoint endpoint = new IPEndPoint(address, port);
                _socket = new Socket(endpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                await _socket.ConnectAsync(endpoint);
                if (_socket.Connected)
                    return true;
            }
            return false;
        }

        public void Dispose()
        {
            _socket?.Disconnect(false);
        }

        public Task<HttpResponseMessage> SendAsync(HttpRequestMessage httpRequestMessage)
        {
            
            
            await 
            throw new NotImplementedException();
        }
    }
}
