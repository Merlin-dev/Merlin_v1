using Albion_Direct;
using Albion_Direct.Pathing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using YinYang.CodeProject.Projects.SimplePathfinding.PathFinders.AStar;

namespace Merlin.Profiles.Gatherer
{
    public sealed partial class Gatherer
    {
        private static List<Tuple<SpellTarget, SpellCategory, bool>> SpellPriorityList = new List<Tuple<SpellTarget, SpellCategory, bool>>
        {
            //new Tuple<SpellTarget, SpellCategory, bool>(SpellTarget.Self, SpellCategory.Buff, true),
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
        float flee;

        public void GenerateFleePosition()
        {
            Core.Log($"Generating Flee Position");
            Vector3 back = _localPlayerCharacterView.transform.forward * 10;
            float randAngle = UnityEngine.Random.Range(-75f, 75f);
            back = Quaternion.AngleAxis(randAngle, Vector3.up) * back;
            fleePosition = back + _localPlayerCharacterView.transform.position;
            fleePositionUpToDate = true;
        }
        PositionPathingRequest SpellAvoidPathRequest;
        public void Fight()
        {
            if (_localPlayerCharacterView.IsMounted)
            {
                Core.Log("Player Mounted. Dismount now.");
                _localPlayerCharacterView.MountOrDismount();
                return;
            }
            
            _combatPlayer = _localPlayerCharacterView.GetLocalPlayerCharacter();
            // GetAttackTarget() is broken.
            _combatTarget = _localPlayerCharacterView.GetAttackTarget();
            
            if (_combatTarget == null)
            {
                //Core.Log($"CombatTarget is null, try Method 2");
                _combatTarget = FindObjectsOfType<FightingObjectView>().Where(x => !x.IsDead() && x.Id == _localPlayerCharacterView.GetTargetId()).FirstOrDefault();
            }
            if (_combatTarget == null)
            {
                //Core.Log($"CombatTarget is null, try Method 3");
                _combatTarget = FindObjectsOfType<FightingObjectView>().Where(x => !x.IsDead() && x.GetTargetId() == _localPlayerCharacterView.Id ).FirstOrDefault();
            }
            if (_combatTarget != null)
                Core.Log($"Target: " + _combatTarget.name + " Dead: " + _combatTarget.IsDead() + " Casting: " + _combatTarget.IsCasting() + " Channel: " + _combatTarget.bIsChanneling() + "Time: " + _combatTarget.GetCastEndTime());
            if (!_combatTarget || (_combatTarget && _combatTarget.IsDead()))
            {
                Core.Log("No longer under Attack");
                _state.Fire(Trigger.EliminatedAttacker);
                return;
            }
            _combatSpells = _combatPlayer.GetSpellSlotsIndexed().Ready(_localPlayerCharacterView).Ignore("ESCAPE_DUNGEON").Ignore("PLAYER_COUPDEGRACE").Ignore("AMBUSH").Ignore("SPRINT_CD_REDUCTION");

            if (_combatTarget != null && !_combatTarget.IsDead() && !_combatTarget.IsCasting())
            {
                _combatSpells = _combatSpells.Ignore("INTERRUPT");
                _combatSpells = _combatSpells.Ignore("SHRIEKMACE");
                _combatSpells = _combatSpells.Ignore("FLAMESHIELD");
            }
            #region Flee
            /*
            if (_combatTarget != null && !_combatTarget.IsDead() && _combatTarget.IsCasting())
            {
                Core.Log($"Running away from Spell");
                flee = Mathf.MoveTowards(flee, 0, Time.deltaTime);
                if (!fleePositionUpToDate)
                    GenerateFleePosition();
                Core.Log($"Flee Position Distance: " + Vector3.Distance(_localPlayerCharacterView.transform.position, fleePosition));
                if (Vector3.Distance(_localPlayerCharacterView.transform.position, fleePosition) > 0.5f)
                {
                    // Turns out we need an Cancel for all Actions here first.
                    //_combatPlayer.StopAnyActionObject();
                    _localPlayerCharacterView.RequestMove(fleePosition);
                    //_client.GetLocalPlayerCharacterView().RequestMove(fleePosition);

                    /*
                    Core.Log("Running...");
                    // Using Pathfiner to find Path and Travel there might be better, Code below causes Freez, need to be fixed.

                    if (HandlePathing(ref SpellAvoidPathRequest))
                        return;                 
                    Core.Log("Trying to find Path...");
                    if (_localPlayerCharacterView.TryFindPath(new ClusterPathfinder(), fleePosition, IsBlockedGathering, out List<Vector3> pathing))
                    {
                        Core.Log("Path found, begin travel to resource");
                        Core.lineRenderer.positionCount = pathing.Count;
                        Core.lineRenderer.SetPositions(pathing.ToArray());
                        SpellAvoidPathRequest = new PositionPathingRequest(_localPlayerCharacterView, fleePosition, pathing);
                    }
                    else
                    {
                        Core.Log("Path not found");
                        fleePositionUpToDate = false;
                    }
                }                    
                return;
                
            }
            else
            {
                Core.Log("not Running");
                fleePositionUpToDate = false;
                SpellAvoidPathRequest = null;
            }
            */
            #endregion
            
            if (_combatCooldown > 0)
            {
                Core.Log("Combat Cooldown > 0.");
                _combatCooldown -= Time.deltaTime;
                return;
            }            

            if (_localPlayerCharacterView.IsCasting() || _combatPlayer.GetIsCasting())
            {
                Core.Log("You are casting. Wait for casting to finish");
                return;
            }
            
            if (_combatTarget != null && !_combatTarget.IsDead() && SpellPriorityList.Any(s => TryToCastSpell(s.Item1, s.Item2, s.Item3)))
                return;

            if (_localPlayerCharacterView.IsUnderAttack(out FightingObjectView attacker))
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
                if (_combatSpells.Count() == 1 && _combatSpells.FirstOrDefault(x => x.GetSpellDescriptor().TryGetName() == "SPRINT_CD_REDUCTION")) // Everything on CD but CD Reduce Boots.
                {
                    _localPlayerCharacterView.CastOnSelf(CharacterSpellSlot.Shoes);
                    return true;
                }
                else if (!_combatSpells.Any(x => x.GetSpellDescriptor().TryGetName() == "INTERRUPT") && _combatTarget.IsCasting()) // We have to Interrupt but CD
                {
                    _localPlayerCharacterView.CastOnSelf(CharacterSpellSlot.Shoes);
                    return true;
                }
                if (_combatSpells.Any(x => x.GetSpellDescriptor().TryGetName() == "INTERRUPT")) // Interrupt Casting Mob
                {
                    _localPlayerCharacterView.CastOn(CharacterSpellSlot.MainHand2, _combatTarget);
                    return true;
                }
                else if (_combatSpells.Any(x => x.GetSpellDescriptor().TryGetName() == "SHRIEKMACE")) // Silence Casting Mob
                {
                    _localPlayerCharacterView.CastOnSelf(CharacterSpellSlot.OffHandOrMainHand3);
                    return true;
                }
                else if (_combatSpells.Any(x => x.GetSpellDescriptor().TryGetName() == "FLAMESHIELD")) // Protect from Cast, No Interrupt ready.
                {
                    _localPlayerCharacterView.CastOnSelf(CharacterSpellSlot.Armor);
                    return true;
                }
                else if (_combatSpells.Any(x => x.GetSpellDescriptor().TryGetName() == "DEFENSIVESLAM")) // Baboom
                {
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