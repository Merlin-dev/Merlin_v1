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

        static readonly TimeSpan _stuckTimeInSeconds = TimeSpan.FromSeconds(0.5);
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