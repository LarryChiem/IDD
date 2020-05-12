using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using Amazon.Runtime.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Appserver.TextractDocument
{
    public class Page: Block
    {
        /*******************************************************************************
        /// Fields
        *******************************************************************************/
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

        /*******************************************************************************
        /// Constructors
        *******************************************************************************/
        public Page(Geometry geometry, string Id, int page) => (_geometry, _Id, _page) = (geometry, Id, page);
        public Page(JToken block)
        {
            this._geometry = new Geometry(block["Geometry"]);
            this._Id = block["Id"].ToString();
            this._page = block["Page"].ToObject<int>();
        }

        /*******************************************************************************
        /// Properties
        *******************************************************************************/
        public override BlockType GetBlockType()
            => BlockType.PAGE;
        public override Geometry GetGeometry() => _geometry;
        public override int GetPage() => _page;
        public int SetPage(int page_number) => (_page) = (page_number);
        public override string GetId() => _Id;
        public override List<Block> GetRelationships() => _children;

        // We are always confident this is a page
        public override float GetConfidence() => 1;

        public List<KeyValuePair<KeyValueSet, KeyValueSet>> GetFormItems() => _keyvaluepairs;
        public List<Table> GetTables() => _tables;

        /*******************************************************************************
        /// Methods
        *******************************************************************************/
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
                    case BlockType.LINE:
                        _lines.Add((Line)child);
                        child.SetPage(this);
                        child.CreateStructure();
                        break;
                    case BlockType.TABLE:
                        _tables.Add((Table)child);
                        child.SetPage(this);
                        child.CreateStructure();
                        break;
                    case BlockType.SELECTION_ELEMENT:
                        _selection.Add((SelectionElement)child);
                        child.SetPage(this);
                        child.CreateStructure();
                        break;
                    case BlockType.KEY_VALUE_SET:
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

            _keyvaluepairs = Sort(_keyvaluepairs);
        }

        private List<KeyValuePair<KeyValueSet, KeyValueSet>> Sort(List<KeyValuePair<KeyValueSet, KeyValueSet>> kvps)
        {
            // The height threshold for determining if two blocks are on the same line. 0.01 = 1% of the page height
            const double threshold = 0.01;

            // This is an Insertion Sort sorting by the Top values. If two blocks have very similar Top values,
            // then they're probably on the same line. We then sort by Left values for Left -> Right sort.
            // This will end up with a list sorted as if you were reading it out of a page.
            var ret = new List<KeyValuePair<KeyValueSet, KeyValueSet>>();
            foreach (var kvp in kvps)
            {
                int i;
                for (i = 0; i < ret.Count; ++i)
                {
                    // If two blocks are close enough (as determined by threshold), then they're probably on the same line
                    if (Math.Abs(kvp.Key.GetGeometry().box.Top - ret[i].Key.GetGeometry().box.Top) < threshold)
                    {
                        // Then break only if the one in question is to the left of the comparison block
                        if (kvp.Key.GetGeometry().box.Left < ret[i].Key.GetGeometry().box.Left)
                            break;
                    }
                    // Else if the insertion block is above the comparison block, then break
                    else if (kvp.Key.GetGeometry().box.Top < ret[i].Key.GetGeometry().box.Top)
                        break;

                }
                //Insert the insertion block at the calculated index
                ret.Insert(i, kvp);
            }

            return ret;
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
