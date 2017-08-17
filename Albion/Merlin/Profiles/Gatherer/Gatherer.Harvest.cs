using Merlin.API.Direct;
using Merlin.Pathing;
using UnityEngine;

namespace Merlin.Profiles.Gatherer
{
    public sealed partial class Gatherer
    {
        public const int MINIMUM_HARVESTABLE_TIER = 2;
        private ClusterPathingRequest _harvestPathingRequest;

        public bool ValidateHarvestable(HarvestableObjectView resource)
        {
            var resourceObject = resource.GetHarvestableObject();

            if (!resourceObject.CanLoot(_localPlayerCharacterView) || resourceObject.GetCharges() <= 0 || resourceObject.GetResourceDescriptor().Tier < MINIMUM_HARVESTABLE_TIER)
                return false;

            Vector3 position = resource.transform.position;
            float terrainHeight = _landscape.GetTerrainHeight(position.c(), out RaycastHit hit);

            if (position.y < terrainHeight - 5)
                return false;

            if (_blacklist.ContainsKey(resource))
                return false;

            return true;
        }

        public bool ValidateMob(MobView mob)
        {
            return !mob.IsDead();
        }

        public bool ValidateTarget(SimulationObjectView target)
        {
            if (target is HarvestableObjectView resource)
                return ValidateHarvestable(resource);

            if (target is MobView mob)
                return ValidateMob(mob);

            return false;
        }

        public bool HandleMounting(Vector3 target)
        {
            LocalPlayerCharacter localPlayer = _localPlayerCharacterView.LocalPlayerCharacter;

            if (!_localPlayerCharacterView.IsMounted)
            {
                if (localPlayer.GetIsMounting())
                    return false;

                MountObjectView mount = _localPlayerCharacterView.GetComponent<MountObjectView>();

                if (mount != null)
                {
                    if (target != Vector3.zero && mount.IsInUseRange(localPlayer))
                        return true;

                    if (mount.IsInUseRange(localPlayer))
                        _localPlayerCharacterView.Interact(mount);
                    else
                        _localPlayerCharacterView.MountOrDismount();
                }
                else
                {
                    _localPlayerCharacterView.MountOrDismount();
                }

                return false;
            }

            return true;
        }

        public void Harvest()
        {

        }
    }
}