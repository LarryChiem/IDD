using Newtonsoft.Json.Linq;
using System.Linq;

namespace Appserver.TextractDocument
{
    /// <summary>
    /// The Geometry is the section of the document which the block belongs to and holds the
    /// coordinates of the bounding box. The Bounding box holds the top left coordinate and
    /// for convenience also the width and height of the box.
    /// 
    /// Coordinates found here are in normalized form, so is the bounding box.
    /// 0,0 is the upper left corner of the Page
    /// </summary>
    /// 
    public class Geometry
    {
        /// Constructors
        public Geometry(JToken block)
        {
            box = new BoundingBox(block["BoundingBox"]);
            var poly = block["Polygon"].ToList();

            polygon = new Polygon {
                bottomLeft = new Coordinate(poly[0]),
                bottomRight = new Coordinate(poly[1]),
                topRight = new Coordinate(poly[2]),
                topLeft = new Coordinate(poly[3])
            };

        }

        /// Properties
        public BoundingBox box;
        public Polygon polygon;
    }
}
