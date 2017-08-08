using Stateless;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Merlin.Profiles.Gatherer
{
    public partial class Gatherer : Profile
    {
        #region Fields

        bool _isRunning;
        int _selectedTownClusterIndex;
        int _selectedMininumTierIndex;
        StateMachine<State, Trigger> _state;
        Dictionary<SimulationObjectView, Blacklisted> _blacklist;
        Dictionary<GatherInformation, bool> _gatherInformations;

        #endregion Fields

        #region Properties and Events

        public override string Name => "Gatherer";

        public string[] TownClusterNames { get { return Enum.GetNames(typeof(TownClusterName)).Select(n => n.Replace("_", " ")).ToArray(); } }

        public string[] TierNames { get { return Enum.GetNames(typeof(Tier)).ToArray(); } }

        public string SelectedTownCluster { get { return TownClusterNames[_selectedTownClusterIndex]; } }

        public Tier SelectedMinimumTier { get { return (Tier)Enum.Parse(typeof(Tier), TierNames[_selectedMininumTierIndex]); } }

        #endregion Properties and Events

        #region Methods

        protected override void OnStart()
        {
            _blacklist = new Dictionary<SimulationObjectView, Blacklisted>();

            _gatherInformations = new Dictionary<GatherInformation, bool>();
            foreach (var resourceType in Enum.GetValues(typeof(ResourceType)).Cast<ResourceType>())
                foreach (var tier in Enum.GetValues(typeof(Tier)).Cast<Tier>())
                    foreach (var enchantment in Enum.GetValues(typeof(EnchantmentLevel)).Cast<EnchantmentLevel>())
                    {
                        if (tier < Tier.IV && enchantment != EnchantmentLevel.White)
                            continue;

                        _gatherInformations.Add(new GatherInformation(resourceType, tier, enchantment), tier >= Tier.II);
                    }

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
                .Permit(Trigger.Restart, State.Search)
                .Permit(Trigger.BankDone, State.Search);
        }

        protected override void OnStop()
        {
            _state = null;

            _blacklist.Clear();
            _blacklist = null;
        }

        protected override void OnUpdate()
        {
            if (!_isRunning)
                return;

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
                if (_showErrors)
                    _localPlayerCharacterView.CreateTextEffect($"[Error: {e.Message}]");

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