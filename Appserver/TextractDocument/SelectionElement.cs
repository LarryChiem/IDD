using System;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Appserver.TextractDocument
{
    public class SelectionElement: Block
    {
        public SelectionElement(JToken block)
        {
            _geometry = new Geometry(block["Geometry"]);
            _Id = block["Id"].ToString();
            Confidence = block["Confidence"].ToObject<float>();
            _page = block["Page"].ToObject<int>();

            switch(block["SelectionStatus"].ToString())
            {
                case "NOT_SELECTED":
                    SelectionStatus = false;
                    break;
                case "SELECTED":
                    SelectionStatus = true;
                    break;
            }
            
            try
            {
                var children = block["Relationships"].ToList<JToken>()[0]["Ids"].ToList<JToken>();

                foreach (var child in children)
                {
                    _childIds.Add(child.ToString());
                }
            }
            catch (System.ArgumentNullException e)
            {

            }
        }
        public override Appserver.TextractDocument.BlockType GetBlockType()
            => Appserver.TextractDocument.BlockType.SELECTION_ELEMENT;
        public override Geometry GetGeometry() => _geometry;
        public override string GetId() => _Id;
        public override List<Block> GetRelationships() => _children;
        public override int GetPage() => _page;
        public override float GetConfidence() => Confidence;

        ////////////////////////
        /// Properties of a Word
        ////////////////////////
        ///

        private float Confidence;
        private bool SelectionStatus;


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
            Console.WriteLine(String.Format("Selection ID: {0}",_Id));
            foreach( var child in _children)
            {
                Console.WriteLine(String.Format("Selection Block Child: {0}",child.GetBlockType()));
            }
        }
    }
}
