using System.Linq;
using SFML.Graphics;

namespace NetGore.Graphics
{
    /// <summary>
    /// A <see cref="MapGrhEffect"/> that dies after a specified amount of time.
    /// </summary>
    public class MapGrhEffectTimed : MapGrhEffect
    {
        /// <summary>
        /// The time at which this effect will expire.
        /// </summary>
        readonly TickCount _expireTime;

        bool _terminateWhenDoneLooping = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="MapGrhEffectTimed"/> class.
        /// </summary>
        /// <param name="grh">Grh to draw.</param>
        /// <param name="position">Position to draw on the map.</param>
        /// <param name="isForeground">If true, this will be drawn in the foreground layer. If false,
        /// it will be drawn in the background layer.</param>
        /// <param name="life">How long the effect will last in milliseconds.</param>
        public MapGrhEffectTimed(Grh grh, Vector2 position, bool isForeground, int life) : base(grh, position, isForeground)
        {
            _expireTime = (TickCount)(TickCount.Now + life);
        }

        /// <summary>
        /// Forcibly kills the effect.
        /// </summary>
        /// <param name="immediate">When false and the <see cref="Grh"/> is animated, the animation will finish
        /// before terminating.</param>
        public override void Kill(bool immediate)
        {
            if (!immediate && Grh != null && Grh.AnimType != AnimType.None)
            {
                if (Grh.AnimType == AnimType.Loop)
                    Grh.AnimType = AnimType.LoopOnce;

                _terminateWhenDoneLooping = true;

                return;
            }

            base.Kill(immediate);
        }

        /// <summary>
        /// When overridden in the derived class, performs the additional updating that this <see cref="MapGrhEffect"/>
        /// needs to do such as checking if it is time to kill the effect. This method should be overridden instead of
        /// <see cref="MapGrh.Update"/>. This method will not be called after the effect has been killed.
        /// </summary>
        /// <param name="currentTime">Current game time.</param>
        protected override void UpdateEffect(TickCount currentTime)
        {
            if (_terminateWhenDoneLooping && Grh.AnimType == AnimType.None)
            {
                Kill(true);
                return;
            }

            if (TickCount.Now >= _expireTime)
                Kill(true);
        }
    }
}