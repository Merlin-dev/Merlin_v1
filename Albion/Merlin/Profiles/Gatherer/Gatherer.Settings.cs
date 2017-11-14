using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Merlin.Profiles.Gatherer
{
    public sealed partial class Gatherer
    {
        private static string _prefsIdentifier = "gath_";

        private bool _allowMobHunting;
        private bool _skipUnrestrictedPvPZones;
        private bool _skipKeeperPacks;
        private bool _allowSiegeCampTreasure;
        private bool _skipRedAndBlackZones;
        private float _keeperSkipRange;
        private float _minimumHealthForGathering;
        private float _percentageForBanking;
        private float _percentageForSiegeCampTreasure;
        private string _selectedGatherCluster;
        private int _selectedTownClusterIndex;
        private int _selectedMininumTierIndex;
        private Dictionary<GatherInformation, bool> _gatherInformations;

        private void LoadSettings()
        {
            _allowMobHunting = bool.Parse(PlayerPrefs.GetString($"{_prefsIdentifier}{nameof(_allowMobHunting)}", bool.FalseString));
            _skipUnrestrictedPvPZones = bool.Parse(PlayerPrefs.GetString($"{_prefsIdentifier}{nameof(_skipUnrestrictedPvPZones)}", bool.TrueString));
            _skipKeeperPacks = bool.Parse(PlayerPrefs.GetString($"{_prefsIdentifier}{nameof(_skipKeeperPacks)}", bool.TrueString));
            _allowSiegeCampTreasure = bool.Parse(PlayerPrefs.GetString($"{_prefsIdentifier}{nameof(_allowSiegeCampTreasure)}", bool.TrueString));
            _skipRedAndBlackZones = bool.Parse(PlayerPrefs.GetString($"{_prefsIdentifier}{nameof(_skipRedAndBlackZones)}", bool.TrueString));
            _keeperSkipRange = PlayerPrefs.GetFloat($"{_prefsIdentifier}{nameof(_keeperSkipRange)}", 22);
            _minimumHealthForGathering = PlayerPrefs.GetFloat($"{_prefsIdentifier}{nameof(_minimumHealthForGathering)}", 0.8f);
            _percentageForBanking = PlayerPrefs.GetFloat($"{_prefsIdentifier}{nameof(_percentageForBanking)}", 99f);
            _percentageForSiegeCampTreasure = PlayerPrefs.GetFloat($"{_prefsIdentifier}{nameof(_percentageForSiegeCampTreasure)}", 33f);
            _selectedGatherCluster = PlayerPrefs.GetString($"{_prefsIdentifier}{nameof(_selectedGatherCluster)}", null);
            _selectedTownClusterIndex = PlayerPrefs.GetInt($"{_prefsIdentifier}{nameof(_selectedTownClusterIndex)}", 0);
            _selectedMininumTierIndex = PlayerPrefs.GetInt($"{_prefsIdentifier}{nameof(_selectedMininumTierIndex)}", 0);
            _gatherInformations = new Dictionary<GatherInformation, bool>();
            foreach (var resourceType in Enum.GetValues(typeof(Albion_Direct.ResourceType)).Cast<Albion_Direct.ResourceType>())
                foreach (var tier in Enum.GetValues(typeof(Albion_Direct.Tier)).Cast<Albion_Direct.Tier>())
                    foreach (var enchantment in Enum.GetValues(typeof(Albion_Direct.EnchantmentLevel)).Cast<Albion_Direct.EnchantmentLevel>())
                    {
                        if ((tier < Albion_Direct.Tier.IV || resourceType == Albion_Direct.ResourceType.Rock) && enchantment != Albion_Direct.EnchantmentLevel.White)
                            continue;

                        var info = new GatherInformation(resourceType, tier, enchantment);
                        var val = bool.Parse(PlayerPrefs.GetString($"{_prefsIdentifier}{info.ToString()}", (tier >= Albion_Direct.Tier.II).ToString()));
                        _gatherInformations.Add(info, val);
                    }
        }

        private void SaveSettings()
        {
            PlayerPrefs.SetString($"{_prefsIdentifier}{nameof(_allowMobHunting)}", _allowMobHunting.ToString());
            PlayerPrefs.SetString($"{_prefsIdentifier}{nameof(_skipUnrestrictedPvPZones)}", _skipUnrestrictedPvPZones.ToString());
            PlayerPrefs.SetString($"{_prefsIdentifier}{nameof(_skipKeeperPacks)}", _skipKeeperPacks.ToString());
            PlayerPrefs.SetString($"{_prefsIdentifier}{nameof(_allowSiegeCampTreasure)}", _allowSiegeCampTreasure.ToString());
            PlayerPrefs.SetString($"{_prefsIdentifier}{nameof(_skipRedAndBlackZones)}", _skipRedAndBlackZones.ToString());
            PlayerPrefs.SetFloat($"{_prefsIdentifier}{nameof(_keeperSkipRange)}", _keeperSkipRange);
            PlayerPrefs.SetFloat($"{_prefsIdentifier}{nameof(_minimumHealthForGathering)}", _minimumHealthForGathering);
            PlayerPrefs.SetFloat($"{_prefsIdentifier}{nameof(_percentageForBanking)}", _percentageForBanking);
            PlayerPrefs.SetFloat($"{_prefsIdentifier}{nameof(_percentageForSiegeCampTreasure)}", _percentageForSiegeCampTreasure);
            PlayerPrefs.SetString($"{_prefsIdentifier}{nameof(_selectedGatherCluster)}", _selectedGatherCluster);
            PlayerPrefs.SetInt($"{_prefsIdentifier}{nameof(_selectedTownClusterIndex)}", _selectedTownClusterIndex);
            PlayerPrefs.SetInt($"{_prefsIdentifier}{nameof(_selectedMininumTierIndex)}", _selectedMininumTierIndex);
            foreach (var kvp in _gatherInformations)
                PlayerPrefs.SetString($"{_prefsIdentifier}{kvp.Key.ToString()}", kvp.Value.ToString());
        }
    }
}