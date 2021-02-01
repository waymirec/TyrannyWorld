using System;
using Network;

namespace Managers
{
    public class WorldManager : Singleton<WorldManager>
    {
        private NetworkEventManager _networkEvent;
        private WorldNetworkClient _worldNetworkClient;

        protected override void OnAwake()
        {
            _networkEvent = Registry.Get<NetworkEventManager>();
            _worldNetworkClient = new WorldNetworkClient(54322);
        }

        private void OnEnable()
        {
            _worldNetworkClient.Start();
        }

        private void OnDisable()
        {
            _worldNetworkClient?.Stop();
        }
    }
}