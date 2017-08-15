using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Merlin.Profiles.Gatherer
{
    public sealed partial class Gatherer
    {
        private void Search()
        {
            Core.Log("Yo");
            //if (_localPlayerCharacterView.IsUnderAttack(out FightingObjectView attacker)){
                //Switch to Combat state
            //}
            Core.Log("You");
            _localPlayerCharacterView.CreateTextEffectTimed("WP: " + _localPlayerCharacterView.GetCharacter().GetWeightPercentage());
            _localPlayerCharacterView.CreateTextEffectTimed("WP: " + _localPlayerCharacterView.GetCharacter().GetWeightPercentage() * 100f);
            Core.Log("Mg");
        }
    }
}
