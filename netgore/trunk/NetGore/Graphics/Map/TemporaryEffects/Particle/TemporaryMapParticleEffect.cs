using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using log4net;
using NetGore.Graphics.ParticleEngine;

namespace NetGore.Graphics
{
    /// <summary>
    /// A <see cref="ITemporaryMapEffect"/> for a <see cref="ParticleEmitter"/>. Simply displays a <see cref="ParticleEmitter"/> at
    /// for a brief amount of time. Derived classes can override some methods to provide more advanced operations.
    /// </summary>
    public class TemporaryMapParticleEffect : ITemporaryMapEffect
    {
        static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        readonly ParticleEmitter _emitter;
        readonly bool _isForeground;

        bool _isAlive = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="TemporaryMapParticleEffect"/> class.
        /// </summary>
        /// <param name="emitter">The <see cref="ParticleEmitter"/>.</param>
        /// <param name="isForeground">If true, this will be drawn in the foreground layer. If false,
        /// it will be drawn in the background layer.</param>
        public TemporaryMapParticleEffect(ParticleEmitter emitter, bool isForeground)
        {
            _isForeground = isForeground;
            _emitter = emitter;
        }

        /// <summary>
        /// Gets or sets if the effect will be killed automatically if the <see cref="Emitter"/> runs out of live particles.
        /// Default value is false.
        /// </summary>
        protected bool AutoKillWhenNoParticles { get; set; }

        /// <summary>
        /// Gets the <see cref="ParticleEmitter"/> used by this <see cref="TemporaryMapParticleEffect"/>.
        /// </summary>
        protected ParticleEmitter Emitter
        {
            get { return _emitter; }
        }

        /// <summary>
        /// When overridden in the derived class, performs the additional updating that this <see cref="TemporaryMapParticleEffect"/>
        /// needs to do. This method will not be called after the effect has been killed.
        /// </summary>
        /// <param name="currentTime">Current game time.</param>
        protected virtual void UpdateEffect(TickCount currentTime)
        {
        }

        #region ITemporaryMapEffect Members

        /// <summary>
        /// Notifies listeners when this <see cref="ITemporaryMapEffect"/> has died. This is only raised once per
        /// <see cref="ITemporaryMapEffect"/>, and is raised when <see cref="ITemporaryMapEffect.IsAlive"/> is set to false.
        /// </summary>
        public event TemporaryMapEffectDiedHandler Died;

        /// <summary>
        /// Gets if this map effect is still alive. When false, it will be removed from the map. Once set to false, this
        /// value will remain false.
        /// </summary>
        public bool IsAlive
        {
            get { return _isAlive; }
        }

        /// <summary>
        /// Forcibly kills the effect.
        /// </summary>
        /// <param name="immediate">If true, the emitter will stop emitting and existing particles will be given time
        /// to expire.</param>
        public void Kill(bool immediate)
        {
            if (!IsAlive)
                return;

            _emitter.Kill();

            if (!immediate)
                return;

            _isAlive = false;

            if (Died != null)
                Died(this);
        }

        /// <summary>
        /// Gets if the <see cref="ITemporaryMapEffect"/> is in the foreground. If true, it will be drawn after the
        /// <see cref="MapRenderLayer.SpriteForeground"/> layer. If false, it will be drawn after the
        /// <see cref="MapRenderLayer.SpriteBackground"/> layer.
        /// </summary>
        public bool IsForeground
        {
            get { return _isForeground; }
        }

        /// <summary>
        /// Makes the object draw itself.
        /// </summary>
        /// <param name="sb"><see cref="ISpriteBatch"/> the object can use to draw itself with.</param>
        public void Draw(ISpriteBatch sb)
        {
            if (!IsAlive)
                return;

            _emitter.Draw(sb);
        }

        /// <summary>
        /// Updates the map effect.
        /// </summary>
        /// <param name="currentTime">The current time.</param>
        public void Update(TickCount currentTime)
        {
            if (!IsAlive)
                return;

            // Check if the effect died off
            if (_emitter.IsExpired)
            {
                Kill(true);
                return;
            }

            // Update the emitter
            _emitter.Update(currentTime);

            // Allow for the derived class to update its own logic
            UpdateEffect(currentTime);
        }

        #endregion
    }
}