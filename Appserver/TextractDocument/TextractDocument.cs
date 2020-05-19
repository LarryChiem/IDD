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
    /*******************************************************************************
    /// Enums
    *******************************************************************************/
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
        /*******************************************************************************
        /// Fields
        *******************************************************************************/
        private Dictionary<string, Block> _blockMap = new Dictionary<string, Block>();
        /*******************************************************************************
        /// Constructors
        *******************************************************************************/
        public TextractDocument()
        {

        }
        /*******************************************************************************
        /// Properties
        *******************************************************************************/
        public DocumentMetadata DocumentMetadata;
        public string JobStatus;
        public List<Page> Pages = new List<Page>();
        public Page GetPage(int pagenumber)
        {
            var p = Pages[pagenumber];
            if (p.GetPage() == pagenumber)
            {
                return p;
            }
            else
            {
                throw new System.ArgumentOutOfRangeException();
            }
        }
        public int PageCount() => Pages.Count();
        /*******************************************************************************
        /// Methods
        *******************************************************************************/
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
                Console.WriteLine("Starting Try Block");
                if (token["AnalyzeDocumentModelVersion"] != null)
                {
                    this.JobStatus = token["AnalyzeDocumentModelVersion"].ToString() == "1.0" ? "SUCCEEDED" : "FAILED";
                }
                else if (token["JobStatus"] == null)
                {
                    this.JobStatus = token["JobStatus"].ToString();
                }
                if(this.JobStatus != "SUCCEEDED")
                {
                    Console.WriteLine("Textract Failed");
                    return;
                }
                else
                {
                    Console.WriteLine("Textract Succeeded");
                }

                this.DocumentMetadata = new DocumentMetadata
                {
                    Pages = token["DocumentMetadata"]["Pages"].ToObject<int>()
                };

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
                switch (b["BlockType"]["Value"].ToString())
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

        public void FromTextractResponse( Amazon.Textract.Model.AnalyzeDocumentResponse response)
        {
            ParseJson(JObject.Parse( JsonConvert.SerializeObject(response)));
        }

        public void AddPages(TextractDocument doc)
        {
            if( Pages.Count == 0)
            {
                DocumentMetadata = doc.DocumentMetadata;
                JobStatus = doc.JobStatus;
            }
            var pages = doc.Pages;
            int count = Pages.Count;
            foreach( var p in pages)
            {
                p.SetPage(count);
                Pages.Add(p);
                ++count;
            }
        }

        public void printSummary()
        {
            foreach( var p in Pages)
            {
                p.PrintSummary();
            }
        }

        public override string ToString()
        {
            string response = String.Format("Page Count: {0}\n",Pages.Count);
            foreach( var p in Pages)
            {
                response += p.ToString();
            }
            return response;
        }
    }
}
