using Merlin.API;
using System;
using UnityEngine;

namespace Merlin
{
    public abstract class Profile : MonoBehaviour
    {
        #region Static

        public static TimeSpan UpdateDelay = TimeSpan.FromSeconds(0.1d);

        #endregion Static

        #region Fields

        protected Client _client;
        protected World _world;
        protected Landscape _landscape;
        protected LocalPlayerCharacterView _localPlayerCharacterView;

        private DateTime _nextUpdate;
        private bool refresh;
        #endregion Fields

        #region Properties and Events

        public abstract string Name { get; }

        #endregion Properties and Events

        #region Methods

        /// <summary>
        /// Called when this instance is enabled.
        /// </summary>
        private void OnEnable()
        {
            _client = Client.Instance;
            _world = World.Instance;
            _landscape = Landscape.Instance;
            _localPlayerCharacterView = _client.LocalPlayerCharacter;
            _nextUpdate = DateTime.Now;
        }

        private void Awake()
        {
        }

        private void Start()
        {
            if (_client.State == GameState.Playing)
            {
                Client.Zoom = 130f;
                Client.GlobalFog = false;
            }
            OnStart();
        }

        private void Stop()
        {
        }

        /// <summary>
        /// Called when this instance is disabled.
        /// </summary>
        private void OnDisable()
        {
            OnStop();

            _client = null;
        }

        /// <summary>
        /// Called when this instance is updated.
        /// </summary>
        private void Update()
        {
            if (_client.State == GameState.Playing)
            {
                if (refresh)
                {
                    _client = Client.Instance;
                    _world = World.Instance;
                    _landscape = Landscape.Instance;
                    _localPlayerCharacterView = _client.LocalPlayerCharacter;
                    refresh = false;
                    Client.Zoom = 130f;
                    Client.GlobalFog = false;
                }
                if (DateTime.Now < _nextUpdate)
                    return;

                OnUpdate();

                _nextUpdate = DateTime.Now + UpdateDelay;
            }
            else
            {
                refresh = true;
            }
        }

        /// <summary>
        /// Called when the GUI is rendered.
        /// </summary>
        private void OnGUI()
        {
        }

        /// <summary>
        /// Called when this instance is started.
        /// </summary>
        protected virtual void OnStart()
        {
        }

        /// <summary>
        /// Called when this instance is stopped.
        /// </summary>
        protected virtual void OnStop()
        {
        }

        protected virtual void OnUpdate()
        {
        }

        #endregion Methods
    }

    public class RestartInstruction : CustomYieldInstruction
    {
        public override bool keepWaiting => false;
    }
}