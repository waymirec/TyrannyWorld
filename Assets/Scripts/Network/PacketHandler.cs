using System;
using Tyranny.Networking;

namespace Network
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class WorldPacketHandler : Attribute, IPacketHandler<WorldOpcode>
    {
        public WorldPacketHandler(WorldOpcode opcode)
        { 
            Opcode = opcode;
        }
            
        public WorldOpcode Opcode { get; }
    }
}