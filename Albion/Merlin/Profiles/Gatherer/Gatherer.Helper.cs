using Albion_Direct;
using Albion_Direct.Pathing;
using System;
using System.Linq;
using UnityEngine;

namespace Merlin.Profiles.Gatherer
{
    public partial class Gatherer
    {

        public bool HandleAttackers()
        {
            if (_localPlayerCharacterView.IsUnderAttack(out FightingObjectView attacker))
            {                
                if (_allowDismountAttack && _localPlayerCharacterView.IsMounted)
                {
                    Core.Log("[Under Attack] - Dismounting");
                    _state.Fire(Trigger.EncounteredAttacker);
                    return true;
                }
                if (!_allowDismountAttack && !_localPlayerCharacterView.IsMounted)
                {
                    Core.Log("[Under Attack] - Alread dismonted");
                    _state.Fire(Trigger.EncounteredAttacker);
                    return true;
                }
                if (!_allowDismountAttack && _localPlayerCharacterView.IsMounted)
                {
                    Core.Log("[Under Attack] - Ignoring");
                    return false;
                }
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