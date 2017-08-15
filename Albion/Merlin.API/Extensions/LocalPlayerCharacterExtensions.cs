using Merlin.API.Direct;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Merlin
{
    public static class LocalPlayerCharacterExtensions
    {
        public static FightingObject GetFightingObject(this LocalPlayerCharacter localPlayer) => localPlayer.Internal;
        public static MovingObject GetMovingObject(this LocalPlayerCharacter localPlayer) => localPlayer.Internal;
        public static WorldObject GetWorldObject(this LocalPlayerCharacter localPlayer) => localPlayer.Internal;
        public static SimulationObject GetSimulationObject(this LocalPlayerCharacter localPlayer) => localPlayer.Internal;
    }
}
