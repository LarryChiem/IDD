using System.Collections.Generic;

namespace Appserver.TextractDocument
{
    public abstract class Block
    {
        public abstract Appserver.TextractDocument.BlockType GetBlockType();
        public abstract Geometry GetGeometry();
        public abstract string GetId();
        public abstract List<Block> GetRelationships();
        public abstract int GetPage();
        public abstract void SetPage(Page page);
        public abstract void CreateStructure();
        public abstract void PrintSummary();
        public abstract float GetConfidence();
    }
}
