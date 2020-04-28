using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Appserver.TextractDocument
{
    public class BoundingBox
    {
        public BoundingBox(JToken block) => (Width, Height, Left, Top) 
            = (block["Width"].ToObject<float>(), 
               block["Height"].ToObject<float>(),
               block["Left"].ToObject<float>(),
               block["Top"].ToObject<float>()
            );
        public float Width;
        public float Height;
        public float Left;
        public float Top;
    }
}
