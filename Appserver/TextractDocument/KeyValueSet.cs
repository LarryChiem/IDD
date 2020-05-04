using Microsoft.CodeAnalysis;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Appserver.TextractDocument
{
    /// <summary>
    /// The KeyValueSet inherits from Block. It has one of two EntityTypes, either a KEY, or
    /// a VALUE.
    /// </summary>
    public class KeyValueSet: Block
    {
        /*******************************************************************************
        /// Fields
        *******************************************************************************/
        private float Confidence;

        private Geometry _geometry;
        private int _page;
        private string _Id;
        private EntityType _entityType;

        private List<Block> _children = new List<Block>();
        private Dictionary<string, Block> _childMap = new Dictionary<string, Block>();
        private List<string> _valueIds = new List<string>();
        private List<string> _childIds = new List<string>();
        private List<Block> _values = new List<Block>();
        private KeyValueSet _value;
        private Page _parent;


        /*******************************************************************************
        /// Enums 
        *******************************************************************************/
        /// <summary>
        /// For KEY entities the children is the value of the key
        /// For VALUE entities the children are the value
        /// </summary>

        public enum EntityType
        {
            KEY,
            VALUE
        }
        /*******************************************************************************
        /// Constructors 
        *******************************************************************************/
        public KeyValueSet(JToken block)
        {
            _geometry = new Geometry(block["Geometry"]);
            _Id = block["Id"].ToString();
            Confidence = block["Confidence"].ToObject<float>();
            _page = block["Page"].ToObject<int>();
            try
            {
                var relationships = block["Relationships"].ToList<JToken>();

                foreach( var r in relationships)
                {
                    if (r["Type"]["Value"].ToString() == "CHILD")
                    {
                        foreach (var child in r["Ids"].ToList<JToken>())
                        {
                            _childIds.Add(child.ToString());
                        }
                    }
                    else if( r["Type"]["Value"].ToString() == "VALUE")
                    {

                        foreach (var child in r["Ids"].ToList<JToken>())
                        {
                            _valueIds.Add(child.ToString());
                        }
                    }
                }
            }
            catch (System.ArgumentNullException e)
            {

            }
            var type = block["EntityTypes"].ToList<JToken>()[0].ToObject<string>();
            switch( type)
            {
                case "KEY":
                    _entityType = EntityType.KEY;
                    break;
                case "VALUE":
                    _entityType = EntityType.VALUE;
                    break;
            }
        }

        /*******************************************************************************
        /// Properties
        *******************************************************************************/
        public override BlockType GetBlockType()
            => BlockType.KEY_VALUE_SET;
        public override Geometry GetGeometry() => _geometry;
        public override string GetId() => _Id;
        public override List<Block> GetRelationships() => _children;
        public override int GetPage() => _page;
        public override float GetConfidence() => Confidence;
        public EntityType GetEntityType() => _entityType;
        public KeyValueSet GetValue()
        {
            if (_entityType == EntityType.KEY)
                return _value;
            return this;
        }

        
        public override void SetPage(Page page)
        {
            _parent = page;
        }


        /*******************************************************************************
        /// Methods
        *******************************************************************************/
        public override void CreateStructure()
        {

            if (_parent == null)
            {
                throw new System.ArgumentException("No Parent");
            }
            foreach (var child in _childIds)
            {
                var b = _parent.GetChildById(child);
                _childMap[child] = b;
                _children.Add(b);
            }

            if( _entityType == EntityType.KEY)
            {
                if( _valueIds.Count >= 1)
                {
                    var child = _parent.GetChildById(_valueIds[0]);
                    if( child.GetBlockType() != BlockType.KEY_VALUE_SET)
                    {
                        throw new System.ArgumentException("Value of Key is not valid type");
                    }
                    _value = (KeyValueSet)child;
                }
            }
        }
        public override void PrintSummary()
        {
            switch (_entityType)
            {
                case EntityType.KEY:
                    Console.WriteLine(String.Format("[{0}] => {1}", this.ToString(), _value.ToString()));
                    break;
                case EntityType.VALUE:
                    Console.WriteLine(String.Format("Value: {0}", this.ToString()));
                    break;
            }
        }

        public override string ToString()
        {
            string keystring = "";
            foreach (var child in _children)
            {
                keystring += child.ToString() + " ";
            }
            return keystring;
        }
    }
}
