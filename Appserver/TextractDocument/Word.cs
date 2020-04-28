using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Appserver.TextractDocument
{
    public class Word: Block
    {
        public Word(JToken block)
        {
            _geometry = new Geometry(block["Geometry"]);
            _Id = block["Id"].ToString();
            Confidence = block["Confidence"].ToObject<float>();
            _page = block["Page"].ToObject<int>();
            Text = block["Text"].ToString();
            try
            {
                var children = block["Relationships"].ToList<JToken>()[0]["Ids"].ToList<JToken>();

                foreach (var child in children)
                {
                    _childIds.Add(child.ToString());
                }
            }
            catch(System.ArgumentNullException e)
            {

            }
        }
        public override Appserver.TextractDocument.BlockType GetBlockType() 
            => Appserver.TextractDocument.BlockType.WORD;
        public override Geometry GetGeometry() => _geometry;
        public override string GetId() => _Id;
        public override List<Block> GetRelationships() => _children;
        public override int GetPage() => _page;
        public override float GetConfidence() => Confidence;

        ////////////////////////
        /// Properties of a Word
        ////////////////////////
        ///

        float Confidence;
        string Text;

        private Geometry _geometry;
        private int _page;
        private string _Id;

        private List<Block> _children = new List<Block>();
        private Dictionary<string, Block> _childMap = new Dictionary<string, Block>();
        private List<string> _childIds = new List<string>();

        private Page _parent;
        public override void SetPage(Page page)
        {
            _parent = page;
        }
        public override void CreateStructure()
        {
        }
        public override void PrintSummary()
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return Text;
        }
    }
}
