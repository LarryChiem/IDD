using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Appserver.TextractDocument
{
    public class Polygon
    {
        //public Polygon(JToken block) => (bottomLeft, bottomRight, topRight, topLeft)
        //    = ( new Coordinate(block[0]),
        //        new Coordinate(block[1]),
        //        new Coordinate(block[2]),
        //        new Coordinate(block[3])
        //    );
        public Coordinate bottomLeft;
        public Coordinate bottomRight;
        public Coordinate topRight;
        public Coordinate topLeft;
    }
}
