using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Merlin.API.Direct
{
    public partial class LandscapeManager
    {
        public List<aea> GetUnrestrictedPvpZones => _internal.f().e;

        public bool IsInAnyUnrestrictedPvpZone(Vector3 location) => GetUnrestrictedPvpZones.Any(pvpZone => Mathf.Pow(location.x - pvpZone.k(), 2) + Mathf.Pow(location.z - pvpZone.l(), 2) < Mathf.Pow(pvpZone.m(), 2));
    }
}