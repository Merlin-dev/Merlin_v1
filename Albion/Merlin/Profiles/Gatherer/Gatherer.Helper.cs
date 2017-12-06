using Albion_Direct;
using Merlin.Pathing;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Merlin.Profiles.Gatherer
{
    public static class StuckHelper
    {
        struct SpeedValue
        {
            public float speed;
            public DateTime stamp;
        }

        static readonly TimeSpan _stuckTimeInSeconds = TimeSpan.FromSeconds(5);
        static List<SpeedValue> _previousSpeeds = new List<SpeedValue>();

        static bool IsPlayerStuck(float player_speed)
        {
            if (_previousSpeeds.Count == 0 || _previousSpeeds.Back().stamp != DateTime.Now)
            {
                _previousSpeeds.Add(new SpeedValue { speed = player_speed, stamp = DateTime.Now });
            }

            DateTime lastValidTime = DateTime.Now.Subtract(_stuckTimeInSeconds);
            _previousSpeeds.RemoveAll(x => x.stamp < lastValidTime);

            for (int i = 0; i < _previousSpeeds.Count; ++i)
            {
                if (_previousSpeeds[i].speed != 0f)
                    return false;
            }
            return true;
        }

        public static bool IsPlayerStuck(LocalPlayerCharacterView player)
        {
            return IsPlayerStuck(player.GetMoveSpeed());
        }

        public static void PretendPlayerIsMoving()
        {
            IsPlayerStuck(1f);
        }
    }

    public struct GatherInformation
    {
        private ResourceType _resourceType;
        private Tier _tier;
        private EnchantmentLevel _enchantmentLevel;

        public ResourceType ResourceType { get { return _resourceType; } }
        public Tier Tier { get { return _tier; } }
        public EnchantmentLevel EnchantmentLevel { get { return _enchantmentLevel; } }
        public DateTime? HarvestDate { get; set; }

        public GatherInformation(ResourceType resourceType, Tier tier, EnchantmentLevel enchantmentLevel)
        {
            _resourceType = resourceType;
            _tier = tier;
            _enchantmentLevel = enchantmentLevel;
            HarvestDate = null;
        }

        public override string ToString()
        {
            return $"{ResourceType} {Tier}.{(int)EnchantmentLevel}";
        }
    }

    public sealed class GatherInformationContainer
    {
        readonly int _resource_size = Enum.GetNames(typeof(ResourceType)).Length;
        readonly int _tier_size = Enum.GetNames(typeof(Tier)).Length;
        readonly int _enchantment_lvl_size = Enum.GetNames(typeof(EnchantmentLevel)).Length;
        List<List<List<bool>>> _data;
        List<List<List<GUIContent>>> _data_gui;

        public GatherInformationContainer()
        {
            _data = ListExtensions.RepeatedDefault<List<List<bool>>>(_resource_size);
            _data_gui = ListExtensions.RepeatedDefault<List<List<GUIContent>>>(_resource_size);
            for (int resource = 0; resource < _resource_size; ++resource) {
                _data[resource] = ListExtensions.RepeatedDefault<List<bool>>(_tier_size);
                _data_gui[resource] = ListExtensions.RepeatedDefault<List<GUIContent>>(_tier_size);
                for (int tier = 0; tier < _tier_size; ++tier) {
                    int ench_size = _enchantment_lvl_size;

                    if ((Tier)tier < Tier.IV || (ResourceType)resource == ResourceType.Rock)
                        ench_size = 1;

                    _data[resource][tier] = ListExtensions.RepeatedDefault<bool>(ench_size);
                    _data_gui[resource][tier] = ListExtensions.RepeatedDefault<GUIContent>(ench_size);
                    for (int ench = 0; ench < ench_size; ++ench)
                    {
                        _data_gui[resource][tier][ench] = new GUIContent(ToString((ResourceType)resource, (Tier)(tier + 1), (EnchantmentLevel)ench));
                    }
                }
            }
        }

        public int Size()
        {
            return _data.Count;
        }

        public List<List<bool>> GetResourceData(ResourceType r)
        {
            return _data[(int)r];
        }

        public GUIContent GetGUIContent(ResourceType r, Tier t, EnchantmentLevel e)
        {
            if (r == ResourceType.Rock || t < Tier.IV)
            {
                return _data_gui[(int)r][(int)t - 1][0];
            }
            else
            {
                return _data_gui[(int)r][(int)t - 1][(int)e];
            }
        }

        public void Enable(ResourceType r, Tier t, EnchantmentLevel e, bool is_enabled)
        {
            if (r == ResourceType.Rock || t < Tier.IV)
            {
                _data[(int)r][(int)t - 1][0] = is_enabled;
            }
            else
            {
                _data[(int)r][(int)t - 1][(int)e] = is_enabled;
            }
        }

        public bool IsEnabled(ResourceType r, Tier t, EnchantmentLevel e)
        {
            if (r == ResourceType.Rock || t < Tier.IV)
            {
                return _data[(int)r][((int)t) - 1][0];
            }
            else
            {
                return _data[(int)r][((int)t) - 1][(int)e];
            }
        }

        public string ToString(ResourceType r, Tier t, EnchantmentLevel e)
        {
            return $"{r} {t}.{(int)e}";
        }

        public void SerializeToPlayerPrefs(string prefs_identifier)
        {
            for (int resource = 0; resource < _data.Count; ++resource) {
                for (int tier = 0; tier < _data[resource].Count; ++tier) {
                    for (int ench = 0; ench < _data[resource][tier].Count; ++ench) {
                        PlayerPrefs.SetString($"{prefs_identifier}{_data_gui[resource][tier][ench].text}", _data[resource][tier][ench].ToString());
                    }
                }
            }
        }

        public void DeserializeFromPlayerPrefs(string prefs_identifier)
        {
            for (int resource = 0; resource < _data.Count; ++resource) {
                for (int tier = 0; tier < _data[resource].Count; ++tier) {
                    for (int ench = 0; ench < _data[resource][tier].Count; ++ench) {
                        _data[resource][tier][ench] = bool.Parse(PlayerPrefs.GetString($"{prefs_identifier}{_data_gui[resource][tier][ench].text}", ((Tier)tier >= Tier.II).ToString()));
                    }
                }
            }
        }

    }

    public partial class Gatherer
    {
        public bool HandleAttackers()
        {
            if (_localPlayerCharacterView.IsUnderAttack(out FightingObjectView attacker))
            {
                Core.Log("[Under Attack]");
                _state.Fire(Trigger.EncounteredAttacker);
                return true;
            }
            return false;
        }

        public bool HandlePathing(ref WorldPathingRequest request, Func<bool> breakFunc = null, Action onDone = null, bool ignoreMount = false)
        {
            if (request != null)
            {
                if (!ignoreMount && !HandleMounting(Vector3.zero))
                    return true;

                if ((breakFunc?.Invoke()).GetValueOrDefault())
                    request = null;
                else if (request.IsRunning)
                    request.Continue();
                else
                {
                    request = null;
                    onDone?.Invoke();
                }

                return true;
            }

            return false;
        }

        public bool HandlePathing(ref ClusterPathingRequest request, Func<bool> breakFunc = null, Action onDone = null, bool ignoreMount = false)
        {
            if (request != null)
            {
                if (!ignoreMount && !HandleMounting(Vector3.zero))
                    return true;

                if ((breakFunc?.Invoke()).GetValueOrDefault())
                    request = null;
                else if (request.IsRunning)
                    request.Continue();
                else
                {
                    request = null;
                    onDone?.Invoke();
                }

                return true;
            }

            return false;
        }

        public bool HandlePathing(ref PositionPathingRequest request, Func<bool> breakFunc = null, Action onDone = null, bool ignoreMount = false)
        {
            if (request != null)
            {
                if (!ignoreMount && !HandleMounting(Vector3.zero))
                    return true;

                if ((breakFunc?.Invoke()).GetValueOrDefault())
                    request = null;
                else if (request.IsRunning)
                    request.Continue();
                else
                {
                    request = null;
                    onDone?.Invoke();
                }

                return true;
            }

            return false;
        }

        public bool IsBlockedMounting(Vector2 location)
        {
            var vector = new Vector3(location.x, 0, location.y);
            if (_skipUnrestrictedPvPZones && _landscape.IsInAnyUnrestrictedPvpZone(vector))
                return true;

            MountObjectView mount = GetLocalMount();

            if (mount != null)
            {
                var resourcePosition = new Vector2(mount.transform.position.x,
                                                    mount.transform.position.z);
                var resourceDistance = (resourcePosition - location).magnitude;

                if (resourceDistance < (mount.GetColliderExtents() + _localPlayerCharacterView.GetColliderExtents()))
                    return false;
            }

            var playerLocation = new Vector2(_localPlayerCharacterView.transform.position.x,
                                                _localPlayerCharacterView.transform.position.z);
            var playerDistance = (playerLocation - location).magnitude;

            if (playerDistance < 2f)
                return false;

            byte cf = _collision.GetCollision(location.b(), 2.0f);
            return ((cf & 0x01) != 0) || ((cf & 0x02) != 0) || ((cf & 0xFF) != 0);
        }

        public bool HasLocalMount()
        {
            MountObjectView mount = GetLocalMount();
            return mount != null;
        }

        public MountObjectView GetLocalMount()
        {
            MountObjectView mount = null;
            foreach (var _mount in _mounts)
            {
                var IsLocalPlayers = _mount.MountObject.s9();
                if (IsLocalPlayers)
                {
                    mount = _mount;
                    break;
                }
            }
            return mount;
        }

        public bool HandleMounting(Vector3 target)
        {
            if (!_localPlayerCharacterView.IsMounted)
            {

                LocalPlayerCharacter localPlayer = _localPlayerCharacterView.LocalPlayerCharacter;
                if (localPlayer.GetIsMounting())
                    return false;

                var mount = _client.GetEntities<MountObjectView>(m => m.IsInUseRange(localPlayer)).FirstOrDefault();
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
                    //Not yet working. Meant to make character walk home if mount is broken.
                    //if (_localPlayerCharacterView.GetLocalPlayerCharacter().IsMountBroken())
                        //return false;

                    _localPlayerCharacterView.MountOrDismount();
                }

                return false;
            }

            return true;
        }
    }
}