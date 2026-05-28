namespace ThirdPixelGames.LevelBuilder
{
    using UnityEngine;

    /// <summary>
    /// Contains all the data you need to generate a curLevel
    /// </summary>
    [CreateAssetMenu(fileName = "Level", menuName = "Level Builder/Level", order = 0)]
    public class Level : ScriptableObject
    {
        /// <summary>
        /// The curLevel's name
        /// </summary>
        public string levelName;
        
        /// <summary>
        /// The type of curLevel to generate
        /// </summary>
        public LevelType levelType;

        /// <summary>
        /// The palette used to generate this curLevel
        /// </summary>
        public Palette palette;
        
        /// <summary>
        /// The scale applied to each item in the curLevel
        /// </summary>
        public float scale = 1.0f;

        /// <summary>
        /// The horizontal size of the curLevel
        /// </summary>
        public int sizeX;

        /// <summary>
        /// The vertical size of the curLevel
        /// </summary>
        public int sizeY;

        /// <summary>
        /// The serialized curLevel data
        /// </summary>
        public string data;

        /// <summary>
        /// The serialized curLevel overlay data
        /// </summary>
        public string overlay;

        /// <summary>
        /// The stack Level data count
        /// </summary>
        public int sizeZ;

        /// <summary>
        /// The serialized additional layer data
        /// </summary>
        public string[] additionalLayers;
    }
}