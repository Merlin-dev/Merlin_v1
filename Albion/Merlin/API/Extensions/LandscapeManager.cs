using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Merlin.API.Direct
{
    public partial class LandscapeManager
    {
        public List<afd> GetUnrestrictedPvpZones => _internal.f().e;

        public bool IsInAnyUnrestrictedPvpZone(Vector3 location) => IsInAnyUnrestrictedPvpZone(GetUnrestrictedPvpZones, location);

        public bool IsInAnyUnrestrictedPvpZone(IEnumerable<afd> pvpZones, Vector3 location) => pvpZones.Any(pvpZone => Mathf.Pow(location.x - pvpZone.k(), 2) + Mathf.Pow(location.z - pvpZone.l(), 2) < Mathf.Pow(pvpZone.m(), 2));

        public bool IsInAnyUnrestrictedPvpZone(Func<afd, bool> selector, Vector3 location) => IsInAnyUnrestrictedPvpZone(GetUnrestrictedPvpZones.Where(selector), location);
    }
}