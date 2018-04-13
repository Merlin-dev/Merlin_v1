using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Merlin.Profiles.Gatherer;
using Albion_Direct;
using Albion.Common.Time;

namespace Merlin.Profiles
{
    public class Fishing : MonoBehaviour
    {
        public static FishingManager fishingManager;
        public static LocalInputHandler input;
        private static LocalPlayerCharacterView player;
        private GameManager _client;

        public static float cooldown = 0;
        private bool runFishing = false;
        FishingZoneObjectView myFishingZone;

        private bool useFood = false;

        private void Start()
        {
            Core.Log("Start Fishing Class");
            fishingManager = GameObject.FindObjectOfType<FishingManager>();
            input = GameObject.FindObjectOfType<LocalInputHandler>();
            player = FindObjectOfType<LocalPlayerCharacterView>();
        }

        private void Update()
        {
            _client = GameManager.GetInstance();
            if (_client.GetState() != GameState.Playing)
                return;
            if (player == null)
            {
                player = FindObjectOfType<LocalPlayerCharacterView>();
                return;
            }
            if (input == null)
            {
                input = GameObject.FindObjectOfType<LocalInputHandler>();
                return;
            }
            if (fishingManager == null)
            {
                fishingManager = GameObject.FindObjectOfType<FishingManager>();
                return;
            }
            if (runFishing && (!IsValid || player.IsMounted || player.IsAttacking() || Input.GetKey(KeyCode.Mouse1))) // Automaticly Stop Fishing
            {
                runFishing = false;
            }
            if (!runFishing && IsFishing && player.FishingState != null) // Automaticly Start Fishing when needed
            {
                runFishing = true;
            }
            if (Input.GetKeyDown(KeyCode.KeypadMultiply))
            {
                useFood = !useFood;
                Core.Log("UseFood: " + useFood.ToString());
            }            
            cooldown = Mathf.MoveTowards(cooldown, 0, Time.deltaTime);
            if (useFood)
                Fooding();
            if (!runFishing)
                return;
            if (cooldown == 0 && !IsFishing && !IsMinigameRunning) // Throw Line
            {
                myFishingZone = FindObjectsOfType<FishingZoneObjectView>().OrderBy(x => Vector3.Distance(player.transform.position, x.transform.position)).FirstOrDefault();
                input.PlayerCharacterView.SetAutoState(LocalPlayerCharacterView.a.ai, myFishingZone);
                cooldown = 1.25f;
            }
            if (cooldown == 0 && player.FishingState != null && HaveBite) // Accept Bite
            {
                player.PlayerCharacter.zo<azy>().f(GameTimeStamp.Now, false);
                cooldown = 0.25f;
                return;
            }
            if (cooldown == 0 && IsFishing && IsMinigameRunning) // Handle Minigame
            {
                if (linetention >= 0.55)
                {
                    ReelIn(true);
                }
                else
                {
                    ReelIn(false);
                }
                cooldown = 0.05f;
            }
        }
        private LocalPlayerCharacter playerCharakter;
        private IEnumerable<SpellSlot> combatSpells;
        int i = 0;
        public void Fooding()
        {
            if (useFood && cooldown == 0)
            {
                cooldown = 0.5f;
                player = _client.GetLocalPlayerCharacterView();
                playerCharakter = player.GetLocalPlayerCharacter();
                if (playerCharakter != null)
                    combatSpells = playerCharakter.GetSpellSlotsIndexed().Ready(player).Ignore("ESCAPE_DUNGEON").Ignore("PLAYER_COUPDEGRACE").Ignore("AMBUSH");
                if (player.IsCasting())
                {
                    Core.Log("You are casting. Wait for casting to finish");
                    return;
                }
                if (combatSpells.Any(x => x.GetSpellDescriptor().TryGetName().ToLowerInvariant().Contains("fish") && x.Slot == CharacterSpellSlot.Food))
                {
                    player.CastOnSelf(CharacterSpellSlot.Food);
                    i++;
                    if (i > 4)
                    {
                        i = 0;
                        ReloadFish();
                    }

                }
            }
        }
        
        private void ReloadFish()
        {
            UIItemSlot buffFoodSlot;
            string fishName = "";
            var slots = GameGui.Instance.CharacterInfoGui.EquipmentStorage.ItemsSlotsRegistered;
            //Core.Log("Slots: " + slots.Count().ToString());
            foreach (var slot in slots)
            {
                if (slot != null && slot.ObservedItemView != null && slot.ObservedItemView.name.ToLowerInvariant().Contains("fish_"))
                {
                    buffFoodSlot = slot;
                    fishName = slot.ObservedItemView.IconName.ToLowerInvariant();
                    //Core.Log(fishName + " detected");
                }
            }
            var playerStorage = GameGui.Instance.CharacterInfoGui.InventoryItemStorage;
            var resourceTypes = Enum.GetNames(typeof(ResourceType)).Select(r => r.ToLowerInvariant()).ToArray();
            foreach (var slot in playerStorage.ItemsSlotsRegistered)
            {
                if (slot != null && slot.ObservedItemView != null)
                {                    
                    var slotItemName = slot.ObservedItemView.IconName.ToLowerInvariant();
                    //Core.Log(slotItemName);
                    if (slotItemName == fishName)
                    {
                        Core.Log("Fish Found. Reloading...");
                        GameGui.Instance.EquipItem(slot, out buffFoodSlot);
                    }
                }
            }
        }

        public static float progress
        {
            get
            {
                return fishingManager.MiniGame.s1();
            }
        }

        public static float linetention
        {
            get
            {
                return fishingManager.MiniGame.s2();
            }
        }

        public static void ReelIn(bool state)
        {
            fishingManager.MiniGame.s3(!state);
        }

        public static bool Reeling
        {
            get
            {
                return fishingManager.MiniGame.s4();
            }

        }

        public static bool IsMinigameRunning
        {
            get
            {
                if (fishingManager.MiniGame.s0() == 0)
                    return true;
                else
                    return false;
            }
        }
        public static bool HaveBite
        {
            get
            {
                if(player == null)
                    player = FindObjectOfType<LocalPlayerCharacterView>();
                if (player.FishingState != null && player.FishingState.get_CurrentFishingState().ToString() == "e")
                    return true;
                else
                    return false;
            }
        }
        public static bool IsValid
        {
            get
            {
                if (fishingManager.IsFishingValidForPlayer())
                    return true;
                else
                    return false;
            }
        }
        public static bool IsFishing
        {
            get
            {
                if (player.IsFishing())
                    return true;
                else
                    return false;
            }
        }
    }
}
