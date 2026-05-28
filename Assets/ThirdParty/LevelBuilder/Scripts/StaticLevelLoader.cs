namespace ThirdPixelGames.LevelBuilder
{
    using UnityEngine;
    using UnityEngine.Assertions;

    /// <summary>
    /// Load a curLevel that is specified in the editor
    /// </summary>
    public class StaticLevelLoader : MonoBehaviour
    {
        #region Public Variables
        /// <summary>
        /// The curLevel data to load
        /// </summary>
        [Tooltip("The curLevel data to load")]
        public Level level;
        #endregion

        #region Unity Methods
        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        private void Awake()
        {
            Assert.IsNotNull(level);
            LevelLoader.LoadLevel(level);
        }
        #endregion
    }
}