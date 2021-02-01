using System;
using Events;
using Network;
using NLog;
using Tyranny.Networking;
using UnityEngine;

namespace Managers
{
    public class WorldPacketHandlers
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        [WorldPacketHandler(WorldOpcode.Hello)]
        public static void HandleHello(PacketReader<WorldOpcode> packetIn, WorldNetworkClient worldClient)
        {
            var guid = new Guid(packetIn.ReadBytes(16));
            worldClient.Id = guid;

            Registry.Get<NetworkEventManager>().FireEvent_OnWorldAuth(worldClient, new NetworkWorldAuthEventArgs
            {
                Guid = worldClient.Id
            });
        }
        
        [WorldPacketHandler(WorldOpcode.EnterWorld)]
        public static void HandleEnterWorld(PacketReader<WorldOpcode> packetIn, WorldNetworkClient worldClient)
        {
            var guid = new Guid(packetIn.ReadBytes(16));
            Registry.Get<NetworkEventManager>().FireEvent_OnEnterWorld(worldClient, new NetworkEnterWorldEventArgs
            {
                Guid = guid,
                Position = new Vector3(packetIn.ReadSingle(), packetIn.ReadSingle(), packetIn.ReadSingle())
            });
        }

        [WorldPacketHandler(WorldOpcode.SpawnWorldEntity)]
        public static void HandleSpawnWorldEntity(PacketReader<WorldOpcode> packetIn, WorldNetworkClient worldClient)
        {
            Registry.Get<NetworkEventManager>().FireEvent_OnSpawnWorldEntity(worldClient, new NetworkSpawnWorldEntityEventArgs
            {
                Guid = new Guid(packetIn.ReadBytes(16)), 
                Position = new Vector3(packetIn.ReadSingle(), packetIn.ReadSingle(), packetIn.ReadSingle())
            });
        }

        [WorldPacketHandler(WorldOpcode.DestroyWorldEntity)]
        public static void HandleDestroyWorldEntity(PacketReader<WorldOpcode> packetIn, WorldNetworkClient worldClient)
        {
            Registry.Get<NetworkEventManager>().FireEvent_OnDestroyWorldEntity(worldClient, new NetworkDestroyWorldEntityEventArgs
            {
                Guid = new Guid(packetIn.ReadBytes(16))
            });
        }
        
        [WorldPacketHandler(WorldOpcode.MoveWorldEntity)]
        public static void HandleMoveWorldEntity(PacketReader<WorldOpcode> packetIn, WorldNetworkClient worldClient)
        {
            Registry.Get<NetworkEventManager>().FireEvent_OnMoveWorldEntity(worldClient, new NetworkMoveWorldEntityArgs
            {
                Guid = new Guid(packetIn.ReadBytes(16)),
                Source = new Vector3(packetIn.ReadSingle(), packetIn.ReadSingle(), packetIn.ReadSingle()),
                Destination = new Vector3(packetIn.ReadSingle(), packetIn.ReadSingle(), packetIn.ReadSingle())
            });
        }

        [WorldPacketHandler(WorldOpcode.WorldEntityDisco)]
        public static void HandleWorldEntityDisco(PacketReader<WorldOpcode> packetIn, WorldNetworkClient worldClient)
        {
            Registry.Get<NetworkEventManager>().FireEvent_OnWorldEntityDisco(worldClient, new NetworkWorldEntityDiscoArgs
            {
                Guid = new Guid(packetIn.ReadBytes(16))
            });
        }
    }
}