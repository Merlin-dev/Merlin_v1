using Merlin.API.Direct;
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
            _currentTarget = null;
            _harvestPathingRequest = null;

            _state.Fire(Trigger.EliminatedAttacker);

            /*
            LocalPlayerCharacter player = _localPlayerCharacterView.GetLocalPlayerCharacter();

            if (_localPlayerCharacterView.IsMounted)
            {
                _localPlayerCharacterView.MountOrDismount();
                return;
            }

            SpellSlot[] spells = null;
            //TODO: Get spells

            FightingObjectView attackTarget = _localPlayerCharacterView.GetAttackTarget();

            if(attackTarget != null)
            {
                //TODO: Spell casting
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

            _state.Fire(Trigger.EliminatedAttacker);*/
        }
    }
}
