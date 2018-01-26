using Stateless;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Albion_Direct.Pathing
{
    public class ClusterPathingRequest
    {
        #region Fields

        private bool _useCollider;

        private LocalPlayerCharacterView _player;
        private SimulationObjectView _target;

        private List<Vector3> _path;
        private List<Vector3> _completedpath;

        private StateMachine<State, Trigger> _state;
        DateTime _pauseTimer;
        //Moving fields
        private float noMovementThreshold = .0001f;
        private const int noMovementFrames = 2;
        Vector3[] previousLocations = new Vector3[noMovementFrames];
        private bool isMoving;
        #endregion Fields

        #region Properties and Events

        public bool IsRunning => _state.State != State.Finish;

        #endregion Properties and Events

        #region Constructors and Cleanup

        public ClusterPathingRequest(LocalPlayerCharacterView player, SimulationObjectView target, List<Vector3> path, bool useCollider = true)
        {
            _player = player;
            _target = target;

            _path = path;
            _completedpath = new List<Vector3>();

            _useCollider = useCollider;
            DateTime _pauseTimer = DateTime.Now;
            _state = new StateMachine<State, Trigger>(State.Start);

            _state.Configure(State.Start)
                      .Permit(Trigger.ApproachTarget, State.Running);

            _state.Configure(State.Running)
                .Permit(Trigger.ReachedTarget, State.Finish)
            .Permit(Trigger.Stuck, State.Pause);

            _state.Configure(State.Pause)
                .Permit(Trigger.ReachedTarget, State.Finish)
                .Permit(Trigger.ApproachTarget, State.Running);
        }

        #endregion Constructors and Cleanup

        #region Methods

        public void Continue()
        {
            switch (_state.State)
            {

                case State.Pause:
                    {
                        if (DateTime.Now > _pauseTimer)
                        {
                            _state.Fire(Trigger.ReachedTarget);
                        }
                        

                        if (_completedpath.Count < 2)
                        {
                            //Core.Log("moving to random Location");
                            Vector3 randomSpot = new Vector3(UnityEngine.Random.Range(-100f, 100f), 0, UnityEngine.Random.Range(-100f, 100f)) + _player.transform.position;
                            _completedpath.Add(randomSpot);
                            break;
                        }
                        var previousNode = _completedpath[_completedpath.Count - 1];
                        var playerPosV2 = new Vector2(_player.transform.position.x, _player.transform.position.z);
                        var previousNodeV2 = new Vector2(previousNode.x, previousNode.z);

                        var distancePreviousToNode = (playerPosV2 - previousNodeV2).sqrMagnitude;
                        var minimumDistance = 1f;

                        if (distancePreviousToNode < minimumDistance)
                        {
                            //Core.Log("Reached Previous Node");

                        }
                        else
                        {
                            _player.RequestMove(previousNode);
                        }

                        break;
                    }
                case State.Start:
                    {
                        if (_path.Count > 0)
                            _state.Fire(Trigger.ApproachTarget);
                        else
                            _state.Fire(Trigger.ReachedTarget);

                        break;
                    }

                case State.Running:
                    {
                        //Early exit if one of them is null.
                        if (_player == null || _target == null)
                        {
                            _state.Fire(Trigger.ReachedTarget);
                            break;
                        }
                        isMovingUpdate();
                        //Core.Log($"Cluster Pathing Request. Player at {_player.transform.position}. Player is move {IsMoving}");

                        if (!IsMoving)
                        {
                            _state.Fire(Trigger.Stuck);
                            _pauseTimer = DateTime.Now + TimeSpan.FromSeconds(0.5);
                            //Core.Log("Stuck Cluster Pathing Request");
                            break;
                        }
                        var currentNode = _path[0];
                        var minimumDistance = 3f;

                        if (_path.Count < 2 && _useCollider)
                        {
                            minimumDistance = _target.GetColliderExtents() + _player.GetColliderExtents();

                            var directionToPlayer = (_player.transform.position - _target.transform.position).normalized;
                            var bufferDistance = directionToPlayer * minimumDistance;

                            currentNode = _target.transform.position + bufferDistance;
                        }

                        var distanceToNode = (_player.transform.position - currentNode).sqrMagnitude;

                        if (distanceToNode < minimumDistance)
                        {
                            _completedpath.Add(_path[0]);
                            _path.RemoveAt(0);
                        }
                        else
                        {
                            _player.RequestMove(currentNode);
                        }

                        if (_path.Count > 0)
                            break;

                        _state.Fire(Trigger.ReachedTarget);
                        break;
                    }
            }
        }

        //check for moving
        public bool IsMoving
        {
            get { return isMoving; }
        }

        void isMovingUpdate()
        {

            for (int i = 0; i < previousLocations.Length - 1; i++)
            {
                previousLocations[i] = previousLocations[i + 1];
            }
            previousLocations[previousLocations.Length - 1] = _player.transform.position;


            for (int i = 0; i < previousLocations.Length - 1; i++)
            {
               // Core.Log($"Distance is {Vector3.Distance(previousLocations[i], previousLocations[i + 1])}. Threshold is {noMovementThreshold}");
                if (Vector3.Distance(previousLocations[i], previousLocations[i + 1]) >= noMovementThreshold)
                {
                    isMoving = true;
                   // Core.Log($"IsMoving true = {isMoving}");
                    break;
                }
                else
                {
                    isMoving = false;
                   // Core.Log($"IsMoving false =  {isMoving}");
                    
                }
            }
        }
        #endregion Methods

        private enum Trigger
        {
            ApproachTarget,
            ReachedTarget,
            Stuck
        }

        private enum State
        {
            Start,
            Running,
            Finish,
            Pause
        }
    }
}
