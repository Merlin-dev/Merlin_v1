using System;
using System.Linq;
using UnityEngine;
using Albion_Direct;

namespace Merlin.Profiles.Gatherer
{
    public partial class Gatherer
    {
        #region Fields

        public KeyCode runningKey = KeyCode.F12;
        public KeyCode espKey = KeyCode.F11;
        public KeyCode unloadKey = KeyCode.F10;
        public KeyCode testkey = KeyCode.F9;

        private static int SpaceBetweenSides = 40;
        private static int SpaceBetweenItems = 4;

        private bool _isUIshown;
        private bool _showESP;

        #endregion Fields

        #region Properties

        private static Rect GatheringUiButtonRect { get; } = new Rect((Screen.width / 2) - 50, 0, 100, 20);

        private static Rect GatheringBotButtonRect { get; } = new Rect((Screen.width / 2) + 50, 0, 100, 20);

        private Rect GatheringWindowRect { get; set; } = new Rect((Screen.width / 2) - 506, 0, 0, 0);

        private string[] TownClusterNames
        { get { return Enum.GetNames(typeof(TownClusterName)).Select(n => n.Replace("_", " ")).ToArray(); } }

        private string[] TierNames
        { get { return Enum.GetNames(typeof(Tier)).ToArray(); } }

        private Tier SelectedMinimumTier
        { get { return (Tier)Enum.Parse(typeof(Tier), TierNames[_selectedMininumTierIndex]); } }

        #endregion Properties

        #region Methods

        private void DrawGatheringUIButton()
        {
            if (GUI.Button(GatheringUiButtonRect, "Gathering UI"))
                _isUIshown = true;

            DrawRunButton(false);
        }

        private void DrawGatheringUIWindow(int windowID)
        {
            GUILayout.BeginHorizontal();
            DrawGatheringUILeft();
            GUILayout.Space(SpaceBetweenSides);
            DrawGatheringUIRight();
            GUILayout.EndHorizontal();

            GUI.DragWindow();
        }

        private void DrawGatheringUILeft()
        {
            GUILayout.BeginVertical();
            DrawGatheringUI_Buttons();
            DrawGatheringUI_Toggles();
            DragGatheringUI_Sliders();
            DrawGatheringUI_SelectionGrids();
            DrawGatheringUI_TextFields();
            GUILayout.EndVertical();
        }

        private void DrawGatheringUI_Toggles()
        {
            _allowMobHunting = GUILayout.Toggle(_allowMobHunting, "Allow hunting of living mobs (experimental - can cause issues)");
            _skipUnrestrictedPvPZones = GUILayout.Toggle(_skipUnrestrictedPvPZones, "Skip unrestricted PvP zones while gathering");
            _skipKeeperPacks = GUILayout.Toggle(_skipKeeperPacks, "Skip keeper mobs while gathering");
            _allowSiegeCampTreasure = GUILayout.Toggle(_allowSiegeCampTreasure, "Allow usage of siege camp treasures");
            _skipRedAndBlackZones = GUILayout.Toggle(_skipRedAndBlackZones, "Skip red and black zones for traveling");
            UpdateESP(GUILayout.Toggle(_showESP, "Show ESP"));
        }

        private void UpdateESP(bool newValue)
        {
            var oldValue = _showESP;
            _showESP = newValue;

            if (oldValue != _showESP)
            {
                if (_showESP)
                    gameObject.AddComponent<ESP.ESP>().StartESP(_gatherInformations);
                else if (gameObject.GetComponent<ESP.ESP>() != null)
                    Destroy(gameObject.GetComponent<ESP.ESP>());
            }
        }

        private void DragGatheringUI_Sliders()
        {
            if (_skipKeeperPacks)
            {
                GUILayout.Label($"Skip keeper range: {_keeperSkipRange}");
                _keeperSkipRange = GUILayout.HorizontalSlider(_keeperSkipRange, 5, 50);
            }

            GUILayout.Label($"Minimum health percentage for gathering: {_minimumHealthForGathering.ToString("P2")}");
            _minimumHealthForGathering = GUILayout.HorizontalSlider(_minimumHealthForGathering, 0.01f, 1f);

            GUILayout.Label($"Weight percentage needed for banking: {_percentageForBanking}");
            _percentageForBanking = Mathf.Round(GUILayout.HorizontalSlider(_percentageForBanking, 1, 400));

            if (_allowSiegeCampTreasure)
            {
                GUILayout.Label($"Weight percentage needed for siege camp treasure: {_percentageForSiegeCampTreasure}");
                _percentageForSiegeCampTreasure = Mathf.Round(GUILayout.HorizontalSlider(_percentageForSiegeCampTreasure, 1, 400));
            }
        }

        private void DrawGatheringUI_SelectionGrids()
        {
            GUILayout.Label("Selected city cluster for banking:");
            _selectedTownClusterIndex = GUILayout.SelectionGrid(_selectedTownClusterIndex, TownClusterNames, TownClusterNames.Length);

            GUILayout.Label("Selected minimum resource tier of interest:");
            _selectedMininumTierIndex = GUILayout.SelectionGrid(_selectedMininumTierIndex, TierNames, TierNames.Length);
        }

        private void DrawGatheringUI_TextFields()
        {
            GUILayout.Label("Selected cluster for gathering:");
            var currentClusterInfo = _world.GetCurrentCluster() != null ? _world.GetCurrentCluster().GetName() : "Unknown";
            var selectedGatherCluster = string.IsNullOrEmpty(_selectedGatherCluster) ? currentClusterInfo : _selectedGatherCluster;
            _selectedGatherCluster = GUILayout.TextField(selectedGatherCluster);
        }

        private void DrawGatheringUIRight()
        {
            GUILayout.BeginVertical();
            GUILayout.Label("Resources to gather:");
            DrawGatheringUI_GatheringToggles();
            GUILayout.EndVertical();
        }

        private void DrawGatheringUI_Buttons()
        {
            if (GUILayout.Button("Close Gathering UI"))
                _isUIshown = !_isUIshown;

            DrawRunButton(true);

            if (GUILayout.Button("Unload"))
                Core.Unload();
        }

        private void DrawGatheringUI_GatheringToggles()
        {
            GUILayout.BeginHorizontal();
            var selectedMinimumTier = SelectedMinimumTier;
            var groupedKeys = _gatherInformations.Keys.GroupBy(i => i.ResourceType).ToArray();
            for (var i = 0; i < groupedKeys.Count(); i++)
            {
                var keys = groupedKeys[i].ToArray();

                GUILayout.BeginVertical();
                for (var j = 0; j < keys.Length; j++)
                {
                    var info = keys[j];
                    if (info.Tier < selectedMinimumTier)
                        _gatherInformations[info] = false;
                    else
                        _gatherInformations[info] = GUILayout.Toggle(_gatherInformations[info], info.ToString());
                }
                GUILayout.EndVertical();
                GUILayout.Space(SpaceBetweenItems);
            }
            GUILayout.EndHorizontal();
        }

        private void DrawRunButton(bool layouted)
        {
            var text = _isRunning ? "Stop Gathering" : "Start Gathering";
            if (layouted ? GUILayout.Button(text) : GUI.Button(GatheringBotButtonRect, text))
            {
                _isRunning = !_isRunning;
                if (_isRunning)
                {
                    ResetCriticalVariables();
                    if (_selectedGatherCluster == "Unknown" && _world.GetCurrentCluster() != null)
                        _selectedGatherCluster = _world.GetCurrentCluster().GetName();
                    _localPlayerCharacterView.CreateTextEffect("[Start]");
                    if (_state.CanFire(Trigger.Failure))
                        _state.Fire(Trigger.Failure);
                }
            }
        }

        protected override void OnUI()
        {
            if (_isUIshown)
                GatheringWindowRect = GUILayout.Window(0, GatheringWindowRect, DrawGatheringUIWindow, "Gathering UI");
            else
                DrawGatheringUIButton();
        }
        
        protected override void HotKey()
        {
            if (Input.GetKeyDown(runningKey))
            {
                _isRunning = !_isRunning;
                if (_isRunning)
                {
                    ResetCriticalVariables();
                    if (_selectedGatherCluster == "Unknown" && _world.GetCurrentCluster() != null)
                        _selectedGatherCluster = _world.GetCurrentCluster().GetName();
                    _localPlayerCharacterView.CreateTextEffect("[Start]");
                    if (_state.CanFire(Trigger.Failure))
                        _state.Fire(Trigger.Failure);
                }
            }
            else if(Input.GetKeyDown(testkey))
            {
                Vector3 playerCenter = _localPlayerCharacterView.transform.position;
                Core.Log("X: " + playerCenter.x + " Y: " + playerCenter.y + " Z: " + playerCenter.z);

                ClusterDescriptor currentWorldCluster = _world.GetCurrentCluster();
                Core.Log("City: " + currentWorldCluster.GetName().ToLowerInvariant());

            }
            else if (Input.GetKeyDown(unloadKey))
            {
                Core.Unload();
            }
            else if (Input.GetKeyDown(espKey))
            {
                var oldValue = _showESP;
                _showESP = !_showESP;

                if (oldValue != _showESP)
                {
                    if (_showESP)
                        gameObject.AddComponent<ESP.ESP>().StartESP(_gatherInformations);
                    else if (gameObject.GetComponent<ESP.ESP>() != null)
                        Destroy(gameObject.GetComponent<ESP.ESP>());
                }
            }
        }

        #endregion Methods
    }
}