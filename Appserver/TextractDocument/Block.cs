using System.Collections.Generic;

namespace Appserver.TextractDocument
{
    /// <summary>
    ///  The Block base class is what other members of the Textract Document inherit from.
    ///  All common properties are listed here.
    /// </summary>
    public abstract class Block
    {
        public abstract BlockType GetBlockType();
        public abstract Geometry GetGeometry();
        public abstract string GetId();
        public abstract List<Block> GetRelationships();
        public abstract int GetPage();
        public abstract void SetPage(Page page);
        /// <summary>
        /// Used in creation to create the structure of the document.
        /// </summary>
        public abstract void CreateStructure();
        public abstract void PrintSummary();
        public abstract float GetConfidence();
    }
}
