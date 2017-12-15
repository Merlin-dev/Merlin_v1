using Albion_Direct;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Merlin.Profiles.Gatherer
{
    public sealed partial class Gatherer
    {
        private static List<Tuple<SpellTarget, SpellCategory, bool>> SpellPriorityList = new List<Tuple<SpellTarget, SpellCategory, bool>>
        {
            new Tuple<SpellTarget, SpellCategory, bool>(SpellTarget.Self, SpellCategory.Buff, true),
            new Tuple<SpellTarget, SpellCategory, bool>(SpellTarget.Self, SpellCategory.Damage, true),
            new Tuple<SpellTarget, SpellCategory, bool>(SpellTarget.Ground, SpellCategory.CrowdControl, true),
            new Tuple<SpellTarget, SpellCategory, bool>(SpellTarget.Self, SpellCategory.CrowdControl, true),
            new Tuple<SpellTarget, SpellCategory, bool>(SpellTarget.Ground, SpellCategory.Damage, true),
            //new Tuple<SpellTarget, SpellCategory, bool>(SpellTarget.Ground, SpellCategory.Debuff, true),
            new Tuple<SpellTarget, SpellCategory, bool>(SpellTarget.Enemy, SpellCategory.Damage, true),
            new Tuple<SpellTarget, SpellCategory, bool>(SpellTarget.Enemy, SpellCategory.MovementBuff, true),
        };

        private LocalPlayerCharacter _combatPlayer;
        private FightingObjectView _combatTarget;
        private IEnumerable<SpellSlot> _combatSpells;
        private float _combatCooldown;

        public void Fight()
        {
            StuckHelper.PretendPlayerIsMoving();

            if (_localPlayerCharacterView.IsMounted)
            {
                Core.Log("Player Mounted. Dismount now.");
                _localPlayerCharacterView.MountOrDismount();
                return;
            }

            if (_combatCooldown > 0 || _localPlayerCharacterView.bIsChanneling())
            {
                Core.LogOnce("Combat Cooldown > 0. Player is channeling: " + _localPlayerCharacterView.bIsChanneling());
                _combatCooldown -= UnityEngine.Time.deltaTime;
                return;
            }

            _combatPlayer = _localPlayerCharacterView.GetLocalPlayerCharacter();
            _combatTarget = _localPlayerCharacterView.GetAttackTarget();
            _combatSpells = _combatPlayer.GetSpellSlotsIndexed().Ready(_localPlayerCharacterView).Ignore("ESCAPE_DUNGEON").Ignore("PLAYER_COUPDEGRACE").Ignore("AMBUSH");

            if (_localPlayerCharacterView.IsCasting() || _combatPlayer.GetIsCasting())
            {
                Core.Log("You are casting. Wait for casting to finish");
                return;
            }
            
            if (_combatTarget != null && !_combatTarget.IsCasting())
            {
                //Ignore the Spell Interrupt to have it up if needed!
                _combatSpells = _combatSpells.Ignore("INTERRUPT");
            }
            else
            {
                //if Interrupt Spell is useable use it to interrupt the cast and dont run like a pussy and die.
                if (_combatTarget != null && _combatTarget.IsCasting() && _combatSpells.First().GetSpellDescriptor().TryGetName() == "INTERRUPT2")
                {
                    Core.Log("Interrupt ready. Use it!");
                }
                else if (_combatTarget != null && _combatTarget.IsCasting())
                {
                    Core.Log("Running away from Spell");
                    // Choose a random point behind Player.
                    Vector3 back = _localPlayerCharacterView.transform.forward * 15f;
                    float randAngle = UnityEngine.Random.Range(-75f, 75f);
                    back = Quaternion.AngleAxis(randAngle, Vector3.up) * back;
                    Vector3 randPos = back + _localPlayerCharacterView.transform.position;

                    _localPlayerCharacterView.StopAnyActionObject();
                    _localPlayerCharacterView.RequestMove(randPos);
                    _combatCooldown = 0.3f;
                }
            }

            if (_combatCooldown > 0 || _localPlayerCharacterView.bIsChanneling())
            {
                Core.LogOnce("Combat Cooldown > 0. Player is channeling: " + _localPlayerCharacterView.bIsChanneling());
                _combatCooldown -= UnityEngine.Time.deltaTime;
                return;
            }

            if (_combatTarget != null && !_combatTarget.IsDead() && SpellPriorityList.Any(s => TryToCastSpell(s.Item1, s.Item2, s.Item3)))
                return;

            if (_localPlayerCharacterView.IsUnderAttack(out FightingObjectView attacker) && !(_combatTarget != null && _combatTarget.IsCasting()))
            {
                Core.LogOnce("You are under attack. Attack the attacker");
                _localPlayerCharacterView.SetSelectedObject(attacker);
                _localPlayerCharacterView.AttackSelectedObject();
                return;
            }

            if (_combatPlayer.GetIsCasting())
            {
                Core.Log("You are casting. Wait for casting to finish");
                return;
            }

            if (_combatPlayer.GetHealth().GetValue() < (_combatPlayer.GetHealth().GetMaximum() * 0.8f))
            {
                Core.LogOnce("Health below 80%");
                var healSpell = _combatSpells.Target(SpellTarget.Self).Category(SpellCategory.Heal);

                if (healSpell.Any())
                {
                    Core.Log("Cast heal spell on self");
                    _localPlayerCharacterView.CastOnSelf(healSpell.FirstOrDefault().Slot);
                }
                return;
            }

            _currentTarget = null;
            _harvestPathingRequest = null;

            Core.Log("[Eliminated]");
            _state.Fire(Trigger.EliminatedAttacker);
        }

        private bool TryToCastSpell(SpellTarget target, SpellCategory category, bool checkCastState)
        {
            try
            {
                if (checkCastState && _localPlayerCharacterView.IsCasting())
                {
                    Core.Log("You are casting. Wait for casting to finish");
                    return false;
                }

                var spells = _combatSpells.Target(target).Category(category);
                var spellToCast = spells.Any() ? spells.First() : null;
                if (spellToCast == null)
                {
                    Core.LogOnce("Spell to Cast == Null. Exit spell cast");
                    return false;
                }

                var spellName = "Unknown";
                try
                {
                    spellName = spellToCast.GetSpellDescriptor().TryGetName();
                    var spellSlot = spellToCast.Slot;
                    switch (target)
                    {
                        case (SpellTarget.Self):
                            Core.Log("Casting " + spellName + " on self.");
                            _localPlayerCharacterView.CastOnSelf(spellSlot);
                            break;

                        case (SpellTarget.Enemy):
                            Core.Log("Casting " + spellName + " on enemy.");
                            _localPlayerCharacterView.CastOn(spellSlot, _combatTarget);
                            break;

                        case (SpellTarget.Ground):
                            Core.Log("Casting " + spellName + " on ground.");
                            _localPlayerCharacterView.CastAt(spellSlot, _combatTarget.GetPosition());
                            break;

                        default:
                            Core.Log($"[SpellTarget {target} is not supported. Spell skipped]");
                            return false;
                    }
                    _combatCooldown = 0.1f;
                    return true;
                    
                }
                catch (Exception e)
                {
                    Core.Log($"[Error while casting {spellName} ({target}/{category}/{checkCastState})]");
                    Core.Log(e);
                    return false;
                }
            }
            catch (Exception e)
            {
                Core.Log($"[Generic casting error ({target}/{category}/{checkCastState})]");
                Core.Log(e);
                return false;
            }
        }
    }
}