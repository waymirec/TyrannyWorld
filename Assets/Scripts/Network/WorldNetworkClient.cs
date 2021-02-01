using System;
using System.Collections.Generic;
using CommonLib;
using Managers;
using NLog;
using Tyranny.Networking;

namespace Network
{
    public class WorldNetworkClient
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        
        public Guid Id
        {
            get => _udpClient.Id;
            set => _udpClient.Id = value;
        }
        
        public int Port { get; set; }
        
        private AsyncUdpClient<WorldOpcode> _udpClient;
        private delegate void PacketHandlerDelegate(PacketReader<WorldOpcode> packetIn, WorldNetworkClient client);
        private Dictionary<WorldOpcode, PacketHandlerDelegate> _packetHandlers;
        private int _port;
        
        public WorldNetworkClient(int port)
        {
            Port = port;
            _packetHandlers = PacketHandlerLoader<WorldOpcode>.Load<PacketHandlerDelegate>(typeof(WorldPacketHandler), typeof(WorldPacketHandlers));
            _udpClient = new AsyncUdpClient<WorldOpcode>(port);
        }

        private void OnDestroy()
        {
            Stop();
        }

        public void Start()
        {
            _udpClient.OnDataReceived += OnDataReceived;
            _udpClient.JoinMulticastGroup("239.0.1.1");
            _udpClient.Start();
            Logger.Debug($"WorldNetworkClient Started.");
        }

        public void Stop()
        {
            try
            {
                _udpClient.OnDataReceived -= OnDataReceived;
                _udpClient.LeaveMulticastGroup("239.0.1.1");
                _udpClient.Stop();
                Logger.Debug($"WorldNetworkClient Stopped.");
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, "Exception trying to stop udp client.");
            }
        }

        public void Send(PacketWriter<WorldOpcode> packetOut, string host)
        {
            _udpClient.Send(packetOut, host);
        }
        
        private void OnDataReceived(object source, AsyncUdpPacketEventArgs<WorldOpcode> args)
        {
            var opcode = args.Packet.Opcode;
            if (_packetHandlers.TryGetValue(opcode, out PacketHandlerDelegate handler))
            {
                Logger.Debug($"Received opcode: {opcode}");
                try
                {
                    handler(args.Packet, this);
                }
                catch (Exception ex)
                {
                    Logger.Warn(ex.ToString());
                }
            }
            else
            {
                Logger.Warn($"No handler found for opcode {opcode}");
            }
        }
    }
}