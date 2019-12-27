using System.Numerics;

namespace FlatPhysics.Map
{
    public interface IBaseMapObject<TMapTile> : IGuidable
    {
        /// <summary>
        /// Gets or sets the map tile.
        /// </summary>
        /// <value>The map tile.</value>
        TMapTile MapTile { get; set; }

        /// <summary>
        /// Object location
        /// </summary>
        /// <value>The position.</value>
        Vector3 Position { get; }
    }
}