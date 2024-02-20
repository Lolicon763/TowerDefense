using GameEnum;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class GoogleDriveMapHandler : MonoBehaviour
{
    const string MapInfosFileName = "MapInfos";
    public static DriveService InitializeDriveService()
    {
        GoogleCredential credential;
        using (var stream = new FileStream("C:/Users/user/Downloads/crafty-raceway-414517-a779f4332785.json", FileMode.Open, FileAccess.Read))
        {
            credential = GoogleCredential.FromStream(stream)
                .CreateScoped(DriveService.ScopeConstants.Drive);
        }

        var service = new DriveService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = "Application Name",
        });

        return service;
    }
    public static async Task ListFilesAsync()
    {
        var service = InitializeDriveService();
        var request = service.Files.List();
        request.Fields = "files(id, name)";
        var response = await request.ExecuteAsync();
        Debug.Log($"response.Files.count = {response.Files.Count}");
        foreach (var file in response.Files)
        {
            Debug.Log($"{file.Name} ({file.Id})"); // 在Unity中使用Debug.Log代替Console.WriteLine
        }
    }
    public static async Task<string> UploadMapAsync(string filePath, string fileNameInDrive)
    {
        var fileMetadata = new Google.Apis.Drive.v3.Data.File()
        {
            Name = fileNameInDrive
        };

        List<MapInfo> mapInfos = await DownloadMapInfosAsync();
        HashSet<string> uniqueCodes = new();
        HashSet<string> uniqueNames = new();
        foreach (var item in mapInfos)
        {
            uniqueCodes.Add(item.index);
            uniqueNames.Add(item.name);
        }
        fileNameInDrive = EnsureUniqueFileName(uniqueNames, fileNameInDrive);
        var uniqueCode = GenerateUniqueCode(uniqueCodes);
        FilesResource.CreateMediaUpload request;
        var service = InitializeDriveService();
        using (var stream = new FileStream(filePath, FileMode.Open))
        {
            request = service.Files.Create(
                fileMetadata, stream, "application/json");
            request.Fields = "id";
            await request.UploadAsync();
        }

        var file = request.ResponseBody;
        Debug.Log("File ID: " + file.Id);
        MapInfo mapInfo = new MapInfo()
        {
            fileId = file.Id,
            name = fileNameInDrive,
            index = uniqueCode
        };
        mapInfos.Add(mapInfo);
        string updatedContent = JsonConvert.SerializeObject(mapInfos);
        await UploadJsonToDrive(service, updatedContent, MapInfosFileName);
        await ShareFileAsync(service, file.Id); // 假設 ShareFileAsync 已正確實現
        return $"https://drive.google.com/file/d/{file.Id}/view";
    }
    public async void HandleUploadAndShare(string path, string mapName)
    {
        List<MapInfo> mapInfos = await DownloadMapInfosAsync();
        HashSet<string> uniqueCodes = new();
        foreach (var item in mapInfos)
        {
            uniqueCodes.Add(item.index);
        }
        var uniqueCode = GenerateUniqueCode(uniqueCodes);
        try
        {
            string shareLink = await GoogleDriveMapHandler.UploadMapAsync(path, $"{mapName}.json");
            Debug.Log("Upload Successful");
            Debug.Log($"Share Link: {shareLink}");
            Debug.Log($"Unique Code: {uniqueCode}");
            CopyToClipboard(shareLink);
        }
        catch (Exception ex)
        {
            Debug.LogError("Upload Failed: " + ex.ToString());
        }
    }

    private void CopyToClipboard(string text)
    {
        GUIUtility.systemCopyBuffer = text;
    }

    public static async Task<string> ShareFileAsync(DriveService service, string fileId)
    {
        var permission = new Google.Apis.Drive.v3.Data.Permission()
        {
            Type = "anyone",
            Role = "reader",
        };

        var request = service.Permissions.Create(permission, fileId);
        await request.ExecuteAsync();

        return $"https://drive.google.com/file/d/{fileId}/view";
    }
    public static async Task DeleteAllFiles(DriveService service)
    {
        // 列出所有文件
        var listRequest = service.Files.List();
        listRequest.Fields = "files(id)";
        var files = await listRequest.ExecuteAsync();
        string path = Path.Combine(Application.dataPath, "Resources/maps");
        if (Directory.Exists(path))
        {
            // 獲取目錄下的所有文件
            string[] filesInLocal = Directory.GetFiles(path);

            foreach (var file in filesInLocal)
            {
                try
                {
                    // 刪除文件
                    File.Delete(file);
                    Debug.Log($"Deleted file: {file}");
                }
                catch (System.Exception ex)
                {
                    // 如果有錯誤發生，輸出錯誤信息
                    Debug.LogError($"Error deleting file {file}: {ex.Message}");
                }
            }
        }
        else
        {
            Debug.LogWarning($"Directory not found: {path}");
        }
        // 對每個文件執行刪除操作
        foreach (var file in files.Files)
        {
            if (file.Name == MapInfosFileName)
            {
                // 替換為一個空白文件
                var emptyContent = new MemoryStream();
                StreamWriter writer = new StreamWriter(emptyContent);
                writer.Write(""); // 寫入空字符串以清空文件內容
                writer.Flush();
                emptyContent.Position = 0; // 重置流位置

                var updateRequest = service.Files.Update(new Google.Apis.Drive.v3.Data.File(), file.Id, emptyContent, "application/octet-stream");
                await updateRequest.UploadAsync();

                Debug.Log($"Replaced file with ID: {file.Id} with an empty file.");
            }
            else
            {
                // 刪除文件
                var deleteRequest = service.Files.Delete(file.Id);
                await deleteRequest.ExecuteAsync();
                Debug.Log($"Deleted file with ID: {file.Id}");
            }
        }
    }
    public static string GenerateUniqueCode(HashSet<string> existingCodes)
    {
        var random = new System.Random();
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        string newCode;

        do
        {
            newCode = new string(Enumerable.Repeat(chars, 5)
              .Select(s => s[random.Next(s.Length)]).ToArray()).ToUpper();
        } while (existingCodes.Contains(newCode));

        return newCode;
    }
    public static async Task<List<MapInfo>> DownloadMapInfosAsync()
    {
        var service = InitializeDriveService();
        FilesResource.ListRequest listRequest = service.Files.List();
        listRequest.Q = $"name = '{MapInfosFileName}' and trashed = false";
        var files = await listRequest.ExecuteAsync();
        if (files.Files.Count == 0)
        {
            Debug.Log("            // 文件不存在，創建一個新的空白文件");
            await UploadJsonToDrive(service, "[]", MapInfosFileName);
        }
        else
        {
            Debug.Log("            // 文件存在");
            // 获取文件ID
            var fileId = files.Files[0].Id;
            var request = service.Files.Get(fileId); // 使用文件ID来获取文件
            var stream = new MemoryStream();
            await request.DownloadAsync(stream);

            stream.Seek(0, SeekOrigin.Begin);
            var reader = new StreamReader(stream);
            var content = await reader.ReadToEndAsync();

            var mapInfos = JsonConvert.DeserializeObject<List<MapInfo>>(content);
            return mapInfos;
        }

        // 如果文件不存在并且创建新文件后，你可能需要返回一个空列表或者再次尝试下载
        return new List<MapInfo>();
    }

    public static async Task<string> CreateOrUpdateFileAsync(DriveService service, string fileId, HashSet<string> existingCodes, string fileName = "CodeList.txt")
    {
        var content = string.Join("\n", existingCodes);
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        writer.Write(content);
        writer.Flush();
        stream.Position = 0;

        FilesResource.UpdateMediaUpload updateRequest = null;
        FilesResource.CreateMediaUpload createRequest = null;

        try
        {
            var fileMetadata = new Google.Apis.Drive.v3.Data.File();
            updateRequest = service.Files.Update(fileMetadata, fileId, stream, "text/plain");
            await updateRequest.UploadAsync();
            return "File updated successfully.";
        }
        catch (Exception ex)
        {
            Debug.LogError("Update failed, trying to create the file: " + ex.Message);
            var newFileMetadata = new Google.Apis.Drive.v3.Data.File() { Name = fileName };
            createRequest = service.Files.Create(newFileMetadata, stream, "text/plain");
            createRequest.Fields = "id";
            var file = await createRequest.UploadAsync();

            if (file.Status == Google.Apis.Upload.UploadStatus.Completed)
            {
                return "New file created successfully.";
            }
            else
            {
                throw new Exception("Failed to create new file.");
            }
        }
    }
    public static async Task UploadJsonToDrive(DriveService service, string jsonContent, string uploadFileName)
    {
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(jsonContent ?? ""));
        var fileMetadata = new Google.Apis.Drive.v3.Data.File()
        {
            Name = uploadFileName
        };
        FilesResource.CreateMediaUpload request;
        request = service.Files.Create(fileMetadata, stream, "application/json");
        request.Fields = "id";
        await request.UploadAsync();
        var file = request.ResponseBody;
        Debug.Log($"File ID: {file.Id}");
    }
    public static string EnsureUniqueFileName(HashSet<string> existingFileNames, string originalFileName)
    {
        string uniqueFileName = originalFileName;
        int counter = 1;

        while (existingFileNames.Contains(uniqueFileName))
        {
            Debug.Log($"file name {uniqueFileName} existing ,try {originalFileName} ({counter++}");
            uniqueFileName = $"{originalFileName} ({counter++})";
        }

        return uniqueFileName;
    }
}