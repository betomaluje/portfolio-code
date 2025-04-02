using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using EditorTool.Models;
using Unity.Services.Ugc;
using Unity.Services.Ugc.Generated.Models;
using UnityEngine;
using Utils;
using DebugTools;

namespace EditorTool.Storage {
    public class RaidCloudStorage {
        private const string Player_Data_Key = "player-map";

        private string _thumbnailFilePath = Directory.GetCurrentDirectory() + "/Assets/Screenshots/";
        private string _filePath = Directory.GetCurrentDirectory() + "/Assets/Maps/";

        private string GetFilePath(string fileName) => Path.Combine(_filePath, fileName);
        private string GetThumbnailPath(string fileName) => Path.Combine(_thumbnailFilePath, $"{fileName}.png");

        public RaidCloudStorage() {
            Directory.CreateDirectory(_filePath);
            Directory.CreateDirectory(_thumbnailFilePath);
        }

        public async UniTask<Content> UpdatePlayersRaid(string contentId, string playerId, string json, string playerName = null) {
            // Convert the JSON string into a byte array
            byte[] fileData = System.Text.Encoding.UTF8.GetBytes(json);

            var fileName = $"{playerId}_{Player_Data_Key}";
            await TakeScreenshot(fileName);
            if (ByteArrayToFile(fileName, fileData)) {

                using var contentStream = File.Open(GetFilePath(fileName), FileMode.Open, FileAccess.Read, FileShare.Read);
                using var thumbnailStream = File.Open(GetThumbnailPath(fileName), FileMode.Open, FileAccess.Read, FileShare.Read);

                var content = await UgcService.Instance.CreateContentVersionAsync(contentId, contentStream, thumbnailStream);
                // var updatedContent = await UpdateContentDetailsAsync(contentId, fileName, playerName);

                DebugLog.Log($"Updated the following content: {content.Id} version: {content.Version}.");

                return content;
            }
            return null;
        }

        private async UniTask<Content> UpdateContentDetailsAsync(Content oldContent) {
            var description = oldContent.Description;
            var tagId = oldContent.Tags.Select(tag => tag.Id).ToList();

            var metadata = new UpdateContentDetailsArgs(oldContent.Id, oldContent.Name, description, oldContent.Visibility == ContentVisibility.Public, tagId);
            var content = await UgcService.Instance.UpdateContentDetailsAsync(metadata);

            return content;
        }

        private async UniTask<List<string>> GetTagIds(string tagToSearch) {
            var tags = await UgcService.Instance.GetTagsAsync();
            var tag = tags.FirstOrDefault(x => x.Name == tagToSearch);
            return tag != null ? new List<string> { tag.Id } : new List<string>();
        }

        public async UniTask<List<Content>> GetPlayerContentAsync() {
            var contents = await UgcService.Instance.GetPlayerContentsAsync(new GetPlayerContentsArgs());
            return contents.Results;
        }

        /// <summary>
        /// Saves the grid to a file for other players to play
        /// </summary>
        /// <param name="playerId">The player's id to use</param>
        /// <param name="json">The map in a string json format</param>
        public async UniTask<Content> SaveRaid(string playerId, string json, string playerName = null) {

            var contents = await GetPlayerContentAsync();

            if (contents.Count > 0) {
                // update
                var content = contents[0];
                var updatedContent = await UpdatePlayersRaid(content.Id, playerId, json, playerName);
                return updatedContent;
            }

            // Convert the JSON string into a byte array
            byte[] fileData = System.Text.Encoding.UTF8.GetBytes(json);

            var fileName = $"{playerId}_{Player_Data_Key}";
            await TakeScreenshot(fileName);

            if (ByteArrayToFile(fileName, fileData)) {
                using var contentStream = File.Open(GetFilePath(fileName), FileMode.Open, FileAccess.Read, FileShare.Read);
                using var thumbnailStream = File.Open(GetThumbnailPath(fileName), FileMode.Open, FileAccess.Read, FileShare.Read);

                var description = playerName != null ? $"{playerName}'s Raid" : RaidNameGenerator.GenerateNickname();
                var tagId = await GetTagIds("Raid");

                var content = await UgcService.Instance.CreateContentAsync(
                    new CreateContentArgs(fileName, description, contentStream) {
                        IsPublic = true,
                        TagsId = tagId,
                        Thumbnail = thumbnailStream
                    });

                return content;
            }

            return null;
        }

        public async UniTask LoadRaidAsync(string contentId, Action<List<GridSaveData>> callback = null) {
            try {

                var content = await UgcService.Instance.GetContentAsync(new GetContentArgs(contentId) { DownloadContent = true, DownloadThumbnail = false });
                var loadedJson = System.Text.Encoding.UTF8.GetString(content.DownloadedContent);
                callback?.Invoke(GridSaveData.FromJson(loadedJson));
            }
            catch (Exception e) {
                DebugLog.Log($"Error loading player {contentId}_{Player_Data_Key} file: {e.Message}");
                callback?.Invoke(GridSaveData.FromJson(null));
            }
        }

        public async UniTask DeleteRaidAsync(string playerId) {
            await UgcService.Instance.DeleteContentAsync(playerId);

            DebugLog.Log($"Deleted content with id: {playerId}.");
        }

        private async UniTask TakeScreenshot(string fileName) {
            ScreenCapture.CaptureScreenshot(GetThumbnailPath(fileName));
            await UniTask.Delay(1000);
        }

        private bool ByteArrayToFile(string fileName, byte[] byteArray) {
            try {
                using var fs = new FileStream(GetFilePath(fileName), FileMode.Create, FileAccess.ReadWrite);
                fs.Write(byteArray, 0, byteArray.Length);
                fs.Close();
                return true;
            }
            catch (Exception ex) {
                Console.WriteLine("Exception caught in process: {0}", ex);
                return false;
            }
        }
    }
}