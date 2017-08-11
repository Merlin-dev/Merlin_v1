using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Merlin.API;
using Stateless;
using UnityEngine;
using SpellCategory = gz.SpellCategory;
using SpellTarget = gz.SpellTarget;

namespace Merlin.Profiles {
	class Combat {

		private readonly LocalPlayerCharacterView _player;
		private readonly List<SpecialMob> _mobList;

		private StateMachine<State, Trigger> _state;

		private float elapsedSeconds;
		private float secondCount;

		public Combat(LocalPlayerCharacterView player) {
			_player = player;
			_mobList = new List<SpecialMob>();

			_state = new StateMachine<State, Trigger>(State.Idle);
			_state.Configure(State.Idle)
				.Permit(Trigger.Died, State.Respawn)
				.Permit(Trigger.Recover, State.Recover)
				.Permit(Trigger.EncounteredAttacker, State.Combat);

			_state.Configure(State.Combat)
				.Permit(Trigger.Finished, State.Idle)
				// .Permit(Trigger.LowHealth, State.Flee) Not working for now.
				.Permit(Trigger.Died, State.Respawn);

			_state.Configure(State.Respawn)
				.Permit(Trigger.Finished, State.Idle);

			_state.Configure(State.Flee)
				.Permit(Trigger.Died, State.Respawn)
				.Permit(Trigger.Finished, State.Recover);

			_state.Configure(State.Recover)
				.Permit(Trigger.EncounteredAttacker, State.Combat)
				.Permit(Trigger.Died, State.Respawn)
				.Permit(Trigger.Finished, State.Idle);

		}

		public void AddMob(string name) {
			_mobList.Add(new SpecialMob(name));
		}

		private void AddSpellToMob(SpecialMob mob, DangerousSpell spell) => mob.AddSpell(spell);

		private SpecialMob GetMobByName(string mobName) {
			return _mobList.FirstOrDefault(m => m.GetName().Equals(mobName));
		}

		public void AddSpellToMob(string mobName, SpellCategory category, SpellTarget target, Evade evade, string spellName = null) {
			DangerousSpell spell = new DangerousSpell(target, category, evade, spellName);
			var mob = GetMobByName(mobName);
			if (mob != null)
				AddSpellToMob(mob, spell);
		}

		private bool IsSpecialMob(FightingObjectView target, out Evade evade) {
			evade = Evade.Tank;
			if (!target.IsCasting()) return false;

			var targetName = target.name;
			var spellName = target.GetSpellCasted().d6;
			var spellTarget = target.GetSpellCasted().d1;
			var spellCategory = target.GetSpellCasted().d4;

			var mob = _mobList.Find(s => s.GetName().Equals(targetName));
			var mobSpell = mob.GetSpell(spellName, spellCategory, spellTarget);
			if (mobSpell == null) return false;
			evade = mobSpell.GetEvadeMethod();
			return true;
		}

		public State GetState() => _state.State;

		public void Update() {
			var curTime = Stopwatch.GetTimestamp() / Stopwatch.Frequency; // Gets seconds
			elapsedSeconds = secondCount - curTime;
			secondCount = curTime;

			// Update based on state
			// Do nothing while idle.
			switch (_state.State) {
				case State.Idle:
					Idle();
					break;
				case State.Combat:
					Fight();
					break;
				case State.Respawn:
					Respawn();
					break;
				case State.Recover:
					Recover();
					break;
				case State.Flee:
					Flee();
					break;
			}
		}

		public bool HandleAttackers() {
			if (_player.IsUnderAttack(out FightingObjectView attacker)) {
				_player.CreateTextEffect("[Attacked]");
				_state.Fire(Trigger.EncounteredAttacker);
				return true;
			}

			return false;
		}

		private void Idle() {
			if (_player.IsUnderAttack(out FightingObjectView attacker)) {
				Core.Log("[Combat] Attacked");
				elapsedSeconds = 0;
				_state.Fire(Trigger.EncounteredAttacker);
				return;
			}

			if (_player.GetHealth() <= 0) {
				Core.Log("[Combat] Player died");
				_state.Fire(Trigger.Died);
				return;
			}

			if (_player.GetHealth() <= _player.GetMaxHealth() * 0.5f) {
				Core.Log("[Combat] Recovering");
				_state.Fire(Trigger.Recover);
				return;
			}
		}

		private void Fight() {
			var attackTimer = _player.GetAttackDelay().p();
			var spells = _player.GetSpells().Ready()
				.Ignore("ESCAPE_DUNGEON").Ignore("PLAYER_COUPDEGRACE")
				.Ignore("AMBUSH").Ignore("OUTOFCOMBATHEAL");

			var attackTarget = _player.GetAttackTarget();

			Evade evade;
			if (attackTarget != null && IsSpecialMob(attackTarget, out evade)) {
				_player.CreateTextEffect("[Dodging]");
				Dodge(attackTarget, evade);
				return;
			}

			if (attackTarget != null && elapsedSeconds > attackTimer / 1000f) {
				var selfBuffSpells = spells.Target(SpellTarget.Self).Category(SpellCategory.Damage);
				if (selfBuffSpells.Any() && !_player.IsCastingSpell()) {
					_player.CreateTextEffect("[Casting Buff Spell]");
					_player.CastOnSelf(selfBuffSpells.FirstOrDefault().SpellSlot);
					elapsedSeconds = 0;
					return;
				}

				var enemyBuffSpells = spells.Target(SpellTarget.Enemy).Category(SpellCategory.Buff);
				if (enemyBuffSpells.Any() && !_player.IsCastingSpell()) {
					_player.CreateTextEffect("[Casting Damage Spell]");
					_player.CastOn(enemyBuffSpells.FirstOrDefault().SpellSlot, attackTarget);
					elapsedSeconds = 0;
					return;
				}

				var enemyCCSpells = spells.Target(SpellTarget.Enemy).Category(SpellCategory.CrowdControl);
				if (enemyCCSpells.Any() && !_player.IsCastingSpell()) {
					_player.CreateTextEffect("[Casting CrowdControl Spell]");
					_player.CastOn(enemyCCSpells.FirstOrDefault().SpellSlot, attackTarget);
					elapsedSeconds = 0;
					return;
				}

				/* No Longer Working
				var groundDamageSpells = spells.Target(gy.SpellTarget.Ground).Category(gy.SpellCategory.Damage);
				if (groundDamageSpells.Any()) {
					player.CreateTextEffect("[Casting Ground Spell]");
					player.CastAt(groundDamageSpells.FirstOrDefault().SpellSlot, attackTarget.transform.position);
					return;
				} */
			}


			if (_player.IsUnderAttack(out FightingObjectView attacker)) {
				_player.SetSelectedObject(attacker);
				_player.AttackSelectedObject();
				return;
			}

			if (_player.GetHealth() <= (_player.GetMaxHealth() * 0.1f)) {
				_state.Fire(Trigger.LowHealth);
				return;
			}

			if (_player.IsCasting())
				return;

			Core.Log("[Expedition] Continuing.");
			_state.Fire(Trigger.Finished);
		}

		private void Dodge(FightingObjectView attackTarget, Evade evadeMethod) {
			Vector3 movePosition;
			switch (evadeMethod) {
				case Evade.Behind:
					movePosition = (attackTarget.transform.position - attackTarget.transform.forward * 3);
					_player.RequestMove(movePosition);
					break;
				case Evade.Left:
					movePosition = (attackTarget.transform.position - attackTarget.transform.right * 3);
					_player.RequestMove(movePosition);
					break;
				case Evade.Defensive:
					// Not implemented yet.
					break;
				case Evade.Tank:
				default:
					break;
			}
		}

		private void Recover() {
			var recoverySpell = _player.GetSpells().Slot(SpellSlotIndex.Armor).FirstOrDefault();

			if (_player.IsUnderAttack(out FightingObjectView attacker)) {
				Core.Log("Attacked");
				_state.Fire(Trigger.EncounteredAttacker);
				return;
			}

			if (recoverySpell != null && recoverySpell.Name.Equals("OUTOFCOMBATHEAL") && recoverySpell.IsReady) {
				_player.CastOnSelf(SpellSlotIndex.Armor);
			}

			if (_player.GetHealth() <= 0)
				_state.Fire(Trigger.Died);

			if (_player.GetHealth() > _player.GetMaxHealth() * 0.75f)
				_state.Fire(Trigger.Finished);
		}

		private void Flee() {
			if (_player.GetHealth() <= 0) {
				_state.Fire(Trigger.Died);
				return;
			}

			/* Not Yet Implemented
			if (_player.IsInCombat())
				path.Flee();
			else
				_state.Fire(Trigger.Finished);
				*/
		}

		private void Respawn() {
			var isRespawnShowing = GameGui.Instance.RespawnGui.ExpeditionStart.isActiveAndEnabled;
			if (isRespawnShowing)
				_player.OnRespawn();
		}

		private class DangerousSpell {
			private readonly string name;
			private readonly Evade evadeMethod;
			private readonly SpellTarget target;
			private readonly SpellCategory category;

			// Name should be the FightingObjectView.GetSpellCasted().d6
			public DangerousSpell(SpellTarget target, SpellCategory category, Evade evadeMethod, string spellName) {
				this.name = spellName;
				this.target = target;
				this.category = category;
				this.evadeMethod = evadeMethod;
			}

			public Evade GetEvadeMethod() => evadeMethod;
			public bool IsNamed() => !string.IsNullOrEmpty(name);
			public string GetName() => name;
			public SpellTarget GetTarget() => target;
			public SpellCategory GetCategory() => category;
		}

		private class SpecialMob {

			private readonly string name;
			public string GetName() => name;

			private List<DangerousSpell> dangerousSpells;

			public SpecialMob(string name, params DangerousSpell[] dSpells) {
				this.name = name;
				this.dangerousSpells = new List<DangerousSpell>();

				foreach (var spell in dSpells)
					dangerousSpells.Add(spell);
			}

			public List<DangerousSpell> GetDangerousSpells() => dangerousSpells;

			public DangerousSpell GetSpell(string spellName, SpellCategory category, SpellTarget target) {
				foreach (var spell in dangerousSpells) {
					if (!spell.IsNamed()) continue;
					if (spell.GetCategory().Equals(category) && spell.GetTarget().Equals(target) && spell.GetName().Equals(spellName))
						return spell;
				}

				return null;
			}
			public DangerousSpell GetSpell(SpellCategory category, SpellTarget target) {
				foreach (var spell in dangerousSpells) {
					if (spell.IsNamed()) continue;
					if (spell.GetCategory().Equals(category) && spell.GetTarget().Equals(target))
						return spell;
				}

				return null;
			}

			internal void AddSpell(DangerousSpell spell) => dangerousSpells.Add(spell);
		}

		public enum State {
			Idle,
			Combat,
			Flee,
			Respawn,
			Recover,
			Debug
		}

		public enum Evade {
			Behind,
			Left,
			Defensive,
			Tank
		}

		public enum Trigger {
			EncounteredAttacker,
			LowHealth,
			Recover,
			Died,
			Finished,
		}

		public static string GetStateString(State state) {
			switch (state) {
				case State.Idle:
					return "Idle";
				case State.Combat:
					return "Combat";
				case State.Flee:
					return "Flee";
				case State.Respawn:
					return "Respawn";
				case State.Recover:
					return "Recover";
				case State.Debug:
					return "Debug";
				default:
					return "Unknown";
			}
		}
	}
}
