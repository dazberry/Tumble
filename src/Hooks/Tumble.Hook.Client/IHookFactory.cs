using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Tumble.Hook.Client
{
    public interface IHookFactory
    {        
        Task<IHookClient> GetClientAsync();
        Task<IHookClient> GetClientAsync(CancellationToken cancellationToken);
    }
}
