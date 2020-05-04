using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Appserver.TextractDocument
{
    /// <summary>
    /// The BoundingBox has the two lengths rom the top left corner
    /// and the coordinate for the top left corner.
    /// </summary>
    public class BoundingBox
    {
        /// Constructors
        public BoundingBox(JToken block) => (Width, Height, Left, Top) 
            = (block["Width"].ToObject<float>(), 
               block["Height"].ToObject<float>(),
               block["Left"].ToObject<float>(),
               block["Top"].ToObject<float>()
            );

        /// Properties
        public float Width;
        public float Height;
        public float Left;
        public float Top;
    }
}
