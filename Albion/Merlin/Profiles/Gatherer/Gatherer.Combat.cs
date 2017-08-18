using Merlin.API.Direct;
using Merlin.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Merlin.Profiles.Gatherer
{
    public sealed partial class Gatherer
    {
        public bool HandleAttackers()
        {
            if (_localPlayerCharacterView.IsUnderAttack(out FightingObjectView attacker))
            {
                Core.Log("[Attacked]");
                _state.Fire(Trigger.EncounteredAttacker);
                return true;
            }
            return false;
        }

        public void Fight()
        {
            LocalPlayerCharacter player = _localPlayerCharacterView.GetLocalPlayerCharacter();

            if (_localPlayerCharacterView.IsMounted)
            {
                _localPlayerCharacterView.MountOrDismount();
                return;
            }

            var spells = player.GetSpellSlotsIndexed().Ready(_localPlayerCharacterView).Ignore("ESCAPE_DUNGEON").Ignore("PLAYER_COUPDEGRACE").Ignore("AMBUSH");

            
            FightingObjectView attackTarget = _localPlayerCharacterView.GetAttackTarget();

            if(attackTarget != null)
            {
                var selfBuffSpells = spells.Target(SpellTarget.Self).Category(SpellCategory.Buff);
                if (selfBuffSpells.Any() && !player.GetIsCasting())
                {
                    Core.Log("[Casting Buff Spell]");
                    //player.CastOnSelf(selfBuffSpells.FirstOrDefault().SpellSlot);
                    return;
                }
            }


            if(_localPlayerCharacterView.IsUnderAttack(out FightingObjectView attacker))
            {
                //TODO: Attack
                return;
            }

            if (player.GetIsCasting())
                return;


            if (player.GetHealth().GetValue() < (player.GetHealth().GetMaximum() * 0.8f))
            {
                //TODO: Spells

                return;
            }

            _currentTarget = null;
            _harvestPathingRequest = null;

            _state.Fire(Trigger.EliminatedAttacker);
        }
    }
}
