using System;
using System.Threading;
using System.Threading.Tasks;

namespace Tumble.Hook.Client
{
    public class HookFactory : IHookFactory
    {
        private SemaphoreSlim _semaphore = new SemaphoreSlim(0);

        IHookClient _hookClient = null;

        public HookFactory(string host, int port)
        { }

        public async Task<IHookClient> GetClientAsync() =>
            await GetClientAsync(CancellationToken.None);

        public async Task<IHookClient> GetClientAsync(CancellationToken cancellationToken)
        {
            if (_hookClient == null)
            {
                await _semaphore.WaitAsync(cancellationToken);
                try
                {
                    var myHookClient = new HookClient();

                    _hookClient = myHookClient;
                }
                finally
                {
                    _semaphore.Release();
                }
            }
            return _hookClient;
        }
    }
}
