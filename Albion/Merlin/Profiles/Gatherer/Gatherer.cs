using Albion_Direct;
using Stateless;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Merlin.Profiles.Gatherer
{
    public enum State
    {
        Search,
        Harvest,
        Combat,
        Bank,
        Repair,
        Travel,
        SiegeCampTreasure,
    }

    public enum Trigger
    {
        Restart,
        DiscoveredResource,
        BankDone,
        RepairDone,
        DepletedResource,
        Overweight,
        Damaged,
        EncounteredAttacker,
        EliminatedAttacker,
        StartTravelling,
        TravellingDone,
        StartSiegeCampTreasure,
        OnSiegeCampTreasureDone,
        Failure,
    }

    public sealed partial class Gatherer : Profile
    {
        #region [dTormentedSoul Area]
        DateTime _startDateTime = DateTime.Now;
        int messageDelayTrigger = 50;
        int messageDelayIncrement = 0;
        #endregion [dTormentedSoul Area]

        private bool _isRunning = false;

        private StateMachine<State, Trigger> _state;
        private Dictionary<SimulationObjectView, Blacklisted> _blacklist;
        private Dictionary<Point2, GatherInformation> _gatheredSpots;
        private List<Point2> _keeperSpots;
        private List<MountObjectView> _mounts;
        private bool _knockedDown;

        public override string Name => "Gatherer";

        protected override void OnStart()
        {
            _blacklist = new Dictionary<SimulationObjectView, Blacklisted>();
            _gatheredSpots = new Dictionary<Point2, GatherInformation>();
            _keeperSpots = new List<Point2>();

            LoadSettings();

            _state = new StateMachine<State, Trigger>(State.Search);
            _state.Configure(State.Search)
                .Permit(Trigger.StartSiegeCampTreasure, State.SiegeCampTreasure)
                .Permit(Trigger.StartTravelling, State.Travel)
                .Permit(Trigger.EncounteredAttacker, State.Combat)
                .Permit(Trigger.DiscoveredResource, State.Harvest)
                .Permit(Trigger.Overweight, State.Bank)
                .Permit(Trigger.Damaged, State.Repair);

            _state.Configure(State.Combat)
                .Permit(Trigger.EliminatedAttacker, State.Search);

            _state.Configure(State.Harvest)
                .Permit(Trigger.DepletedResource, State.Search)
                .Permit(Trigger.EncounteredAttacker, State.Combat);

            _state.Configure(State.Bank)
                .Permit(Trigger.Restart, State.Search)
                .Permit(Trigger.BankDone, State.Search);

            _state.Configure(State.Repair)
                .Permit(Trigger.RepairDone, State.Search);

            _state.Configure(State.Travel)
                .Permit(Trigger.TravellingDone, State.Search);

            _state.Configure(State.SiegeCampTreasure)
                .Permit(Trigger.OnSiegeCampTreasureDone, State.Search);

            foreach (State state in Enum.GetValues(typeof(State)))
            {
                if (state != State.Search)
                    _state.Configure(state).Permit(Trigger.Failure, State.Search);
            }
        }

        protected override void OnStop()
        {
            SaveSettings();

            _state = null;

            _blacklist.Clear();
            _blacklist = null;
        }

        protected override void OnUpdate()
        {
            //If we don't have a view, do not do anything!
            if (_localPlayerCharacterView == null)
                return;

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
                foreach (var keeper in _client.GetEntities<MobView>(mob => !mob.IsDead() && (mob.MobType().ToLowerInvariant().Contains("keeper") || mob.MobType().ToLowerInvariant().Contains("undead") || mob.MobType().ToLowerInvariant().Contains("bonecrusher"))))
                {
                    var keeperPosition = keeper.GetInternalPosition();
                    if (!_keeperSpots.Contains(keeperPosition))
                        _keeperSpots.Add(keeperPosition);
                }

                _mounts = _client.GetEntities<MountObjectView>(mount => mount.IsInUseRange(_localPlayerCharacterView.LocalPlayerCharacter));

                if (_knockedDown != _localPlayerCharacterView.IsKnockedDown())
                {
                    _knockedDown = _localPlayerCharacterView.IsKnockedDown();
                    if (_knockedDown)
                    {
                        Core.Log("[DEAD - Currently knocked down!]");
                    }
                }

                switch (_state.State)
                {
                    case State.Search: Search(); break;
                    case State.Harvest: Harvest(); break;
                    case State.Combat: Fight(); break;
                    case State.Bank: Bank(); break;
                    case State.Repair: Repair(); break;
                    case State.Travel: Travel(); break;
                    case State.SiegeCampTreasure: SiegeCampTreasure(); break;
                }
            }
            catch (Exception e)
            {
                Core.Log(e);

                ResetCriticalVariables();
                _state.Fire(Trigger.Failure);
            }
        }

        private void ResetCriticalVariables()
        {
            _worldPathingRequest = null;
            _bankPathingRequest = null;
            _repairPathingRequest = null;
            _repairFindPathingRequest = null;
            _harvestPathingRequest = null;
            _currentTarget = null;
            _failedFindAttempts = 0;
            _changeGatheringPathRequest = null;
            _siegeCampTreasureCoroutine = null;
            _targetCluster = null;
            _travelPathingRequest = null;
            _movingToRepair = false;
            _movingToBank = false;
        }

        private void Blacklist(SimulationObjectView target, TimeSpan duration)
        {
            _blacklist[target] = new Blacklisted()
            {
                Target = target,
                Timestamp = DateTime.Now + duration,
            };
        }

        private class Blacklisted
        {
            public SimulationObjectView Target { get; set; }

            public DateTime Timestamp { get; set; }
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
    }
}