using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Merlin.Profiles.Gatherer.Gatherer;
using Albion_Direct;

namespace Merlin.Profiles.ESP
{
    public class ESP : MonoBehaviour
    {
        private class Rendering
        {
            private static Color color_0;
            private static GUIStyle guistyle_0 = new GUIStyle(GUI.skin.label);
            private static Texture2D texture2D_0 = new Texture2D(1, 1);

            public static void DrawSphere (GameObject target, float radius)
            {
                if (target.transform.Find("VisAggroRadius") == null)
                {
                    GameObject sphere = Instantiate(GameObject.CreatePrimitive(PrimitiveType.Sphere), target.transform, false);
                    sphere.name = "VisAggroRadius";
                    Destroy(sphere.GetComponent<Collider>());
                    Renderer renderer = sphere.GetComponent<Renderer>();
                    renderer.material.shader = Shader.Find("Particles/Additive");
                    renderer.material.SetColor("_TintColor", new Color(165, 0, 0, 10));
                    sphere.transform.localScale = (new Vector3(radius, radius, radius));
                }
            }

            public static void DrawProjectorCircle(GameObject target, float radius)
            {
                // This needs Testing we may have to add it as Child gameobject to be able to Rotate the Projector incase its facing the wrong way.
                Projector projector = target.GetComponent<Projector>();
                if (projector == null)
                {
                    projector = target.AddComponent<Projector>();
                    projector.orthographic = true;
                    projector.orthographicSize = radius;
                    //Need a Proper Circle Material, Import embeded or try use std. Blob-Shadow Unity ?
                    //Might aswell find something useful in Albion.
                    //projector.material = Resources.Load("Resources/")

                }
            }

            public static void DrawLine(GameObject target, Vector3 from, Vector3 to, Color color)
            {
                try
                {
                    LineRenderer line = target.GetComponent<LineRenderer>();
                    if (line == null)
                    {
                        line = target.AddComponent<LineRenderer>();
                        line.material = new Material(Shader.Find("Particles/Additive"));
                        line.startWidth = 0.05F;
                        line.endWidth = 0.05F;
                    }
                    line.startColor = color;
                    line.endColor = color;
                    line.SetPosition(0, new Vector3(from.x, from.y + 2, from.z));
                    line.SetPosition(1, new Vector3(to.x, to.y + 1, to.z));
                }
                catch(Exception e)
                {
                    Core.Log("ESP DrawLine Error: " + e);
                }
            }

            public static void BoxRect(Rect rect, Color color)
            {
                try
                {
                    if (color != color_0)
                    {
                        texture2D_0.SetPixel(0, 0, color);
                        texture2D_0.Apply();
                        color_0 = color;
                    }
                    GUI.DrawTexture(rect, texture2D_0);
                }
                catch (Exception e)
                {
                    Core.Log("ESP BoxRect Error: " + e);
                }
            }

            public static void DrawBox(Vector2 pos, Vector2 size, float thick, Color color, Boolean center = false)
            {
                try
                {
                    if (center)
                    {
                        pos.x -= size.x / 2;
                        pos.y -= size.y / 2;
                    }
                    BoxRect(new Rect(pos.x, pos.y, size.x, thick), color);
                    BoxRect(new Rect(pos.x, pos.y, thick, size.y), color);
                    BoxRect(new Rect(pos.x + size.x, pos.y, thick, size.y), color);
                    BoxRect(new Rect(pos.x, pos.y + size.y, size.x + thick, thick), color);
                }
                catch (Exception e)
                {
                    Core.Log("ESP DrawBox Error: " + e);
                }
            }

            public static void DrawHealth(Vector2 pos, float health, bool center = false)
            {
                try
                {
                    if (center)
                    {
                        pos -= new Vector2(26f, 0f);
                    }
                    pos -= new Vector2(0f, 8f);
                    BoxRect(new Rect(pos.x, pos.y, 52f, 5f), Color.black);
                    pos += new Vector2(1f, 1f);
                    Color green = Color.green;
                    if (health <= 60f)
                    {
                        green = Color.yellow;
                    }
                    if (health <= 25f)
                    {
                        green = Color.red;
                    }
                    BoxRect(new Rect(pos.x, pos.y, 0.5f * health, 3f), green);
                }
                catch (Exception e)
                {
                    Core.Log("ESP DrawHealth Error: " + e);
                }
            }

            public static void DrawString(Vector2 pos, string text, Color color, bool center = true, int size = 12, bool outline = true, int lines = 1)
            {
                try
                {
                    guistyle_0.fontSize = size;
                    guistyle_0.normal.textColor = color;
                    GUIContent content = new GUIContent(text);
                    if (center)
                    {
                        pos.x -= guistyle_0.CalcSize(content).x / 2f;
                    }
                    Rect rect = new Rect(pos.x, pos.y, 300f, lines * 25f);
                    if (!outline)
                    {
                        GUI.Label(rect, content, guistyle_0);
                    }
                    else
                    {
                        DrawOutlinedString(rect, text, color);
                    }
                }
                catch (Exception e)
                {
                    Core.Log("ESP DrawString Error: " + e);
                }
            }

            public static void DrawWatermark(string text, Color color, int size = 12)
            {
                try
                {
                    guistyle_0.fontSize = size;
                    guistyle_0.normal.textColor = color;
                    GUIContent content = new GUIContent(text);
                    GUI.Label(new Rect(1, 1, 300f, 25f), content, guistyle_0);
                }
                catch (Exception e)
                {
                    Core.Log("ESP DrawWatermark Error: " + e);
                }
            }

            public static void DrawWatermark2(string text, Color color, int size = 12)
            {
                try
                {
                    guistyle_0.fontSize = size;
                    guistyle_0.normal.textColor = color;
                    GUIContent content = new GUIContent(text);
                    GUI.Label(new Rect(250, 250, 300f, 25f), content, guistyle_0);
                }
                catch (Exception e)
                {
                    Core.Log("ESP DrawWatermark2 Error: " + e);
                }
            }

            public static void DrawOutlinedString(Rect rect, string text, Color color)
            {
                try
                {
                    GUIStyle backupStyle = guistyle_0;
                    guistyle_0.normal.textColor = Color.black;
                    rect.x--;
                    GUI.Label(rect, text, guistyle_0);
                    rect.x += 2;
                    GUI.Label(rect, text, guistyle_0);
                    rect.x--;
                    rect.y--;
                    GUI.Label(rect, text, guistyle_0);
                    rect.y += 2;
                    GUI.Label(rect, text, guistyle_0);
                    rect.y--;
                    guistyle_0.normal.textColor = color;
                    GUI.Label(rect, text, guistyle_0);
                    guistyle_0 = backupStyle;
                }
                catch (Exception e)
                {
                    Core.Log("ESP DrawOutlinedString Error: " + e);
                }
            }
        }

        public static Rect espWindow = new Rect(50f, 150f, 235f, 275f);
        public static Rect debugWindow = new Rect(50f, 450f, 235f, 275f);

        internal static class Resources
        {
            public static Boolean Enabled = true;
            public static Boolean DrawDepleted = true;
            public static float MaxLineDistance = 150;
        }

        internal static class Players
        {
            public static Boolean Enabled = true;
            public static Boolean DrawFriendly = true;
        }

        private Dictionary<GatherInformation, bool> gatherInformations;
        private SimulationObjectView[] resources;
        private LocalPlayerCharacterView localPlayer;
        private PlayerCharacterView[] players;
        private GameManager _client;

        public void StartESP(Dictionary<GatherInformation, bool> gatherInformations)
        {
            this.gatherInformations = gatherInformations;

            StartCoroutine(GetViews());
            StartCoroutine(AggroRadius());
        }

        private void OnDisable()
        {
            StopAllCoroutines();
        }

        private IEnumerator AggroRadius()
        {
            while (true)
            {
                try
                {
                    if (_client.GetState() == GameState.Playing)
                    {
                        foreach (var mob in FindObjectsOfType<MobView>())
                        {
                            float aggroRadius = 0; // Need variable to get actual Aggro Range.
                            if (mob != null && mob.gameObject != null && mob.GetTier() >= 3)
                            {
                                Rendering.DrawSphere(mob.gameObject, aggroRadius != 0 ? aggroRadius : 10);
                            }
                        }
                    }                    
                }
                catch (Exception e)
                {
                    Core.Log("ESP AggroRadius Error: " + e);
                }
                yield return new WaitForSeconds(5);
            }
        }

        private IEnumerator GetViews()
        {
            while (true)
            {
                try
                {
                    localPlayer = FindObjectOfType<LocalPlayerCharacterView>();
                }
                catch (Exception e)
                {
                    Core.Log("ESP Finding localPlayer error: " + e);
                }

                yield return new WaitForSeconds(0.2f);

                try
                {
                    players = FindObjectsOfType<PlayerCharacterView>();
                }
                catch (Exception e)
                {
                    Core.Log("ESP Finding players Error: " + e);
                }

                yield return new WaitForSeconds(1f);

                try
                {
                    var harvestables = FindObjectsOfType<HarvestableObjectView>().Where(view =>
                    {
                        var harvestableObject = view.GetHarvestableObject();

                        var resourceType = harvestableObject.GetResourceType().Value;
                        var tier = (Tier)harvestableObject.GetTier();
                        var enchantmentLevel = (EnchantmentLevel)harvestableObject.GetRareState();

                        var info = new GatherInformation(resourceType, tier, enchantmentLevel);

                        return enchantmentLevel > 0 && gatherInformations[info];
                    }).ToArray();

                    var mobs = FindObjectsOfType<MobView>().Where(view =>
                    {
                        var resourceType = view.GetResourceType();
                        if (!resourceType.HasValue)
                            return false;

                        var tier = (Tier)view.GetTier();
                        var enchantmentLevel = (EnchantmentLevel)view.GetRareState();

                        var info = new GatherInformation(resourceType.Value, tier, enchantmentLevel);

                        return enchantmentLevel > 0 && gatherInformations[info];
                    }).ToArray();

                    resources = harvestables.OfType<SimulationObjectView>().Union(mobs.OfType<SimulationObjectView>()).ToArray();
                }
                catch (Exception e)
                {
                    Core.Log("ESP Finding Harvestable Views Error: " + e);
                }

                yield return new WaitForSeconds(1f);
            }
        }

        private void OnGUI()
        {
            _client = GameManager.GetInstance();

            if (_client.GetState() == GameState.Playing)
            {
                if (resources != null)
                    DrawResourceESPs();

                if (players != null)
                    DrawPlayerESPs();
                DrawFishingESP();
            }
        }
        private void DrawFishingESP()
        {
            if (localPlayer != null)
                Rendering.DrawString(new Vector2(50, 140), "LocalPlayer", Color.green);
            else
                Rendering.DrawString(new Vector2(50, 140), "LocalPlayer", Color.red);
            if (Fishing.fishingManager != null)
                Rendering.DrawString(new Vector2(50, 150), "FishingManager", Color.green);
            else
                Rendering.DrawString(new Vector2(50, 150), "FishingManager", Color.red);
            Rendering.DrawString(new Vector2(50, 160), "Progress: " + Fishing.progress.ToString("F2"), Color.red);
            Rendering.DrawString(new Vector2(50, 170), "Tention: " + Fishing.linetention.ToString("F2"), Color.red);
            Rendering.DrawString(new Vector2(50, 180), "Reeling: " + Fishing.Reeling.ToString(), Color.red);

            if (Fishing.IsFishing)
                Rendering.DrawString(new Vector2(50, 190), "isFishing", Color.green);
            else
                Rendering.DrawString(new Vector2(50, 190), "isFishing", Color.red);

            if (Fishing.IsMinigameRunning)
                Rendering.DrawString(new Vector2(50, 200), "MiniGameRunning", Color.green);
            else
                Rendering.DrawString(new Vector2(50, 200), "MiniGameRunning", Color.red);

            if (Fishing.IsValid)
                Rendering.DrawString(new Vector2(50, 210), "valid", Color.green);
            else
                Rendering.DrawString(new Vector2(50, 210), "valid", Color.red);

            if (Fishing.HaveBite)
                Rendering.DrawString(new Vector2(50, 220), "Bite", Color.green);
            else
                Rendering.DrawString(new Vector2(50, 220), "Bite", Color.red);


            foreach (var x in _client.GetEntities<FishingZoneObjectView>(x => x != null))
            {
                Rendering.DrawLine(x.gameObject, x.transform.position, localPlayer.transform.position, Color.yellow);
            }


        }
        private void DrawResourceESPs()
        {
            var myPos = Camera.main.WorldToScreenPoint(localPlayer.transform.position);

            if (fleePositionUpToDate)
            {
                Rendering.DrawLine(localPlayer.gameObject, localPlayer.transform.position, fleePosition, Color.cyan);
            }

            foreach (var view in resources)
            {
                if (view == null || view.transform == null)
                    continue;

                try
                {
                    var pos = Camera.main.WorldToScreenPoint(view.transform.position);
                    pos.y = Screen.height - (pos.y + 1f);

                    var lineColor = Color.white;

                    var valid = false;
                    var enchantmentLevel = 0;
                    if (view is HarvestableObjectView harvestable)
                    {
                        var harvestableObject = harvestable.GetHarvestableObject();

                        valid = harvestable.CanLoot(localPlayer) && harvestableObject.GetCharges() > 0;

                        enchantmentLevel = harvestableObject.GetRareState();
                    }
                    else if (view is MobView mob)
                    {
                        valid = !mob.IsDead();
                        enchantmentLevel = mob.GetRareState();
                    }

                    if (enchantmentLevel == 1)
                        lineColor = Color.green;
                    else if (enchantmentLevel == 2)
                        lineColor = Color.cyan;
                    else if (enchantmentLevel == 3)
                        lineColor = Color.magenta;

                    if (!valid)
                        lineColor = Color.grey;

                    Rendering.DrawBox(pos, new Vector2(100, 100), 2f, lineColor, true);
                    if (Vector3.Distance(view.transform.position, localPlayer.transform.position) < Resources.MaxLineDistance)
                        Rendering.DrawLine(view.gameObject, view.transform.position, localPlayer.transform.position, lineColor);
                    else
                    {
                        var l = view.gameObject.GetComponent<LineRenderer>();
                        if (l != null)
                            Destroy(l);
                    }
                }
                catch (Exception e)
                {
                    Core.Log("ESP DrawResourceESPs Error: " + e);
                }
            }
        }

        private void DrawPlayerESPs()
        {
            Rendering.DrawString(new Vector2(Screen.width - 150, Screen.height - 320), "count: " + (players.Count() - 1), Color.white, true, 20);

            foreach (var player in players)
            {
                if (player == null || player.transform == null)
                    continue;

                try
                {
                    if (player == localPlayer)
                        continue;
                        
                    var pos = Camera.main.WorldToScreenPoint(player.transform.position);
                    pos.y = Screen.height - (pos.y + 1f);
                        
                    if (player.PlayerCharacter.j3() != localPlayer.PlayerCharacter.j3())
                    {
                        Rendering.DrawLine(player.gameObject, player.transform.position, localPlayer.transform.position, Color.red);
                        Rendering.DrawBox(pos, new Vector2(100, 100), 2f, Color.red, true);
                    }
                    else if (Players.DrawFriendly)
                        Rendering.DrawLine(player.gameObject, player.transform.position, localPlayer.transform.position, Color.yellow);
                }
                catch (Exception e)
                {
                    Core.Log("ESP DrawPlayerESPs Error: " + e);
                }
            }
        }
    }
}