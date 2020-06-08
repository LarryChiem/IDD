using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Appserver.TextractDocument
{
    /// <summary>
    /// This family of classes takes a Textract Response from Json and turns it into a well structured
    /// class hierarchy.
    /// </summary>
    public abstract class AbstractTextractObject
    {
        public abstract void FromJson(JToken token);
    }
}
