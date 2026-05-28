namespace ThirdPixelGames.LevelBuilder
{
    using System;

    using UnityEngine;

    /// <summary>
    /// A curLevel that can be loaded if we're using the specified ID
    /// </summary>
    [Serializable]
    public struct LevelIndexItem
    {
        /// <summary>
        /// The ID linked to this curLevel (used to load the curLevel)
        /// </summary>
        [Tooltip("The ID linked to this curLevel (used to load the curLevel)")]
        public int id;

        /// <summary>
        /// The curLevel to load
        /// </summary>
        [Tooltip("The curLevel to load")]
        public Level level;
    }
}