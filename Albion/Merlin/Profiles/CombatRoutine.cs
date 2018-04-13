using Albion_Direct;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Merlin.Profiles
{
    class CombatRoutine : MonoBehaviour
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
        protected GameManager client;
        protected LocalPlayerCharacterView player;
        private FightingObjectView target;
        private LocalPlayerCharacter playerCharakter;
        private IEnumerable<SpellSlot> combatSpells;
        private float cooldown = 0;
        private bool active = false;

        private void Start()
        {
            
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.KeypadPlus))
            {
                active = true;
                // Get Current Spells
                if (playerCharakter != null)
                    combatSpells = playerCharakter.GetSpellSlotsIndexed().Ready(player).Ignore("ESCAPE_DUNGEON").Ignore("PLAYER_COUPDEGRACE").Ignore("AMBUSH");
                foreach (var x in combatSpells)
                {
                    Core.Log(x.GetSpellDescriptor().TryGetName() + " " + x.GetSpellDescriptor().TryGetCategory() + " " + x.GetSpellDescriptor().TryGetTarget() + " " + x.Slot);
                }
            }                
            else if (Input.GetKeyDown(KeyCode.KeypadMinus))
                active = false;
            cooldown = Mathf.MoveTowards(cooldown, 0, Time.deltaTime);
            if (cooldown > 0 || !active)
            {
                //Core.Log("Cooldown or not active.");
                return;
            }
            client = GameManager.GetInstance();
            player = client.GetLocalPlayerCharacterView();
            playerCharakter = player.GetLocalPlayerCharacter();
            target = player.GetAttackTarget();

            Combat();
        }

        public void Combat()
        {
            if (client.GetState() != GameState.Playing || player.IsMounted)
            {
                Core.Log("GameState or Mounted.");
                return;
            }
                

            if (!target || (target && target.IsDead()))
            {
                Core.Log("No Target or Dead");
                return;
            }

            combatSpells = playerCharakter.GetSpellSlotsIndexed().Ready(player).Ignore("ESCAPE_DUNGEON").Ignore("PLAYER_COUPDEGRACE").Ignore("AMBUSH").Ignore("MIGHTYSWING");

            if (!target.IsCasting() && !target.bIsChanneling())
            {
                combatSpells = combatSpells.Ignore("INTERRUPT");
                combatSpells = combatSpells.Ignore("SHRIEKMACE");
                combatSpells = combatSpells.Ignore("FLAMESHIELD");
            }

            if (SpellPriorityList.Any(s => TryToCastSpell(s.Item1, s.Item2, s.Item3)))
                return;

            //if (_localPlayerCharacterView.IsUnderAttack(out FightingObjectView attacker))
                //Core.Log("You are under attack. Attack the attacker");
                //_localPlayerCharacterView.SetSelectedObject(attacker);
                //_localPlayerCharacterView.AttackSelectedObject();
        }

        private bool TryToCastSpell(SpellTarget spellTarget, SpellCategory category, bool checkCastState)
        {
            try
            {

                if (checkCastState && player.IsCasting())
                {
                    Core.Log("You are casting. Wait for casting to finish");
                    return false;
                }
                /*
                if (!combatSpells.Any(x => x.GetSpellDescriptor().TryGetName() == "INTERRUPT") && !combatSpells.Any(x => x.GetSpellDescriptor().TryGetName() == "SHRIEKMACE") && target.IsCasting()) // We have to Interrupt but CD
                {
                    Core.Log("Using CD Reduce Boots.");
                    player.CastOnSelf(CharacterSpellSlot.Shoes);
                    return true;
                }*/
                if (combatSpells.Any(x => x.GetSpellDescriptor().TryGetName() == "INTERRUPT")) // Interrupt Casting Mob
                {
                    Core.Log("Using Interrupt");
                    player.CastOn(CharacterSpellSlot.MainHand2, target);
                    return true;
                }
                else if (combatSpells.Any(x => x.GetSpellDescriptor().TryGetName() == "SHRIEKMACE")) // Silence Casting Mob
                {
                    Core.Log("Using Silence");
                    player.CastOnSelf(CharacterSpellSlot.OffHandOrMainHand3);
                    return true;
                }
                else if (combatSpells.Any(x => x.GetSpellDescriptor().TryGetName() == "FLAMESHIELD")) // Protect from Cast, No Interrupt ready.
                {
                    Core.Log("Using Flameshield");
                    player.CastOnSelf(CharacterSpellSlot.Armor);
                    return true;
                }
                else if (combatSpells.Any(x => x.GetSpellDescriptor().TryGetName() == "SHRINKINGSMASH")) // Baboom
                {
                    Core.Log("Using Shrinking Smash");
                    player.CastAt(CharacterSpellSlot.OffHandOrMainHand3, target.GetPosition());
                    return true;
                }
                else if (combatSpells.Any(x => x.GetSpellDescriptor().TryGetName() == "DEFENSIVESLAM")) // Baboom
                {
                    Core.Log("Using Slam");
                    player.CastOn(CharacterSpellSlot.MainHand1, target);
                    return true;
                }

                var spells = combatSpells.Target(spellTarget).Category(category);
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
                    switch (spellTarget)
                    {
                        case (SpellTarget.Self):
                            Core.Log("Casting " + spellName + " on self.");
                            player.CastOnSelf(spellSlot);
                            break;

                        case (SpellTarget.Enemy):
                            Core.Log("Casting " + spellName + " on enemy.");
                            player.CastOn(spellSlot, target);
                            break;

                        case (SpellTarget.Ground):
                            Core.Log("Casting " + spellName + " on ground.");
                            player.CastAt(spellSlot, target.GetPosition());
                            break;

                        default:
                            Core.Log($"[SpellTarget {spellTarget} is not supported. Spell skipped]");
                            return false;
                    }

                    cooldown = 0.1f;
                    return true;

                }
                catch (Exception e)
                {
                    Core.Log($"[Error while casting {spellName} ({spellTarget}/{category}/{checkCastState})]");
                    Core.Log(e);
                    return false;
                }
            }
            catch (Exception e)
            {
                Core.Log($"[Generic casting error ({spellTarget}/{category}/{checkCastState})]");
                Core.Log(e);
                return false;
            }
        }
    }
}
