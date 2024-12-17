using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

using GamePush;
using Examples.Console;

namespace Examples.Files
{
    public class Files : MonoBehaviour
    {
        [SerializeField] private TMP_InputField _inputUrl;
        [SerializeField] private TMP_InputField _inputFilename;
        [SerializeField] private TMP_InputField _inputTags;

        [SerializeField] private TMP_InputField _inputContent;
        [SerializeField] private TMP_InputField _inputChooseFile;
        [Space(15)]
        [SerializeField] private Button _uploadButton;
        [SerializeField] private Button _uploadUrlButton;
        [SerializeField] private Button _uploadContentButton;
        [SerializeField] private Button _loadContentButton;
        [SerializeField] private Button _chooseFileButton;
        [SerializeField] private Button _fetchButton;
        [SerializeField] private Button _fetchMoreButton;
        [SerializeField] private Button _fetchWithTagsButton;

        private void OnEnable()
        {
            _uploadButton.onClick.AddListener(Upload);
            _uploadUrlButton.onClick.AddListener(UploadUrl);
            _uploadContentButton.onClick.AddListener(UploadContent);
            _loadContentButton.onClick.AddListener(LoadContent);
            _chooseFileButton.onClick.AddListener(ChooseFile);
            _fetchButton.onClick.AddListener(Fetch);
            _fetchMoreButton.onClick.AddListener(FetchMore);
            _fetchWithTagsButton.onClick.AddListener(FetchWithTags);


            GP_Files.OnUploadSuccess += OnUploadSuccess;
            GP_Files.OnUploadError += OnUploadError;

            GP_Files.OnUploadContentError += OnUploadContentError;
            GP_Files.OnUploadContentSuccess += OnUploadContentSuccess;

            GP_Files.OnUploadUrlError += OnUploadUrlError;
            GP_Files.OnUploadUrlSuccess += OnUploadUrlSuccess;

            GP_Files.OnLoadContent += OnLoadContent;
            GP_Files.OnLoadContentError += OnLoadContentError;

            GP_Files.OnChooseFile += OnChooseFile;
            GP_Files.OnChooseFileError += OnChooseFileError;

            GP_Files.OnFetchError += OnFetchError;
            GP_Files.OnFetchSuccess += OnFetchSuccess;

            GP_Files.OnFetchMoreError += OnFetchMoreError;
            GP_Files.OnFetchMoreSuccess += OnFetchMoreSuccess;

        }

        private void OnDisable()
        {
            _uploadButton.onClick.RemoveListener(Upload);
            _uploadUrlButton.onClick.RemoveListener(UploadUrl);
            _uploadContentButton.onClick.RemoveListener(UploadContent);
            _loadContentButton.onClick.RemoveListener(LoadContent);
            _chooseFileButton.onClick.RemoveListener(ChooseFile);
            _fetchButton.onClick.RemoveListener(Fetch);
            _fetchMoreButton.onClick.RemoveListener(FetchMore);
            _fetchWithTagsButton.onClick.RemoveListener(FetchWithTags);


            GP_Files.OnUploadSuccess -= OnUploadSuccess;
            GP_Files.OnUploadError -= OnUploadError;

            GP_Files.OnUploadContentError -= OnUploadContentError;
            GP_Files.OnUploadContentSuccess -= OnUploadContentSuccess;

            GP_Files.OnUploadUrlError -= OnUploadUrlError;
            GP_Files.OnUploadUrlSuccess -= OnUploadUrlSuccess;

            GP_Files.OnLoadContent -= OnLoadContent;
            GP_Files.OnLoadContentError -= OnLoadContentError;

            GP_Files.OnChooseFile -= OnChooseFile;
            GP_Files.OnChooseFileError -= OnChooseFileError;

            GP_Files.OnFetchError -= OnFetchError;
            GP_Files.OnFetchSuccess -= OnFetchSuccess;

            GP_Files.OnFetchMoreError -= OnFetchMoreError;
            GP_Files.OnFetchMoreSuccess -= OnFetchMoreSuccess;

        }


        public void Upload() => GP_Files.Upload(_inputTags.text);
        public void UploadUrl() => GP_Files.UploadUrl(_inputUrl.text, _inputFilename.text, _inputTags.text);
        public void UploadContent() => GP_Files.UploadContent(_inputContent.text, _inputFilename.text, _inputTags.text);
        public void LoadContent() => GP_Files.LoadContent(_inputUrl.text);
        public void ChooseFile() => GP_Files.ChooseFile(_inputChooseFile.text);

        public void Fetch() => GP_Files.Fetch();
        public void FetchMore() => GP_Files.FetchMore();

        public void FetchWithTags()
        {
            FilesFetchFilter filter = new FilesFetchFilter();
            filter.tags = _inputTags.text.Split(",");
            GP_Files.Fetch(filter);
        }




        private void OnUploadError() => ConsoleUI.Instance.Log("ON UPLOAD: ERROR");
        private void OnUploadSuccess(FileData data)
        {
            ConsoleUI.Instance.Log("ON UPLOAD: SUCCESS");

            ConsoleUI.Instance.Log("UPLOAD FILE: ID: " + data.id);
            ConsoleUI.Instance.Log("UPLOAD FILE: NAME: " + data.name);
            ConsoleUI.Instance.Log("UPLOAD FILE: PLAYER ID: " + data.playerId);
            ConsoleUI.Instance.Log("UPLOAD FILE: SIZE: " + data.size);
            ConsoleUI.Instance.Log("UPLOAD FILE: SRC: " + data.src);
            for (int i = 0; i < data.tags.Length; i++)
            {
                ConsoleUI.Instance.Log("UPLOAD FILE: TAGS: " + data.tags[i]);
            }
            ConsoleUI.Instance.Log(" ");
        }


        private void OnUploadUrlError() => ConsoleUI.Instance.Log("ON UPLOAD URL: ERROR");
        private void OnUploadUrlSuccess(FileData data)
        {
            ConsoleUI.Instance.Log("ON UPLOAD URL: SUCCESS");

            ConsoleUI.Instance.Log("UPLOAD URL: FILE: ID: " + data.id);
            ConsoleUI.Instance.Log("UPLOAD URL: FILE: NAME: " + data.name);
            ConsoleUI.Instance.Log("UPLOAD URL: FILE: PLAYER ID: " + data.playerId);
            ConsoleUI.Instance.Log("UPLOAD URL: FILE: SIZE: " + data.size);
            ConsoleUI.Instance.Log("UPLOAD URL: FILE: SRC: " + data.src);
            for (int i = 0; i < data.tags.Length; i++)
            {
                ConsoleUI.Instance.Log("UPLOAD URL: FILE: TAGS: " + data.tags[i]);
            }
            ConsoleUI.Instance.Log(" ");
        }


        private void OnUploadContentError() => ConsoleUI.Instance.Log("ON UPLOAD CONTENT: ERROR");
        private void OnUploadContentSuccess(FileData data)
        {
            ConsoleUI.Instance.Log("ON UPLOAD CONTENT: SUCCESS");

            ConsoleUI.Instance.Log("UPLOAD CONTENT: FILE: ID: " + data.id);
            ConsoleUI.Instance.Log("UPLOAD CONTENT: FILE: NAME: " + data.name);
            ConsoleUI.Instance.Log("UPLOAD CONTENT: FILE: PLAYER ID: " + data.playerId);
            ConsoleUI.Instance.Log("UPLOAD CONTENT: FILE: SIZE: " + data.size);
            ConsoleUI.Instance.Log("UPLOAD CONTENT: FILE: SRC: " + data.src);
            for (int i = 0; i < data.tags.Length; i++)
            {
                ConsoleUI.Instance.Log("UPLOAD CONTENT: FILE: TAGS: " + data.tags[i]);
            }
            ConsoleUI.Instance.Log(" ");
        }

        private void OnChooseFileError() => ConsoleUI.Instance.Log("ON CHOOSE FILE: ERROR");
        private void OnChooseFile(string tempUrl) => ConsoleUI.Instance.Log("ON CHOOSE FILE: " + tempUrl);

        private void OnLoadContent(string text) => ConsoleUI.Instance.Log("ON LOAD CONTENT: " + text);
        private void OnLoadContentError() => ConsoleUI.Instance.Log("ON LOAD CONTENT: ERROR");


        private void OnFetchError() => ConsoleUI.Instance.Log("FETCH FILE: ERROR");
        private void OnFetchSuccess(List<FileData> data, bool canLoadMore)
        {
            ConsoleUI.Instance.Log("FETCH FILE: CAN LOAD MORE: " + canLoadMore);

            for (int i = 0; i < data.Count; i++)
            {
                ConsoleUI.Instance.Log("FETCH FILE: ID: " + data[i].id);
                ConsoleUI.Instance.Log("FETCH FILE: NAME: " + data[i].name);
                ConsoleUI.Instance.Log("FETCH FILE: PLAYER ID: " + data[i].playerId);
                ConsoleUI.Instance.Log("FETCH FILE: SIZE: " + data[i].size);
                ConsoleUI.Instance.Log("FETCH FILE: SRC: " + data[i].src);
                for (int x = 0; x < data[i].tags.Length; x++)
                {
                    ConsoleUI.Instance.Log("FETCH FILE: TAGS: " + data[i].tags[x]);
                }
                ConsoleUI.Instance.Log(" ");
            }
        }

        private void OnFetchMoreError() => ConsoleUI.Instance.Log("FETCH MORE FILE: ERROR");
        private void OnFetchMoreSuccess(List<FileData> data, bool canLoadMore)
        {
            ConsoleUI.Instance.Log("FETCH MORE FILE: CAN LOAD MORE: " + canLoadMore);
            for (int i = 0; i < data.Count; i++)
            {
                ConsoleUI.Instance.Log("FETCH MORE FILE: ID: " + data[i].id);
                ConsoleUI.Instance.Log("FETCH MORE FILE: NAME: " + data[i].name);
                ConsoleUI.Instance.Log("FETCH MORE FILE: PLAYER ID: " + data[i].playerId);
                ConsoleUI.Instance.Log("FETCH MORE FILE: SIZE: " + data[i].size);
                ConsoleUI.Instance.Log("FETCH MORE FILE: SRC: " + data[i].src);
                for (int x = 0; x < data[i].tags.Length; x++)
                {
                    ConsoleUI.Instance.Log("FETCH MORE FILE: TAGS: " + data[i].tags[x]);
                }
                ConsoleUI.Instance.Log(" ");
            }
        }

    }
}