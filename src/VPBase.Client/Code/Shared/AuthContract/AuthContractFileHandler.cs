using System.Collections.Generic;
using System.IO;
using System.Text;
using VPBase.Auth.Contract.Files;
using VPBase.Auth.Contract.SharedInterfaces;

namespace VPBase.Client.Code.Shared.AuthContract
{
    public class AuthContractFileHandler : IAuthContractFileHandler
    {
        private readonly ConfigFolderFileHandlerSettings _configFileHandlerSettings;
        private readonly IAuthContractLogger _logger;

        public AuthContractFileHandler(ConfigFolderFileHandlerSettings configFileHandlerSettings,
            IAuthContractLogger logger)
        {
            _configFileHandlerSettings = configFileHandlerSettings;
            _logger = logger;
        }

        public IEnumerable<string> ReadJsonDataFromFiles(string fileNameStartWith)
        {
            var listOfFiles = new List<string>();

            var physicaFolderPath = GetPhysicalFolderPath();

            if (!string.IsNullOrEmpty(physicaFolderPath))
            {
                var directoryInfo = new DirectoryInfo(physicaFolderPath);

                var files = directoryInfo.GetFiles(fileNameStartWith + "*" + _configFileHandlerSettings.FileExtension);

                foreach (var file in files)
                {
                    using (var stream = File.Open(file.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        using (var streamReader = new StreamReader(stream, Encoding.UTF8))
                        {
                            var fileData = streamReader.ReadToEnd();
                            listOfFiles.Add(fileData);
                        }
                    }
                }
            }

            return listOfFiles;
        }

        public bool WriteJsonDataToFile(string jsonData, string fileName)
        {
            var physicaFolderPath = GetPhysicalFolderPath();

            var filePath = Path.Combine(physicaFolderPath, fileName);

            using (var writer = new StreamWriter(filePath, false))
            {
                writer.Write(jsonData);
            };

            return true;
        }

        private string GetPhysicalFolderPath()
        {
            return _configFileHandlerSettings.PhysicalPath;
        }
    }

    public class TestAuthContractFileHandler : IAuthContractFileHandler
    {
        public TestAuthContractFileHandler()
        {
            TestableFiles = new List<TestableFile>();
        }

        public List<TestableFile> TestableFiles { get; set; }

        public IEnumerable<string> ReadJsonDataFromFiles(string fileNameStartWith)
        {
            var filesData = new List<string>();

            foreach (var testableFile in TestableFiles)
            {
                filesData.Add(testableFile.JsonData);
            }

            return filesData;
        }

        public bool WriteJsonDataToFile(string jsonData, string fileName)
        {
            TestableFiles.Add(new TestableFile() { JsonData = jsonData, FileName = fileName });

            return true;
        }
    }

    public class TestableFile
    {
        public string JsonData { get; set; }

        public string FileName { get; set; }
    }
}