using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using GamePush.Data;

namespace GamePush.Core
{
    public class DocumentsModule
    {
        public event Action OnDocumentsOpen;
        public event Action OnDocumentsClose;
        
        public event Action<DocumentData, Action, Action> OnShowDocument;
        
        public event Action<DocumentData> OnFetchSuccess;
        public event Action OnFetchError;
        public async Task Open(DocumentType type = DocumentType.PLAYER_PRIVACY_POLICY)
        {
            var fetchTask = Fetch(DocumentFormat.HTML, type);
            
            var document = await fetchTask;
            
            if (document != null)
            {
                document.Format = DocumentFormat.HTML;
                document.content = ParseHTML(document.content);
                OnShowDocument?.Invoke(document, OnDocumentsOpen, OnDocumentsClose);
            }
            else
            {
                Logger.Error("Document not found");
            }
        }

        public async Task<DocumentData> Fetch(DocumentFormat format = DocumentFormat.HTML, DocumentType type = DocumentType.PLAYER_PRIVACY_POLICY)
        {
            FetchDocumentInput input = new FetchDocumentInput
                {
                    type = type.ToString(),
                };

            var result = await DataFetcher.FetchDocument(input, format.ToString());
            if (result == null)
            {
                Logger.Error("Can't fetch privacy policy");
                OnFetchError?.Invoke();
            }

            result.Format = format;
            
            OnFetchSuccess?.Invoke(result);

            return result;
        }
        
        string ParseHTML(string html)
        {
            html = html.Replace("<h1>", "<size=50><b>");
            html = html.Replace("</h1>", "</b></size>");

            html = html.Replace("<h2>", "<size=45><b>");
            html = html.Replace("</h2>", "</b></size>");
            
            html = html.Replace("<h3>", "<size=40><b>");
            html = html.Replace("</h3>", "</b></size>");

            html = html.Replace("<p>", "<size=30>");
            html = html.Replace("</p>", "</size>\n");

            html += "\n\n\n";
            return html;
        }
    }
}
