using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace Merlin.Profiles.Gatherer
{
    public partial class Gatherer
    {
        #region Fields

        bool _showErrors;
        bool _isUIshown;
        float _zoom;
        bool _globalFog;
        bool _autoUpdate;
        bool _showESP;

        #endregion Fields

        #region Properties

        static Dictionary<string, Texture2D> ColoredTextures { get; } = new Dictionary<string, Texture2D>();

        string[] TownClusterNames { get { return Enum.GetNames(typeof(TownClusterName)).Select(n => n.Replace("_", " ")).ToArray(); } }

        string[] TierNames { get { return Enum.GetNames(typeof(Tier)).ToArray(); } }

        string SelectedTownCluster { get { return TownClusterNames[_selectedTownClusterIndex]; } }

        Tier SelectedMinimumTier { get { return (Tier)Enum.Parse(typeof(Tier), TierNames[_selectedMininumTierIndex]); } }

        #endregion Properties

        #region Methods

        void DrawGatheringUIButton()
        {
            var drawRect = new Rect((Screen.width / 2) - 20, 0, 100, 20);
            var drawRect2 = new Rect((Screen.width / 2) - 20 + 100, 0, 100, 20);

            if (GUI.Button(drawRect, "Gathering UI"))
                _isUIshown = true;

            if (GUI.Button(drawRect2, _isRunning ? "Stop Gathering" : "Start Gathering"))
                OnRunButton();
        }

        void DrawGatheringUI()
        {
            //GUI.Window(0, GetGuiDrawRect(), DrawGatheringUIWindow, "Gathering UI");

            var drawRect = GetGuiDrawRect();
            var rectTexture = GetColoredTexture("UI", (int)drawRect.width, (int)drawRect.height, Color.grey);

            GUILayout.BeginArea(drawRect, rectTexture);

            DrawGatheringUI_MainButtons();
            DrawGatheringUI_DebugToggles();
            DrawGatheringUI_SelectionGrids();
            DrawGatheringUI_GatheringToggles();
            DrawGatheringUI_ESP();
            DrawGatheringUI_Range();

            GUILayout.EndArea();

            //GUI.DragWindow();
        }

        Rect GetGuiDrawRect()
        {
            var position = new Vector2((Screen.width / 2) - 400, (Screen.height / 2) - 450);
            var size = new Vector2(800, 900);

            return new Rect(position, size);
        }

        void DrawGatheringUIWindow(int windowID)
        {
            var drawRect = GetGuiDrawRect();
            var rectTexture = GetColoredTexture("UI", (int)drawRect.width, (int)drawRect.height, Color.grey);

            GUILayout.BeginArea(drawRect, rectTexture);

            DrawGatheringUI_MainButtons();
            DrawGatheringUI_DebugToggles();
            DrawGatheringUI_SelectionGrids();
            DrawGatheringUI_GatheringToggles();
            DrawGatheringUI_ESP();

            GUILayout.EndArea();

            GUI.DragWindow();
        }

        void DrawGatheringUI_MainButtons()
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Close Gathering UI"))
                _isUIshown = false;

            if (GUILayout.Button(_isRunning ? "Stop Gathering" : "Start Gathering"))
                OnRunButton();

            if (GUILayout.Button("Unload Merlin"))
                Core.Unload();
            GUILayout.EndHorizontal();
        }

        void OnRunButton()
        {
            _isRunning = !_isRunning;
            if (_isRunning)
            {
                if (_selectedGatherCluster == "Unknown" && _world.GetCurrentCluster() != null)
                    _selectedGatherCluster = _world.GetCurrentCluster().GetName();
                _localPlayerCharacterView.CreateTextEffect("[Start]");
                if (_state.CanFire(Trigger.Failure))
                    _state.Fire(Trigger.Failure);
            }

            //_zoom = _isRunning ? 130f : 35f;
            //_globalFog = !_isRunning;
        }

        void DrawGatheringUI_DebugToggles()
        {
            GUILayout.BeginHorizontal();
            _showErrors = GUILayout.Toggle(_showErrors, "Show errors as messages");
            _allowMobHunting = GUILayout.Toggle(_allowMobHunting, "Allow hunting of mobs [experimental! can cause issues]");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            _skipUnrestrictedPvPZones = GUILayout.Toggle(_skipUnrestrictedPvPZones, "Skip unrestricted PvP Zones (pathfinding & gathering)");
            _skipKeeperPacks = GUILayout.Toggle(_skipKeeperPacks, "Skip Keeper Packs (pathfinding & gathering)");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            _autoUpdate = GUILayout.Toggle(_autoUpdate, "Auto Update Zoom & Fog");
            _allowSiegeCampTreasure = GUILayout.Toggle(_allowSiegeCampTreasure, "Allow banking on Siege Camps");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label($"Keeper Skip Range: {_keeperSkipRange}");
            _keeperSkipRange = GUILayout.HorizontalSlider(_keeperSkipRange, 5, 50);
            GUILayout.Label($"Minimum Health for gathering: {_minimumHealthForGathering.ToString("P2")}");
            _minimumHealthForGathering = GUILayout.HorizontalSlider(_minimumHealthForGathering, 0.01f, 1f);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label($"Banking Weight: {_percentageForBanking}");
            _percentageForBanking = Mathf.Round(GUILayout.HorizontalSlider(_percentageForBanking, 1, 400));
            GUILayout.Label($"Siege Camp Treasure Weight: {_percentageForSiegeCampTreasure}");
            _percentageForSiegeCampTreasure = Mathf.Round(GUILayout.HorizontalSlider(_percentageForSiegeCampTreasure, 1, 400));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label($"Camera Zoom: {_zoom}");
            _zoom = GUILayout.HorizontalSlider(_zoom, 1f, 130f);
            _globalFog = GUILayout.Toggle(_globalFog, "Global Fog be enabled?");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Selected Gather Cluster:");
            var currentClusterInfo = _world.GetCurrentCluster() != null ? _world.GetCurrentCluster().GetName() : "Unknown";
            var selectedGatherCluster = string.IsNullOrEmpty(_selectedGatherCluster) ? currentClusterInfo : _selectedGatherCluster;
            _selectedGatherCluster = GUILayout.TextField(selectedGatherCluster);
            GUILayout.EndHorizontal();
        }

        void DrawGatheringUI_SelectionGrids()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Selected Town Cluster:");
            _selectedTownClusterIndex = GUILayout.SelectionGrid(_selectedTownClusterIndex, TownClusterNames, TownClusterNames.Length);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Selected Minimum Gathering Tier:");
            _selectedMininumTierIndex = GUILayout.SelectionGrid(_selectedMininumTierIndex, TierNames, TierNames.Length);
            GUILayout.EndHorizontal();
        }

        void DrawGatheringUI_GatheringToggles()
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
            }
            GUILayout.EndHorizontal();
        }

        void DrawGatheringUI_ESP()
        {
            var oldValue = _showESP;
            _showESP = GUILayout.Toggle(_showESP, "Show ESP");

            if (oldValue != _showESP)
            {
                if (_showESP)
                    gameObject.AddComponent<ESP.ESP>().StartESP(_gatherInformations);
                else if (gameObject.GetComponent<ESP.ESP>() != null)
                    Destroy(gameObject.GetComponent<ESP.ESP>());
            }
        }

        protected override void OnUI()
        {
            if (_isUIshown)
                DrawGatheringUI();
            else
                DrawGatheringUIButton();
        }

        static Texture2D GetColoredTexture(string id, int width, int height, Color color)
        {
            if (ColoredTextures.ContainsKey(id))
                return ColoredTextures[id];

            var pixels = new Color[width * height];
            for (var i = 0; i < pixels.Length; i++)
                pixels[i] = color;

            var result = new Texture2D(width, height);
            result.SetPixels(pixels);
            result.Apply();

            ColoredTextures.Add(id, result);

            return result;
        }

        #endregion Methods
    }
}