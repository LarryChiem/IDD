using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Appserver.TextractDocument
{
    public class Cell: Block
    {
        public Cell(JToken block)
        {
            _geometry = new Geometry(block["Geometry"]);
            _Id = block["Id"].ToString();
            Confidence = block["Confidence"].ToObject<float>();
            _page = block["Page"].ToObject<int>();
            RowIndex = block["RowIndex"].ToObject<int>();
            ColumnIndex = block["ColumnIndex"].ToObject<int>();
            RowSpan = block["RowSpan"].ToObject<int>();
            ColumnSpan = block["ColumnSpan"].ToObject<int>();
            try
            {
                // Check if there are any relationsips
                if (block["Relationships"] != null && block["Relationships"].ToList<JToken>().Count > 0)
                {
                    var children = block["Relationships"].ToList<JToken>()[0]["Ids"].ToList<JToken>();

                    foreach (var child in children)
                    {
                        _childIds.Add(child.ToString());
                    }
                }
            }
            catch (System.ArgumentNullException e)
            {

            }
        }
        public override Appserver.TextractDocument.BlockType GetBlockType() 
            => Appserver.TextractDocument.BlockType.CELL;
        public override Geometry GetGeometry() => _geometry;
        public override string GetId() => _Id;
        public override List<Block> GetRelationships() => _children;
        public override int GetPage() => _page;
        public override float GetConfidence() => Confidence;
        public int GetRow() => RowIndex;
        public int GetCol() => ColumnIndex;
        ////////////////////////
        /// Properties of a Cell
        ////////////////////////
        ///

        float Confidence;
        private int RowIndex;
        private int ColumnIndex;
        private int RowSpan;
        private int ColumnSpan;

        private Geometry _geometry;
        private int _page;
        private string _Id;

        private List<Block> _children = new List<Block>();
        private Dictionary<string, Word> _childMap = new Dictionary<string, Word>();
        private List<string> _childIds = new List<string>();

        private Page _parent;
        public override void SetPage(Page page)
        {
            _parent = page;
        }
        public override void CreateStructure()
        {
            foreach (var child in _childIds)
            {
                 _childMap[child] = (Word)_parent.GetChildById(child);
                _children.Add(_childMap[child]);
            }
        }
        public override void PrintSummary()
        {
            Console.WriteLine(String.Format("[{0}][{1}]: {2}", RowIndex, ColumnIndex, ToString()));
        }

        public override string ToString()
        {
            string words = "";
            foreach( var word in _children)
            {
                words = words + word.ToString() + " ";
            }
            words.Trim();
            return words;
        }
    }
}
