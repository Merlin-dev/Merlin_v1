using System;
using System.Collections.Generic;
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

        static int SpaceBetweenSides = 40;
        static int SpaceBetweenItems = 4;

        bool _isUIshown;
        bool _showESP;

        string _lastSelectedGatherCluster = "";
        int _lastSelectedGatherClusterLength = 0;

        static readonly string[] _townClusterNames = Enum.GetNames(typeof(TownClusterName)).Select(n => n.Replace("_", " ")).ToArray();
        static readonly string[] _tierNames = Enum.GetNames(typeof(Tier)).ToArray();

        #endregion Fields

        #region GUIFields

        static readonly GUIContent _labelSelectedCity = new GUIContent("Selected city cluster for banking:");
        static readonly GUIContent _labelSelectedMinimumTier = new GUIContent("Selected minimum resource tier of interest:");
        static readonly GUIContent _labelSelectGatheringCluster = new GUIContent("Selected cluster for gathering:");
        static readonly GUIContent _labelUseCurrentCluster = new GUIContent("Use Current Cluster");
        static readonly GUIContent _labelResourcesToGather = new GUIContent("Resources to gather:");
        static readonly GUIContent _labelCloseUI = new GUIContent("Close Gathering UI");
        static readonly GUIContent _labelUnload = new GUIContent("Unload");
        static readonly GUIContent _labelEnableAll = new GUIContent("Enable All");
        static readonly GUIContent _labelDisableAll = new GUIContent("Disable All");
        static readonly GUIContent _labelGatheringUI = new GUIContent("Gathering UI");

        static readonly GUIContent[] _labelsTownClusterNames = _townClusterNames.Select(x => new GUIContent(x)).ToArray();
        static readonly GUIContent[] _labelsTierNames = _tierNames.Select(x => new GUIContent(x)).ToArray();

        #endregion GUIFields

        #region Properties

        static Rect GatheringUiButtonRect { get; } = new Rect((Screen.width / 2) - 50, 0, 100, 20);
        static Rect GatheringBotButtonRect { get; } = new Rect((Screen.width / 2) + 50, 0, 100, 20);
        Rect GatheringWindowRect { get; set; } = new Rect((Screen.width / 2) - 506, 0, 0, 0);

        Tier SelectedMinimumTier { get { return (Tier)_selectedMininumTierIndex; } }

        #endregion Properties

        #region Methods

        void DrawGatheringUIButton()
        {
            if (GUI.Button(GatheringUiButtonRect, "Gathering UI"))
                _isUIshown = true;

            DrawRunButton(false);
        }

        void DrawGatheringUIWindow(int windowID)
        {
            GUILayout.BeginHorizontal();
            DrawGatheringUILeft();
            GUILayout.Space(SpaceBetweenSides);
            DrawGatheringUIRight();
            GUILayout.EndHorizontal();

            GUI.DragWindow();
        }

        void DrawGatheringUILeft()
        {
            GUILayout.BeginVertical();
            DrawGatheringUI_Buttons();
            DrawGatheringUI_Toggles();
            DragGatheringUI_Sliders();
            DrawGatheringUI_SelectionGrids();
            DrawGatheringUI_TextFields();
            GUILayout.EndVertical();
        }

        void DrawGatheringUI_Toggles()
        {
            _allowMobHunting = GUILayout.Toggle(_allowMobHunting, "Allow hunting of living mobs (experimental - can cause issues)");
            _skipUnrestrictedPvPZones = GUILayout.Toggle(_skipUnrestrictedPvPZones, "Skip unrestricted PvP zones while gathering");
            _skipKeeperPacks = GUILayout.Toggle(_skipKeeperPacks, "Skip keeper mobs while gathering");
            _allowSiegeCampTreasure = GUILayout.Toggle(_allowSiegeCampTreasure, "Allow usage of siege camp treasures");
            _skipRedAndBlackZones = GUILayout.Toggle(_skipRedAndBlackZones, "Skip red and black zones for traveling");
            UpdateESP(GUILayout.Toggle(_showESP, "Show ESP"));
        }

        void UpdateESP(bool newValue)
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

        void DragGatheringUI_Sliders()
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

        void DrawGatheringUI_SelectionGrids()
        {
            GUILayout.Label(_labelSelectedCity);
            _selectedTownClusterIndex = GUILayout.SelectionGrid(_selectedTownClusterIndex, _labelsTownClusterNames, 4);

            GUILayout.Label(_labelSelectedMinimumTier);
            _selectedMininumTierIndex = GUILayout.SelectionGrid(_selectedMininumTierIndex, _labelsTierNames, _labelsTierNames.Length);
        }

        void DrawGatheringUI_AutocompleteSelectedCluster()
        {
            if (_selectedGatherCluster.Length >= 3
                && _lastSelectedGatherClusterLength <= _selectedGatherCluster.Length
                && _selectedGatherCluster != _lastSelectedGatherCluster)
            {
                string[] clusterNames = GameGui.Instance.WorldMap.GetClusters().Values.Select(x => ((ClusterDescriptor)x.Info).GetName()).ToArray();
                string autoComplete = Array.Find(clusterNames, x => (x.ToLower().StartsWith(_selectedGatherCluster.ToLower())));
                if (!string.IsNullOrEmpty(autoComplete))
                {
                    _selectedGatherCluster = autoComplete;

                    TextEditor editor = (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), GUIUtility.keyboardControl);
                    if (editor != null)
                    {
                        editor.text = _selectedGatherCluster; // Internal text is only updated next frame.
                        editor.SelectTextEnd();
                    }
                }
            }
        }

        void DrawGatheringUI_TextFields()
        {
            GUILayout.Label(_labelSelectGatheringCluster);
            GUILayout.BeginHorizontal();

            _lastSelectedGatherCluster = _selectedGatherCluster;
            _lastSelectedGatherClusterLength = _selectedGatherCluster.Length;
            _selectedGatherCluster = GUILayout.TextField(_selectedGatherCluster);
            DrawGatheringUI_AutocompleteSelectedCluster();

            if (GUILayout.Button(_labelUseCurrentCluster, GUILayout.Width(160)))
            {
                ClusterDescriptor currentClusterInfo = _world.GetCurrentCluster();
                if (currentClusterInfo != null) {
                    _selectedGatherCluster = _world.GetCurrentCluster().GetName();
                }
            }
            GUILayout.EndHorizontal();
        }

        void DrawGatheringUIRight()
        {
            GUILayout.BeginVertical();
            GUILayout.Label(_labelResourcesToGather);
            DrawGatheringUI_GatheringToggles();
            GUILayout.EndVertical();
        }

        void DrawGatheringUI_Buttons()
        {
            if (GUILayout.Button(_labelCloseUI))
                _isUIshown = !_isUIshown;

            DrawRunButton(true);

            if (GUILayout.Button(_labelUnload))
                Core.Unload();
        }

        void DrawGatheringUI_GatheringToggles()
        {
            GUILayout.BeginHorizontal();

            for (int resource = 0; resource < _gatherInformations.Size(); ++resource)
            {
                List<List<bool>> data = _gatherInformations.GetResourceData((ResourceType)resource);
                GUILayout.BeginVertical();

                if (GUILayout.Button(_labelEnableAll))
                {
                    for (int tierIdx = 0; tierIdx < data.Count; ++tierIdx)
                    {
                        Tier tier = (Tier)(tierIdx + 1);
                        for (int ench = 0; ench < data[tierIdx].Count; ++ench)
                        {
                            _gatherInformations.Enable((ResourceType)resource, tier, (EnchantmentLevel)ench,
                                (Tier)tierIdx >= SelectedMinimumTier);
                        }
                    }
                }

                if (GUILayout.Button(_labelDisableAll))
                {
                    for (int tierIdx = 0; tierIdx < data.Count; ++tierIdx)
                    {
                        Tier tier = (Tier)(tierIdx + 1);
                        for (int ench = 0; ench < data[tierIdx].Count; ++ench)
                        {
                            _gatherInformations.Enable((ResourceType)resource, tier, (EnchantmentLevel)ench, false);
                        }
                    }
                }

                for (int tierIdx = 0; tierIdx < data.Count; ++tierIdx)
                {
                    Tier tier = (Tier)(tierIdx + 1);
                    for (int ench = 0; ench < data[tierIdx].Count; ++ench)
                    {
                        if ((Tier)tierIdx < SelectedMinimumTier)
                        {
                            _gatherInformations.Enable((ResourceType)resource, tier, (EnchantmentLevel)ench, false);
                        }
                        bool enabled = _gatherInformations.IsEnabled((ResourceType)resource, tier, (EnchantmentLevel)ench);
                        enabled = GUILayout.Toggle(enabled, _gatherInformations.GetGUIContent((ResourceType)resource, tier,
                            (EnchantmentLevel)ench));
                        _gatherInformations.Enable((ResourceType)resource, tier, (EnchantmentLevel)ench, enabled);
                    }
                }

                GUILayout.EndVertical();
                GUILayout.Space(SpaceBetweenItems);
            }
            GUILayout.EndHorizontal();
        }

        void DrawRunButton(bool layouted)
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
                GatheringWindowRect = GUILayout.Window(0, GatheringWindowRect, DrawGatheringUIWindow, _labelGatheringUI);
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