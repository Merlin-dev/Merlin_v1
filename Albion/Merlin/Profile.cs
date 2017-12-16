using Albion_Direct;
using System;
using UnityEngine;

namespace Merlin
{
    public abstract class Profile : MonoBehaviour
    {
        #region Fields

        protected GameManager _client;
        protected ObjectManager _world;
        protected LandscapeManager _landscape;
        protected LocalPlayerCharacterView _localPlayerCharacterView;
        protected CollisionManager _collision;

        private DateTime _nextUpdate;

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
            _client = GameManager.GetInstance();
            _world = ObjectManager.GetInstance();
            _landscape = _client.GetLandscapeManager();
            _collision = _world.GetCollisionManager();
            _localPlayerCharacterView = _client.GetLocalPlayerCharacterView();
            _nextUpdate = DateTime.Now;
        }

        private void Awake()
        {
        }

        private void Start()
        {
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
            HotKey();

            _client = GameManager.GetInstance();
            _world = ObjectManager.GetInstance();
            _landscape = _client.GetLandscapeManager();
            _localPlayerCharacterView = _client.GetLocalPlayerCharacterView();
            OnUpdate();
        }

        /// <summary>
        /// Called when the GUI is rendered.
        /// </summary>
        private void OnGUI()
        {
            OnUI();
        }

        /// <summary>
        /// Called when this instance is started.
        /// </summary>
        protected abstract void OnStart();

        /// <summary>
        /// Called when this instance is stopped.
        /// </summary>
        protected abstract void OnStop();

        protected abstract void OnUpdate();

        protected abstract void HotKey();

        protected virtual void OnUI()
        {
        }
        #endregion Methods
    }
}