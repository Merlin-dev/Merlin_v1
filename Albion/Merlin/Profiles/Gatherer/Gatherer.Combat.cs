using Albion_Direct;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Albion_Direct.Pathing;
using UnityEngine;
using YinYang.CodeProject.Projects.SimplePathfinding.PathFinders.AStar;

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
            new Tuple<SpellTarget, SpellCategory, bool>(SpellTarget.Enemy, SpellCategory.Damage, true),
            new Tuple<SpellTarget, SpellCategory, bool>(SpellTarget.Enemy, SpellCategory.MovementBuff, true),
        };

        private LocalPlayerCharacter _combatPlayer;
        private FightingObjectView _combatTarget;
        private IEnumerable<SpellSlot> _combatSpells;
        private float _combatCooldown;
        public static Vector3 fleePosition = new Vector3(0, 0, 0);
        public static bool fleePositionUpToDate = false;

        public void GenerateFleePosition()
        {
            Core.Log($"Generating Flee Position");
            Vector3 back = _localPlayerCharacterView.transform.forward * 10f;
            float randAngle = UnityEngine.Random.Range(-75f, 75f);
            back = Quaternion.AngleAxis(randAngle, Vector3.up) * back;
            fleePosition = back + _localPlayerCharacterView.transform.position;
            fleePositionUpToDate = true;
            return;
        }

        public void Fight()
        {
            if (_localPlayerCharacterView.IsMounted)
            {
                Core.Log("Player Mounted. Dismount now.");
                _localPlayerCharacterView.MountOrDismount();
                return;
            }
            _combatPlayer = _localPlayerCharacterView.GetLocalPlayerCharacter();
            _combatTarget = _localPlayerCharacterView.GetAttackTarget();
            _combatSpells = _combatPlayer.GetSpellSlotsIndexed().Ready(_localPlayerCharacterView).Ignore("ESCAPE_DUNGEON").Ignore("PLAYER_COUPDEGRACE").Ignore("AMBUSH");

            if (_combatCooldown > 0)
            {
                Core.Log("Combat Cooldown > 0.");
                _combatCooldown -= UnityEngine.Time.deltaTime;
                return;
            }

        
            if (_localPlayerCharacterView.IsCasting() || _combatPlayer.GetIsCasting())
            {
                Core.Log("You are casting. Wait for casting to finish");
                return;
            }

            if (_combatTarget != null && !_combatTarget.IsDead() && SpellPriorityList.Any(s => TryToCastSpell(s.Item1, s.Item2, s.Item3)))
                return;

            if (_localPlayerCharacterView.IsUnderAttack(out FightingObjectView attacker) )
            {
                    Core.Log("You are under attack. Attack the attacker");
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
                Core.Log("Health below 80%");
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

                if (!_combatSpells.Any(x => x.GetSpellDescriptor().TryGetName() == "INTERRUPT") && !_combatSpells.Any(x => x.GetSpellDescriptor().TryGetName() == "SHRIEKMACE") && _combatTarget.IsCasting()) // We have to Interrupt but CD
                {
                    Core.Log("Using CD Reduce Boots.");
                    _localPlayerCharacterView.CastOnSelf(CharacterSpellSlot.Shoes);
                    return true;
                }
                else if (_combatSpells.Any(x => x.GetSpellDescriptor().TryGetName() == "INTERRUPT")) // Interrupt Casting Mob
                {
                    Core.Log("Using Interrupt");
                    _localPlayerCharacterView.CastOn(CharacterSpellSlot.MainHand2, _combatTarget);
                    return true;
                }
                else if (_combatSpells.Any(x => x.GetSpellDescriptor().TryGetName() == "SHRIEKMACE")) // Silence Casting Mob
                {
                    Core.Log("Using Silence");
                    _localPlayerCharacterView.CastOnSelf(CharacterSpellSlot.OffHandOrMainHand3);
                    return true;
                }
                else if (_combatTarget != null && _combatTarget.IsCasting() && _combatSpells.Any(x => x.GetSpellDescriptor().TryGetName() == "FLAMESHIELD"))
                {
                    _localPlayerCharacterView.CastOnSelf(CharacterSpellSlot.Armor);
                    return true;
                }
                else if (_combatSpells.Any(x => x.GetSpellDescriptor().TryGetName() == "SHRINKINGSMASH")) // Baboom
                {
                    Core.Log("Using Shrinking Smash");
                    _localPlayerCharacterView.CastAt(CharacterSpellSlot.OffHandOrMainHand3, _combatTarget.GetPosition());
                    return true;
                }
                else if (_combatSpells.Any(x => x.GetSpellDescriptor().TryGetName() == "DEFENSIVESLAM")) // Baboom
                {
                    Core.Log("Using Slam");
                    _localPlayerCharacterView.CastOn(CharacterSpellSlot.MainHand1, _combatTarget);
                    return true;
                }

                var spells = _combatSpells.Target(target).Category(category);
                var spellToCast = spells.Any() ? spells.First() : null;
                if (spellToCast == null)
                {
                    Core.Log("Spell to Cast == Null. Exit spell cast");
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