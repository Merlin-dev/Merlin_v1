using Stateless;
using System;
using System.Collections.Generic;
using UnityEngine;
using Albion_Direct;

namespace Merlin.Pathing
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
        private const int noMovementFrames = 30;
        Vector3[] previousLocations = new Vector3[noMovementFrames];
        private bool isMoving;
        private const float _moveSpeed = 3f;
        private const float _turnSpeed = 90f;
        private float _arrivalDistance = 1.5f;
        private const float _pathNodeLeeway = 1.7f;

        #endregion Fields

        #region Properties and Events

        public bool IsRunning => _state.State != State.Finish;
        public float FinishedDistance => _arrivalDistance;

        #endregion Properties and Events

        #region Constructors and Cleanup

        public ClusterPathingRequest(LocalPlayerCharacterView player, SimulationObjectView target, List<Vector3> path,
            float ArrivalDistance = 1.5f, bool useCollider = true)
        {
            _player = player;
            _target = target;
            _useCollider = useCollider;
            _path = path;

            _completedpath = new List<Vector3>();
            DateTime _pauseTimer = DateTime.Now;

            _arrivalDistance = ArrivalDistance + _pathNodeLeeway;
            if (useCollider)
                _arrivalDistance += _player.GetColliderExtents() + _target.GetColliderExtents();
            Core.Log("Arrival distance : " + _arrivalDistance.ToString());

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
                            Vector2 randDirection = UnityEngine.Random.insideUnitCircle * 100f;
                            Vector3 randomSpot = new Vector3(randDirection.x, 0, randDirection.y) + _player.transform.position;
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
                            _player.RequestMove(GetLerpedMovement(_player.transform, previousNode));
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

                        if (!IsMoving)
                        {
                            _state.Fire(Trigger.Stuck);
                            _pauseTimer = DateTime.Now + TimeSpan.FromSeconds(0.5);
                            break;
                        }

                        if (GetPlayerDistanceFromTarget(_player, _target, _useCollider) <= _arrivalDistance)
                        {
                            // We have arrived safely at our destination. Pat on back.
                            _path.Clear();
                            _state.Fire(Trigger.ReachedTarget);
                            break;
                        }
                        else
                        {
                            var currentNode = _path[0];
                            var distanceToNode = (_player.transform.position - currentNode).magnitude;
                            if (distanceToNode < _pathNodeLeeway)
                            {
                                _completedpath.Add(_path[0]);
                                _path.RemoveAt(0);

                                if (_path.Count > 0)
                                {
                                    currentNode = _path[0];
                                }
                                else
                                {
                                    Core.Log("[ClusterPathingRequest] WARNING : No more path nodes, but we haven't arrived at destination yet.");
                                    break;
                                }
                            }

                            // Lerp to make it more human.
                            _player.RequestMove(GetLerpedMovement(_player.transform, currentNode));
                        }
                    } break;
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

        private static Vector3 GetLerpedMovement(Transform player_transform, Vector3 target_pos)
        {
            Vector3 movement_direction = target_pos - player_transform.position;
            movement_direction.Normalize();
            float angle = Mathf.Rad2Deg * Mathf.Abs(Mathf.Acos(Vector3.Dot(player_transform.forward, movement_direction)));
            angle /= 180f;
            Vector3 move_amount = Vector3.Lerp(player_transform.forward, movement_direction, Time.deltaTime * _turnSpeed * angle);
            move_amount *= _moveSpeed;
            return player_transform.position + move_amount;
        }

        public static float GetPlayerDistanceFromTarget(LocalPlayerCharacterView player, SimulationObjectView target, bool useColliders = true)
        {
            float ret = (target.transform.position - player.transform.position).magnitude;
            if (useColliders)
            {
                ret += target.GetColliderExtents();
                ret += player.GetColliderExtents();
            }
            return ret;
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
