using Merlin.Profiles.Gatherer;
using System;
using UnityEngine;

namespace Merlin
{
    public class Core
    {
        public static GameObject _coreObject;

        private static Profile _activeProfile;

        public static LineRenderer lineRenderer;

        public static void Load()
        {
            _coreObject = new GameObject();
            lineRenderer = _coreObject.AddComponent<LineRenderer>();

            {
                // Unity has a built-in shader that is useful for drawing
                // simple colored things.
                Shader shader = Shader.Find("Hidden/Internal-Colored");
                Material lineMaterial = new Material(shader);
                lineMaterial.hideFlags = HideFlags.HideAndDontSave;
                // Turn backface culling off
                lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
                lineMaterial.SetInt("_ZTest", (int)UnityEngine.Rendering.CompareFunction.Always);
                // Turn off depth writes
                lineMaterial.SetInt("_ZWrite", 0);
                lineMaterial.color = Color.yellow;
                lineRenderer.material = lineMaterial;
            }

            _coreObject.AddComponent<Console>().enabled = true;
            var gatherer = _coreObject.AddComponent<Gatherer>();
            Activate(gatherer);

            UnityEngine.Object.DontDestroyOnLoad(_coreObject);
        }

        public static void Unload()
        {
            if (_activeProfile != null)
                _activeProfile.enabled = false;

            _activeProfile = null;

            UnityEngine.Object.Destroy(_coreObject);

            _coreObject = null;
        }

        public static void Log(string message)
        {
            Debug.Log($"[{DateTime.Now}] {message}");
        }

        public static void Log(Exception e)
        {
            Debug.LogException(e);
        }

        public static void Activate(Profile profile)
        {
            if (_activeProfile != null)
                _activeProfile.enabled = false;

            _activeProfile = profile;
            _activeProfile.enabled = true;
        }

        public static void Deactivate()
        {
            if (_activeProfile != null)
                _activeProfile.enabled = false;

            _activeProfile = null;
        }

        public static void DeactivateAll()
        {
            var profiles = _coreObject.GetComponents<Profile>();

            foreach (var profile in profiles)
                profile.enabled = false;

            _activeProfile = null;
        }
    }

    public class UnloadButton : MonoBehaviour
    {
        private void OnGUI()
        {
            if (GUI.Button(new Rect(Screen.width / 2f - 50, 10, 100, 30), "Unload"))
            {
                Core.Unload();
            }
        }
    }
}