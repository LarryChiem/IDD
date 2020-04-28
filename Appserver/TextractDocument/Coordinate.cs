using Newtonsoft.Json.Linq;

namespace Appserver.TextractDocument
{
    public class Coordinate
    {
        public Coordinate(JToken block) => (X, Y) = (block["X"].ToObject<float>(), block["Y"].ToObject<float>());
        public float X;
        public float Y;
    }
}
