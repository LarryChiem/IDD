using Newtonsoft.Json.Linq;

namespace Appserver.TextractDocument
{
    /// <summary>
    /// The Coordinate class is used in the Geometry class and holds the
    /// (x,y) coordinate.
    /// </summary>
    public class Coordinate
    {
        /// Constructors
        public Coordinate(JToken block) => (X, Y) = (block["X"].ToObject<float>(), block["Y"].ToObject<float>());

        /// Properties
        public float X;
        public float Y;
    }
}
