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
            Logger.Log("Try open");
            var fetchTask = Fetch(new FetchDocumentInput { Type = type });
            
            var document = await fetchTask;

            if (document != null)
            {
                document.Format = DocumentFormat.HTML;
                OnShowDocument?.Invoke(document, OnDocumentsOpen, OnDocumentsClose);
            }
            else
            {
                Logger.Error("Document not found");
            }
        }

        public async Task<DocumentData> Fetch(FetchDocumentInput input = null)
        {
            Logger.Log("Try fetch");
            
            if (input == null)
                input = new FetchDocumentInput { Type = DocumentType.PLAYER_PRIVACY_POLICY};
            
            var result = await DataFetcher.FetchDocument(input);
            if (result == null)
            {
                Logger.Error("Can't fetch privacy policy");
                OnFetchError?.Invoke();
            }

            result.Format = input.Format ?? DocumentFormat.HTML;
            
            OnFetchSuccess?.Invoke(result);

            return result;
        }
    }
}
