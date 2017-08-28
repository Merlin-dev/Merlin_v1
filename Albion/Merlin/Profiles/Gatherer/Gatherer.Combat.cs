using Merlin.API;
using Merlin.API.Direct;
using System.Linq;

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

            if (attackTarget != null && !attackTarget.IsDead())
            {
                var selfBuffSpells = spells.Target(SpellTarget.Self).Category(SpellCategory.Buff);
                if (selfBuffSpells.Any() && !player.GetIsCasting())
                {
                    Core.Log("[Casting Buff Spell]");
                    _localPlayerCharacterView.CastOnSelf(selfBuffSpells.FirstOrDefault().Slot);
                    return;
                }

                var selfDamageSpells = spells.Target(SpellTarget.Self).Category(SpellCategory.Damage);
                if (selfDamageSpells.Any() && !player.GetIsCasting())
                {
                    Core.Log("[Casting Damage Spell]");
                    _localPlayerCharacterView.CastOnSelf(selfDamageSpells.FirstOrDefault().Slot);
                    return;
                }

                var groundCCSpells = spells.Target(SpellTarget.Ground).Category(SpellCategory.CrowdControl);
                if (groundCCSpells.Any() && !player.GetIsCasting())
                {
                    Core.Log("[Casting Ground Spell]");
                    _localPlayerCharacterView.CastOnSelf(groundCCSpells.FirstOrDefault().Slot);
                    return;
                }

                var selfCCSpells = spells.Target(SpellTarget.Self).Category(SpellCategory.CrowdControl);
                if (selfCCSpells.Any())
                {
                    Core.Log("[Casting Self Spell]");
                    _localPlayerCharacterView.CastOnSelf(selfCCSpells.FirstOrDefault().Slot);
                    return;
                }

                var enemyDamageSpells = spells.Target(SpellTarget.Enemy).Category(SpellCategory.Damage);
                if (enemyDamageSpells.Any() && !player.GetIsCasting())
                {
                    Core.Log("[Casting Damage Spell]");
                    _localPlayerCharacterView.CastOn(enemyDamageSpells.FirstOrDefault().Slot, player.GetAttackTarget());
                    return;
                }
            }

            if (_localPlayerCharacterView.IsUnderAttack(out FightingObjectView attacker))
            {
                _localPlayerCharacterView.SetSelectedObject(attacker);
                _localPlayerCharacterView.AttackSelectedObject();
                return;
            }

            if (player.GetIsCasting())
                return;

            if (player.GetHealth().GetValue() < (player.GetHealth().GetMaximum() * 0.8f))
            {
                var healSpell = spells.Target(SpellTarget.Self).Category(SpellCategory.Heal);

                if (healSpell.Any())
                    _localPlayerCharacterView.CastOnSelf(healSpell.FirstOrDefault().Slot);
                return;
            }

            _currentTarget = null;
            _harvestPathingRequest = null;

            Core.Log("[Eliminated]");
            _state.Fire(Trigger.EliminatedAttacker);
        }
    }
}