using Stateless;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using YinYang.CodeProject.Projects.SimplePathfinding.PathFinders.AStar;
using Albion_Direct;

namespace Merlin.Pathing
{
    public class WorldPathingRequest
    {
        #region Fields

        private static readonly Vector3 HeightDifference = new Vector3 { y = 1000 };

        private GameManager _client;
        private ObjectManager _world;
        private CollisionManager _collision;
        private LandscapeManager _landscape;

        private ClusterDescriptor _origin;
        private ClusterDescriptor _destination;

        private Vector3? _destinationPosition;
        private float? _destinationExtends;

        private List<ClusterDescriptor> _path;

        private StateMachine<State, Trigger> _state;

        private PositionPathingRequest _exitPathingRequest;

        private DateTime _timeout;
        private bool _skipUnrestrictedPvPZones;

        #endregion Fields

        #region Properties and Events

        public bool IsRunning => _state.State != State.Finish;

        #endregion Properties and Events

        #region Constructors and Cleanup

        public WorldPathingRequest(ClusterDescriptor start, ClusterDescriptor end, List<ClusterDescriptor> path, bool skipUnrestrictedPvPZones)
        {
            _client = GameManager.GetInstance();
            _world = ObjectManager.GetInstance();
            _collision = _world.GetCollisionManager();

            _origin = start;
            _destination = end;
            _skipUnrestrictedPvPZones = skipUnrestrictedPvPZones;

            _path = path;
            _timeout = DateTime.Now;

            _state = new StateMachine<State, Trigger>(State.Start);

            _state.Configure(State.Start)
                .Permit(Trigger.ApproachDestination, State.Running);

            _state.Configure(State.Running)
                .Permit(Trigger.ReachedDestination, State.Finish);
        }

        #endregion Constructors and Cleanup

        #region Methods

        public void Continue()
        {
            if (_timeout > DateTime.Now)
                return;

            switch (_state.State)
            {
                case State.Start:
                    {
                        if (_path.Count > 0)
                            _state.Fire(Trigger.ApproachDestination);
                        else
                            _state.Fire(Trigger.ReachedDestination);

                        break;
                    }

                case State.Running:
                    {
                        var nextCluster = _path[0];
                        var currentCluster = _world.GetCurrentCluster();

                        if (currentCluster.GetIdent() != nextCluster.GetIdent())
                        {
                            if (_exitPathingRequest != null)
                            {
                                if (_exitPathingRequest.IsRunning)
                                {
                                    _exitPathingRequest.Continue();
                                }
                                else
                                {
                                    _timeout = DateTime.Now + TimeSpan.FromSeconds(5);
                                    _exitPathingRequest = null;
                                }

                                break;
                            }

                            var player = _client.GetLocalPlayerCharacterView();
                            var exits = currentCluster.GetExits();

                            var exit = exits.FirstOrDefault(e => e.GetDestination().GetIdent() == nextCluster.GetIdent());
                            var exitLocation = exit.GetPosition();

                            var destination = new Vector3(exitLocation.GetX(), 0, exitLocation.GetY());

                            var exitCollider = Physics.OverlapCapsule(destination - HeightDifference, destination + HeightDifference, 2f).FirstOrDefault(c => c.name.ToLowerInvariant().Equals("exit") || c.name.ToLowerInvariant().Contains("entrance"));
                            _destinationPosition = exitCollider?.transform?.position;
                            _destinationExtends = exitCollider?.GetColliderExtents();
                            if (_destinationPosition.HasValue)
                            {
                                var temp = _destinationPosition.Value;
                                temp.y = 0;
                                _destinationPosition = temp;
                            }

                            _landscape = _client.GetLandscapeManager();
                            if (player.TryFindPath(new ClusterPathfinder(), destination, IsBlockedWithExitCheck, out List<Vector3> pathing))
                                _exitPathingRequest = new PositionPathingRequest(_client.GetLocalPlayerCharacterView(), destination, pathing, false);
                        }
                        else
                        {
                            _path.RemoveAt(0);
                            _exitPathingRequest = null;
                        }

                        if (_path.Count > 0)
                            break;

                        _state.Fire(Trigger.ReachedDestination);
                        break;
                    }
            }
        }

        public bool IsBlockedWithExitCheck(Vector2 location)
        {
            var vector = new Vector3(location.x, 0, location.y);

            if (_skipUnrestrictedPvPZones && _landscape.IsInAnyUnrestrictedPvpZone(vector))
                return true;

            var location3d = new Vector3(location.x, 0, location.y);
            if (_destinationPosition.HasValue && _destinationExtends.HasValue && Vector3.Distance(location3d, _destinationPosition.Value) <= _destinationExtends.Value)
                return false;

            byte cf = _collision.GetCollision(location.b(), 2.0f);
            if (cf == 255)
            {
                //if the location contains an exit return false (passable), otherwise return true
                var locationContainsExit = Physics.OverlapSphere(location3d, 2.0f).Any(c => c.name.ToLowerInvariant().Equals("exit") || c.name.ToLowerInvariant().Contains("entrance"));
                return !locationContainsExit;
            }
            else
                return (((cf & 0x01) != 0) || ((cf & 0x02) != 0));
        }

        #endregion Methods

        private enum Trigger
        {
            ApproachDestination,
            ReachedDestination,
        }

        private enum State
        {
            Start,
            Running,
            Finish
        }
    }
}