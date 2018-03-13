using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Albion_Direct
{
    public partial class LandscapeManager
    {
        public List<aim> GetUnrestrictedPvpZones => _internal.g().e;

        public bool IsInAnyUnrestrictedPvpZone(Vector3 location) => IsInAnyUnrestrictedPvpZone(GetUnrestrictedPvpZones, location);

        public bool IsInAnyUnrestrictedPvpZone(IEnumerable<aim> pvpZones, Vector3 location) => pvpZones.Any(pvpZone => Mathf.Pow(location.x - pvpZone.f, 2) + Mathf.Pow(location.z - pvpZone.g, 2) < Mathf.Pow(pvpZone.h, 2));

        public bool IsInAnyUnrestrictedPvpZone(Func<aim, bool> selector, Vector3 location) => IsInAnyUnrestrictedPvpZone(GetUnrestrictedPvpZones.Where(selector), location);
    }
}