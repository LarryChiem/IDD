using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Appserver.TextractDocument
{
    public class Polygon
    {
        /*******************************************************************************
        /// Properties
        *******************************************************************************/
        public Coordinate bottomLeft;
        public Coordinate bottomRight;
        public Coordinate topRight;
        public Coordinate topLeft;
    }
}
