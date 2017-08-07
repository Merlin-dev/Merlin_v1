using Stateless;
using System;
using System.Collections.Generic;

namespace Merlin.Profiles.Gatherer
{
    public partial class Gatherer : Profile
    {
        #region Fields

        private StateMachine<State, Trigger> _state;
        private Dictionary<SimulationObjectView, Blacklisted> _blacklist;

        #endregion Fields

        #region Properties and Events

        public override string Name => "Gatherer";

        #endregion Properties and Events

        #region Methods

        protected override void OnStart()
        {
            _blacklist = new Dictionary<SimulationObjectView, Blacklisted>();

            _state = new StateMachine<State, Trigger>(State.Search);
            _state.Configure(State.Search)
                .Permit(Trigger.EncounteredAttacker, State.Combat)
                .Permit(Trigger.DiscoveredResource, State.Harvest)
                .Permit(Trigger.Overweight, State.Bank);

            _state.Configure(State.Combat)
                .Permit(Trigger.EliminatedAttacker, State.Search);

            _state.Configure(State.Harvest)
                .Permit(Trigger.DepletedResource, State.Search)
                .Permit(Trigger.EncounteredAttacker, State.Combat);

            _state.Configure(State.Bank)
                .Permit(Trigger.Restart, State.Search);
        }

        protected override void OnStop()
        {
            _state = null;

            _blacklist.Clear();
            _blacklist = null;
        }

        protected override void OnUpdate()
        {
            if (_blacklist.Count > 0)
            {
                var whitelist = new List<SimulationObjectView>();

                foreach (var blacklisted in _blacklist.Values)
                {
                    if (DateTime.Now >= blacklisted.Timestamp)
                        whitelist.Add(blacklisted.Target);
                }

                foreach (var target in whitelist)
                    _blacklist.Remove(target);
            }

            try
            {
                switch (_state.State)
                {
                    case State.Search:
                        Search();
                        break;

                    case State.Harvest:
                        Harvest();
                        break;

                    case State.Combat:
                        Fight();
                        break;

                    case State.Bank:
                        Bank();
                        break;
                }
            }
            catch (Exception e)
            {
                Core.Log(e);
            }
        }

        private void Blacklist(SimulationObjectView target, TimeSpan duration)
        {
            _blacklist[target] = new Blacklisted()
            {
                Target = target,
                Timestamp = DateTime.Now + duration,
            };
        }

        #endregion Methods

        private class Blacklisted
        {
            public SimulationObjectView Target { get; set; }

            public DateTime Timestamp { get; set; }
        }
    }
}