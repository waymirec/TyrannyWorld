using System;
using UnityEngine;

namespace Events
{
    public class NetworkWorldAuthEventArgs : EventArgs
    {
        public Guid Guid { get; set; }
    }
    
    public class NetworkEnterWorldEventArgs : EventArgs
    {
        public Guid Guid { get; set; }
        public Vector3 Position { get; set; }
    }
    
    public class NetworkSpawnWorldEntityEventArgs : EventArgs
    {
        public Guid Guid { get; set; }
        public Vector3 Position { get; set; }
    }
    
    public class NetworkDestroyWorldEntityEventArgs : EventArgs
    {
        public Guid Guid { get; set; }
    }

    public class NetworkPingPongArgs : EventArgs
    {
        public Guid Guid { get; set; }
        public int Count { get; set; }
    }

    public class NetworkMoveWorldEntityArgs : EventArgs
    {
        public Guid Guid { get; set; }
        public Vector3 Source { get; set; }
        public Vector3 Destination { get; set; }
    }

    public class NetworkWorldEntityDiscoArgs : EventArgs
    {
        public Guid Guid { get; set; }
    }
}