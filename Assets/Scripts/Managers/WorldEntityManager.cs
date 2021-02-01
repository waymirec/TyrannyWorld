using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Events;
using Network;
using NLog;
using Tyranny.Networking;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions;

namespace Managers
{
    public class WorldEntityManager : Singleton<WorldEntityManager>
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();
        
        private readonly IDictionary<Guid, GameObject> _entities = new ConcurrentDictionary<Guid, GameObject>();
        private NetworkEventManager _networkEventManager;
        
        protected override void OnAwake()
        {
            _networkEventManager = Registry.Get<NetworkEventManager>();
        }


        private void OnEnable()
        {
            _networkEventManager.OnEnterWorld += OnEnterWorld;
            _networkEventManager.OnSpawnWorldEntity += OnSpawnWorldEntity;
            _networkEventManager.OnDestroyWorldEntity += OnDestroyWorldEntity;
            _networkEventManager.OnMoveWorldEntity += OnMoveWorldEntity;
            _networkEventManager.OnWorldEntityDisco += OnWorldEntityDisco;
        }

        private void OnDisable()
        {
            _networkEventManager.OnEnterWorld -= OnEnterWorld;
            _networkEventManager.OnSpawnWorldEntity -= OnSpawnWorldEntity;
            _networkEventManager.OnDestroyWorldEntity -= OnDestroyWorldEntity;
            _networkEventManager.OnMoveWorldEntity -= OnMoveWorldEntity;
            _networkEventManager.OnWorldEntityDisco -= OnWorldEntityDisco;
        }

        private void OnEnterWorld(object source, NetworkEnterWorldEventArgs args)
        {
            var guid = args.Guid;
            var position = args.Position;
            
            Logger.Debug($"Creating player object {position.x},{position.y},{position.z}");
            var entity = CreateWorldEntity(guid, position, "Player");
        }
        
        private void OnSpawnWorldEntity(object source, NetworkSpawnWorldEntityEventArgs args)
        {
            var guid = args.Guid;
            var position = args.Position;
            
            Logger.Debug($"Spawning GO {guid.ToString()} at {position.x},{position.y},{position.z}");
            var entity = CreateWorldEntity(guid, position, "Player");
        }

        private void OnDestroyWorldEntity(object source, NetworkDestroyWorldEntityEventArgs args)
        {
            Assert.IsTrue(_entities.ContainsKey(args.Guid));
            Destroy(_entities[args.Guid]);
        }

        private void OnMoveWorldEntity(object source, NetworkMoveWorldEntityArgs args)
        {
            Assert.IsTrue(_entities.ContainsKey(args.Guid));
            var entity = _entities[args.Guid];
            entity.GetComponent<NavMeshAgent>().SetDestination(args.Destination);
        }

        private void OnWorldEntityDisco(object source, NetworkWorldEntityDiscoArgs args)
        {
            Logger.Debug($"OnWorldEntityDisco :: {args.Guid}");
            Assert.IsTrue(_entities.ContainsKey(args.Guid));
            var entity = _entities[args.Guid];
            Logger.Debug($"Target Entity => {entity}");
            var visible = _entities
                .Where(kvp => kvp.Key != args.Guid)
                .Where(kvp => Vector3.Distance(entity.transform.position, kvp.Value.transform.position) < 100.0);
            
            foreach(var kvp in visible)
            {
                Logger.Debug($"Advertising entity: {kvp.Key}");
                var pos = kvp.Value.transform.position;
                var disco = new PacketWriter<WorldOpcode>(WorldOpcode.SpawnWorldEntity);
                disco.Write(args.Guid.ToByteArray());
                disco.Write(kvp.Key.ToByteArray());
                disco.Write(pos.x);
                disco.Write(pos.y);
                disco.Write(pos.z);
                var client = (WorldNetworkClient) source;
                client.Send(disco, "239.0.0.0");
            }
        }
        
        private GameObject CreateWorldEntity(Guid id, Vector3 position, string resourceName)
        {
            var entity = Instantiate(Resources.Load("Player"), position, Quaternion.identity) as GameObject;
            Assert.IsNotNull(entity);
            entity.name = id.ToString();
            _entities[id] = entity;
            return entity;
        }
    }
}