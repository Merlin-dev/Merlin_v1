using Merlin.API.Direct;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Merlin.Profiles.Gatherer
{
    public sealed partial class Gatherer
    {
        private const int BANKING_PECTENTAGE = 99;

        private SimulationObjectView _currentTarget;

        List<HarvestableObjectView> targets = new List<HarvestableObjectView> { };

        private void Search()
        {
            targets.Clear();

            targets = _client.GetEntities<HarvestableObjectView>((s) => { return true; });

            if (_localPlayerCharacterView.IsUnderAttack(out FightingObjectView attacker)){
                //Switch to Combat state
                return;
            }

            if (_localPlayerCharacterView.GetLoadPercent() > BANKING_PECTENTAGE)
            {
                //Switch to Banking state
                return;
            }

            if (_currentTarget != null)
            {
                Blacklist(_currentTarget, TimeSpan.FromMinutes(0.5));
                _currentTarget = null;

                _localPlayerCharacterView.CreateTextEffectTimed("TODO: Clear _harvestPathRequest here");

                return;
            }
        }

        static Material lineMaterial;
        static void CreateLineMaterial()
        {
            if (!lineMaterial)
            {
                // Unity has a built-in shader that is useful for drawing
                // simple colored things.
                Shader shader = Shader.Find("Hidden/Internal-Colored");
                lineMaterial = new Material(shader);
                lineMaterial.hideFlags = HideFlags.HideAndDontSave;
                // Turn backface culling off
                lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
                lineMaterial.SetInt("_ZTest", (int)UnityEngine.Rendering.CompareFunction.Always);
                // Turn off depth writes
                lineMaterial.SetInt("_ZWrite", 0);
            }
        }

        protected override void OnUI()
        {
            foreach (var item in targets)
            {
                var pos = Camera.main.WorldToScreenPoint(item.transform.position);

                StringBuilder builder = new StringBuilder();

                try
                {
                    hn.b ho = item.GetHarvestableObject().HarvestableObject_Internal.se();

                    builder.AppendLine(ho.a0().ToString());
                    builder.AppendLine(ho.a1().ToString());
                    builder.AppendLine(ho.a3().ToString()); //Guild role
                    builder.AppendLine(ho.a5().ToString());
                    builder.AppendLine(ho.ae().ToString()); //level
                    builder.AppendLine(ho.af().ToString()); //Resource
                    builder.AppendLine(ho.ag().ToString());
                    builder.AppendLine(ho.ah().ToString());
                    builder.AppendLine(ho.ai().ToString());
                    builder.AppendLine(ho.aj().ToString());
                    builder.AppendLine(ho.ak().ToString()); //Tool required
                    builder.AppendLine(ho.al().ToString());
                    builder.AppendLine(ho.am().ToString());
                    builder.AppendLine(ho.an().ToString());
                   // builder.AppendLine(ho.ao().ToString());
                   // builder.AppendLine(ho.ap().ToString());
                    builder.AppendLine(ho.aq().ToString());
                    builder.AppendLine(ho.ar().ToString());
                    builder.AppendLine(ho.@as().ToString());
                    builder.AppendLine(ho.at().ToString());
                    builder.AppendLine(ho.au().ToString()); //Max items
                    float[] arr = ho.av(); //Regen steps
                    foreach (var arritt in arr)
                    {
                        builder.AppendLine("\t" + arritt);
                    }
                    builder.AppendLine(ho.ay().ToString());
                    builder.AppendLine(ho.az().ToString());
                }catch
                {

                }
                GUI.Label(new Rect(pos.x,Screen.height - pos.y, 200,800), builder.ToString());
            }
        }

        protected override void OnCameraPostRender(Camera cam)
        {
            if(cam == Camera.main)
            {
                CreateLineMaterial();
                lineMaterial.SetPass(0);

                GL.Begin(GL.LINES);
                foreach (var item in targets)
                {
                    GL.Color(Color.red);
                    GL.Vertex(_localPlayerCharacterView.transform.position);
                    GL.Vertex(item.transform.position);
                }
                GL.End();
            }
        }

        /*public bool IdentifiedTarget(out SimulationObjectView target)
        {
            var resources = _client.GetEntities<HarvestableObjectView>(ValidateHarvestable);
        }*/
    }
}
