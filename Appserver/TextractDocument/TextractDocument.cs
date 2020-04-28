using Amazon.Textract.Model;
using Amazon.Textract;
using Newtonsoft.Json;
using System.Text.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Appserver.TextractDocument
{
    public enum BlockType
    {
        PAGE,
        LINE,
        WORD,
        TABLE,
        CELL,
        KEY_VALUE_SET,
        SELECTION_ELEMENT
    }
    public class TextractDocument : AbstractTextractObject
    {
        public DocumentMetadata DocumentMetadata;
        public string JobStatus;
        public List<Page> Pages = new List<Page>();

        private Dictionary<string, Block> _blockMap = new Dictionary<string, Block>();
        public TextractDocument()
        {

        }
        public override void FromJson(JToken token)
        {
            // Read in all the blocks
            ParseJson(token);

            // Construct the structure
            foreach( var p in Pages)
            {
                p.CreateStructure();
            }
        }
        public void ParseJson(JToken token) 
        { 
            // Start reading
            JToken blocks;
            try
            {
                this.DocumentMetadata = new DocumentMetadata
                {
                    Pages = token["DocumentMetadata"]["Pages"].ToObject<int>()
                };
                this.JobStatus = token["JobStatus"].ToString();

                blocks = token["Blocks"];
            }
            catch(Exception e)
            {
                Console.WriteLine(String.Format("Error: Invalid Textract JSON. {0}",e.Message));
                throw;
            }

            Page currentPage = null;
            foreach( var b in blocks.Children())
            {
                Block block = null;
                switch (b["BlockType"].ToString())
                {
                    case "PAGE":
                        block = new Page(b);
                        break;
                    case "LINE":
                        block = new Line(b);
                        break;
                    case "WORD":
                        block = new Word(b);
                        break;
                    case "TABLE":
                        block = new Table(b);
                        break;
                    case "CELL":
                        block = new Cell(b);
                        break;
                    case "KEY_VALUE_SET":
                        block = new KeyValueSet(b);
                        break;
                    case "SELECTION_ELEMENT":
                        block = new SelectionElement(b);
                        break;
                    default:
                        Console.WriteLine(String.Format("Child Token: {0}", b.ToString()));
                        throw new System.ArgumentException(String.Format("Unknown block type: {0}", b["BlockType"].ToString()));
                }

                switch ( block.GetBlockType() )
                {
                    case Appserver.TextractDocument.BlockType.PAGE:
                        currentPage = (Page)block;
                        Pages.Add(currentPage);
                        break;
                    default:
                        currentPage.addBlock(block);
                        break;
                }

                _blockMap.Add(block.GetId(), block);
            }
        }

        public void FromTextractResponse( Amazon.Textract.Model.GetDocumentAnalysisResponse response)
        {
            ParseJson(System.Text.Json.JsonSerializer.Serialize(response));
        }

        public int PageCount() => Pages.Count();
        public void printSummary()
        {
            foreach( var p in Pages)
            {
                p.PrintSummary();
            }
        }
    }
}
