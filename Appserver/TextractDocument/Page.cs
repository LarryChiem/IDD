using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Appserver.TextractDocument
{
    public class Page: Block
    {
        ///////////////////////////////////////////////
        // Inherited Members
        ///////////////////////////////////////////////
        public override Appserver.TextractDocument.BlockType GetBlockType() 
            => Appserver.TextractDocument.BlockType.PAGE;
        public override Geometry GetGeometry() => _geometry;
        public override int GetPage() => _page;
        public override string GetId() => _Id;
        public override List<Block> GetRelationships() => _children;

        // We are always confident this is a page
        public override float GetConfidence() => 1;

        public List<KeyValuePair<KeyValueSet, KeyValueSet>> GetFormItems() => _keyvaluepairs;

        ///////////////////////////////////////////////
        /// Properties
        ///////////////////////////////////////////////
        private List<Block> _children = new List<Block>();
        private Dictionary<string, Block> _childMap = new Dictionary<string, Block>();
        private List<Line> _lines = new List<Line>();
        private List<Table> _tables = new List<Table>();
        private List<SelectionElement> _selection = new List<SelectionElement>();
        private List<KeyValueSet> _keyvalue = new List<KeyValueSet>();
        private List<KeyValuePair<KeyValueSet, KeyValueSet>> _keyvaluepairs = new List<KeyValuePair<KeyValueSet, KeyValueSet>>();
        private readonly Geometry _geometry;
        private int _page;
        private string _Id;

        public Page(Geometry geometry, string Id, int page) => (_geometry, _Id, _page) = (geometry, Id, page);
        public Page(JToken block)
        {
            this._geometry = new Geometry(block["Geometry"]);
            this._Id = block["Id"].ToString();
            this._page = block["Page"].ToObject<int>();
        }
        
        public void addBlocks(List<Block> blocks)
        {
            foreach( var b in blocks)
            {
                addBlock(b);
            }
        }
        public void addBlock(Block block)
        {
            _children.Add(block);
            _childMap.Add(block.GetId(), block);
        }

        public override void CreateStructure()
        {
            foreach(var child in _children)
            {
                switch( child.GetBlockType())
                {
                    case Appserver.TextractDocument.BlockType.LINE:
                        _lines.Add((Line)child);
                        child.SetPage(this);
                        child.CreateStructure();
                        break;
                    case Appserver.TextractDocument.BlockType.TABLE:
                        _tables.Add((Table)child);
                        child.SetPage(this);
                        child.CreateStructure();
                        break;
                    case Appserver.TextractDocument.BlockType.SELECTION_ELEMENT:
                        _selection.Add((SelectionElement)child);
                        child.SetPage(this);
                        child.CreateStructure();
                        break;
                    case Appserver.TextractDocument.BlockType.KEY_VALUE_SET:
                        _keyvalue.Add((KeyValueSet)child);
                        child.SetPage(this);
                        child.CreateStructure();
                        break;
                    default:
                        //Console.WriteLine(String.Format("Block Type: {0}", child.GetBlockType()));
                        break;
                }
            }
            foreach( var kv in _keyvalue)
            {
                if(kv.GetEntityType() == KeyValueSet.EntityType.KEY)
                {
                    _keyvaluepairs.Add(new KeyValuePair<KeyValueSet,KeyValueSet>(kv, kv.GetValue()));
                }
            }
        }

        public override void SetPage(Page page)
        {
        }

        public Block GetChildById( string id)
        {
            return _childMap[id];
        }
        public override void PrintSummary()
        {
            Console.WriteLine(String.Format("Page-num: {0}", _page));
            Console.WriteLine(String.Format("Page-id: {0}",_Id));
            Console.WriteLine(String.Format("Line count: {0}", _lines.Count));
            Console.WriteLine(String.Format("Table count: {0}", _tables.Count));
            Console.WriteLine(String.Format("Selection Count: {0}", _selection.Count));
            Console.WriteLine(String.Format("Key-Value Count: {0}", _keyvalue.Count));
            Console.WriteLine(String.Format("Child count: {0}", _childMap.Count));
            foreach( var line in _lines)
            {
                line.PrintSummary();
            }
            foreach (var table in _tables)
            {
                table.PrintSummary();
            }
            foreach (var kv in _keyvaluepairs)
            {
                kv.Key.PrintSummary();
            }
        }

        public override string ToString()
        {
            string s = String.Format("Page: {0}\n",_page) ;

            foreach( var child in _children)
            {
                s += child.ToString();
            }
            
            return s;
        }
    }
}
