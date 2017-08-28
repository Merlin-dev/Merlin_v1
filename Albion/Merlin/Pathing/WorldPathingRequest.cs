using Merlin.API.Direct;
using Stateless;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using YinYang.CodeProject.Projects.SimplePathfinding.Helpers;
using YinYang.CodeProject.Projects.SimplePathfinding.PathFinders.AStar;

namespace Merlin.Pathing
{
    public class WorldPathingRequest
    {
        #region Fields

        private GameManager _client;
        private ObjectManager _world;
        private CollisionManager _collision;

        private ClusterDescriptor _origin;
        private ClusterDescriptor _destination;

        private List<ClusterDescriptor> _path;

        private StateMachine<State, Trigger> _state;

        private ClusterPathingRequest _exitPathingRequest;

        private DateTime _timeout;

        #endregion Fields

        #region Properties and Events

        public bool IsRunning => _state.State != State.Finish;

        #endregion Properties and Events

        #region Constructors and Cleanup

        public WorldPathingRequest(ClusterDescriptor start, ClusterDescriptor end, List<ClusterDescriptor> path)
        {
            _client = GameManager.GetInstance();
            _world = ObjectManager.GetInstance();
            _collision = _world.GetCollisionManager();

            _origin = start;
            _destination = end;

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

                            if (player.TryFindPath(new ClusterPathfinder(), destination, IsBlocked, out List<Vector3> pathing))
                                _exitPathingRequest = new ClusterPathingRequest(_client.GetLocalPlayerCharacterView(), null, pathing, false);
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

        public bool IsBlocked(Vector2 location)
        {
            byte cf = _collision.GetCollision(location.b(), 2.0f);
            if (cf == 255)
            {
                var location3d = new Vector3(location.x, 0, location.y);
                var meshCollidersAtLocation = Physics.OverlapSphere(location3d, 2.0f).Where(c => c.GetType() == typeof(MeshCollider));

                return meshCollidersAtLocation.Any(c => !c.isTrigger);
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