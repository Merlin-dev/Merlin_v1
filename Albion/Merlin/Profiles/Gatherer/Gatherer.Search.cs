using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Merlin.Profiles.Gatherer
{
    public sealed partial class Gatherer
    {
        private SimulationObjectView _currentTarget;

        private void Search()
        {
            if (_localPlayerCharacterView.IsUnderAttack(out FightingObjectView attacker))
            {
                Core.Log("[Attacked]");
                _state.Fire(Trigger.EncounteredAttacker);
                return;
            }

            if (_localPlayerCharacterView.GetLoadPercent() > BANKING_PECTENTAGE)
            {
                Core.Log("Overweight");
                _state.Fire(Trigger.Overweight);
                return;
            }

            if (_currentTarget != null)
            {
                Core.Log("[Blacklisting target]");

                Blacklist(_currentTarget, TimeSpan.FromMinutes(0.5));

                _currentTarget = null;
                _harvestPathingRequest = null;

                return;
            }

            if (IdentifiedTarget(out SimulationObjectView target))
            {
                Core.Log("[Checking Target]");

                _currentTarget = target;
            }

            if (_currentTarget != null && ValidateTarget(_currentTarget))
            {
                Core.Log("[Identified]");

                _state.Fire(Trigger.DiscoveredResource);

                return;
            }
        }

        public bool IdentifiedTarget(out SimulationObjectView target)
        {
            var resources = _client.GetEntities<HarvestableObjectView>(ValidateHarvestable);

            var views = new List<SimulationObjectView>();
            foreach (var r in resources)
            {
                views.Add(r);
            }

            //TODO: Filter by config

            target = views.OrderBy((view) =>
            {
                var playerPosition = _localPlayerCharacterView.transform.position;
                var resourcePosition = view.transform.position;

                var score = (resourcePosition - playerPosition).sqrMagnitude;

                if (view is HarvestableObjectView harvestable)
                {
                    var harvestableObject = harvestable.GetHarvestableObject();
                    var rareState = harvestableObject.GetRareState();

                    if (harvestableObject.GetResourceDescriptor().Tier >= MINIMUM_HARVESTABLE_TIER + 1) score /= (harvestableObject.GetResourceDescriptor().Tier - 1);
                    if (harvestableObject.GetCharges() == harvestableObject.GetMaxCharges()) score /= 2;
                    if (rareState > 0) score /= rareState;
                }

                var yDelta = Math.Abs(_landscape.GetTerrainHeight(playerPosition.c(), out RaycastHit A_1) - _landscape.GetTerrainHeight(resourcePosition.c(), out RaycastHit A_2));

                score += (yDelta * 10f);

                return (int)score;
            }).FirstOrDefault();

            if (target != null)
                Core.Log($"Resource spotted: {target.name}");

            return target != default(SimulationObjectView);
        }

        public bool IsBlocked(Vector2 location)
        {
            if (_currentTarget != null)
            {
                var resourcePosition = new Vector2(_currentTarget.transform.position.x,
                                                    _currentTarget.transform.position.z);
                var distance = (resourcePosition - location).magnitude;

                if (distance < (_currentTarget.GetColliderExtents() + _localPlayerCharacterView.GetColliderExtents()))
                    return false;
            }

            if (_localPlayerCharacterView != null)
            {
                var playerLocation = new Vector2(_localPlayerCharacterView.transform.position.x,
                                                    _localPlayerCharacterView.transform.position.z);
                var distance = (playerLocation - location).magnitude;

                if (distance < 2f)
                    return false;
            }

            return _collision.GetCollision(location.b(), 1.0f) > 0;
        }
    }
}