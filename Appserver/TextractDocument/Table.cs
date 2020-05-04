using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Appserver.TextractDocument
{
    public class Table : Block
    {
        /*******************************************************************************
        /// Fields
        *******************************************************************************/

        /// This confidence is how confident it is that this is a table, not of the
        /// contents of the table
        float Confidence;

        private Geometry _geometry;
        private int _page;
        private string _Id;

        // Ordered list from document
        private List<Block> _children = new List<Block>();

        // Quick lookup list
        private Dictionary<string, Block> _childMap = new Dictionary<string, Block>();

        // Used in initial setup to store id's to use during structure creation
        private List<string> _childIds = new List<string>();

        // 2D table structure
        private List<List<Cell>> table = new List<List<Cell>>();
        private int _rowcount = 0;
        private int _columncount = 0;
        // The parent is used for lookup of children during structure creation
        private Page _parent;

        /*******************************************************************************
        /// Constructors
        *******************************************************************************/
        public Table(JToken block)
        {
            _geometry = new Geometry(block["Geometry"]);
            _Id = block["Id"].ToString();
            Confidence = block["Confidence"].ToObject<float>();
            _page = block["Page"].ToObject<int>();
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

        /*******************************************************************************
        /// Properties
        *******************************************************************************/
        public override BlockType GetBlockType() 
            => BlockType.TABLE;
        public override Geometry GetGeometry() => _geometry;
        public override string GetId() => _Id;
        public override List<Block> GetRelationships() => _children;
        public override int GetPage() => _page;
        public override float GetConfidence() => Confidence;
        public List<Cell> this[int i] => table[i];
        public List<List<Cell>> GetTable() => table;
        public override void SetPage(Page page)
        {
            _parent = page;
        }

        /*******************************************************************************
        /// Methods
        *******************************************************************************/
        /// <summary>
        /// The structure in Textract is by row. We want to create the rows then stack them
        /// in a list.
        /// </summary>
        public override void CreateStructure()
        {

            if (_parent == null)
            {
                throw new System.ArgumentException("No Parent");
            }
            int rowcount = 1;
            List<Cell> row = new List<Cell>();
            foreach (var child in _childIds)
            {
                Block b = _parent.GetChildById(child);
                _childMap[child] = b;
                b.SetPage(_parent);
                if( ((Cell)b).GetRow() != rowcount)
                {
                    table.Add(row);
                    row = new List<Cell>();
                    rowcount = ((Cell)b).GetRow();
                }
                row.Add((Cell)b);
                if (row.Count > this._columncount)
                    this._columncount = row.Count;
                b.CreateStructure();
            }

            // After last cell, add row
            table.Add(row);
            rowcount = row.Last<Cell>().GetRow();
            _rowcount = table.Count;
        }
        public override void PrintSummary()
        {
            Console.WriteLine(String.Format("Table-id: {0}", _Id));
            Console.WriteLine(String.Format("Child count: {0}", _childIds.Count));
            Console.WriteLine(String.Format("Table-size: {0}x{1}", _rowcount, _columncount));
            Console.WriteLine(String.Format("Table-Confidence: {0}", Confidence));
            Console.WriteLine(ToString());
            Console.WriteLine(String.Format("Child count: {0}", _childMap.Count));
        }
        public override string ToString()
        {
            string t = "";
            foreach( var row in table)
            {
                string r = "";
                foreach ( var cell in row)
                {
                    r = r + cell.ToString() + "|";
                }
                t += r.Remove(r.Length - 1, 1) + "\n";
            }
            return t;
        }
    }
}
