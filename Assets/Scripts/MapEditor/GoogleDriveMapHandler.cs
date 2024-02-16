using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using System.IO;
using System.Threading.Tasks;
using System;

public class GoogleDriveMapHandler : MonoBehaviour
{
    public static async Task ListFilesAsync()
    {
        // 載入服務帳戶密鑰
        GoogleCredential credential;
        using (var stream = new FileStream("C:/Users/user/Downloads/crafty-raceway-414517-a779f4332785.json", FileMode.Open, FileAccess.Read))
        {
            credential = GoogleCredential.FromStream(stream)
                .CreateScoped(DriveService.ScopeConstants.Drive);
        }

        // 創建Drive API服務
        var service = new DriveService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = "ApplicationName",
        });

        // 列出檔案
        var request = service.Files.List();
        request.Fields = "files(id, name)";
        var response = await request.ExecuteAsync();
        Debug.Log($"response.Files.count = {response.Files.Count}");
        foreach (var file in response.Files)
        {
            Debug.Log($"{file.Name} ({file.Id})"); // 在Unity中使用Debug.Log代替Console.WriteLine
        }
    }
    public static async Task<string> UploadFileAsync(string filePath, string fileNameInDrive)
    {
        GoogleCredential credential;
        using (var stream = new FileStream("C:/Users/user/Downloads/crafty-raceway-414517-a779f4332785.json", FileMode.Open, FileAccess.Read))
        {
            credential = GoogleCredential.FromStream(stream).CreateScoped(DriveService.ScopeConstants.DriveFile);
        }

        var service = new DriveService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = "ApplicationName",
        });

        var fileMetadata = new Google.Apis.Drive.v3.Data.File()
        {
            Name = fileNameInDrive
        };

        FilesResource.CreateMediaUpload request;
        using (var stream = new FileStream(filePath, FileMode.Open))
        {
            request = service.Files.Create(
                fileMetadata, stream, "application/json");
            request.Fields = "id";
            await request.UploadAsync();
        }

        var file = request.ResponseBody;
        Debug.Log("File ID: " + file.Id);
        await ShareFileAsync(service, file.Id); // 假設 ShareFileAsync 已正確實現
        return $"https://drive.google.com/file/d/{file.Id}/view";
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

}