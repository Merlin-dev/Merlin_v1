using Merlin.API;
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
        float? _zoom;
        bool? _globalFog;

        #endregion Fields

        #region Properties

        static Dictionary<string, Texture2D> ColoredTextures { get; } = new Dictionary<string, Texture2D>();

        #endregion Properties

        #region Methods

        void DrawGatheringUIButton()
        {
            var drawRect = new Rect((Screen.width / 2) - 20, 0, 100, 20);

            if (GUI.Button(drawRect, "Gathering UI"))
                _isUIshown = true;
        }

        void DrawGatheringUI()
        {
            var position = new Vector2((Screen.width / 2) - 400, (Screen.height / 2) - 300);
            var size = new Vector2(800, 600);
            var drawRect = new Rect(position, size);
            var rectTexture = GetColoredTexture("UI", (int)drawRect.width, (int)drawRect.height, Color.grey);

            GUILayout.BeginArea(drawRect, rectTexture);

            DrawGatheringUI_MainButtons();
            DrawGatheringUI_DebugToggles();
            DrawGatheringUI_SelectionGrids();
            DrawGatheringUI_GatheringToggles();

            GUILayout.EndArea();
        }

        void DrawGatheringUI_MainButtons()
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Close Gathering UI"))
                _isUIshown = false;

            if (GUILayout.Button(_isRunning ? "Stop Gathering" : "Start Gathering"))
            {
                _isRunning = !_isRunning;
                if (_isRunning)
                {
                    _localPlayerCharacterView.CreateTextEffect("[Restart]");
                    _state.Fire(Trigger.Restart);
                }

                if (!_zoom.HasValue)
                    _zoom = Client.Zoom;
                if (!_globalFog.HasValue)
                    _globalFog = Client.GlobalFog;

                Client.Zoom = _isRunning ? 130f : _zoom.Value;
                Client.GlobalFog = _isRunning ? false : _globalFog.Value;
            }

            if (GUILayout.Button("Unload Merlin"))
                Core.Unload();
            GUILayout.EndHorizontal();
        }

        void DrawGatheringUI_DebugToggles()
        {
            _showErrors = GUILayout.Toggle(_showErrors, "Show errors as messages");
        }

        void DrawGatheringUI_SelectionGrids()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Selected Town:");
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
                    {
                        var format = string.Format("{0} {1}.{2}", info.ResourceType.ToString(), info.Tier.ToString(), (int)info.EnchantmentLevel);
                        _gatherInformations[info] = GUILayout.Toggle(_gatherInformations[info], format);
                    }
                }
                GUILayout.EndVertical();
            }
            GUILayout.EndHorizontal();
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