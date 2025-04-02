using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Utils;
using Unity.Services.Authentication;
using Unity.Services.CloudCode;
using Unity.Services.Ugc;

public class CloudPlayersList {

    public async UniTask<List<CloudPlayer>> FetchAllPlayerIds() {
        var result = await CloudCodeService.Instance.CallEndpointAsync<CloudAllPlayers>("GetAllPlayers");
        result.Players = "{\"Items\":" + result.Players + "}";

        var players = JsonHelper.FromJson<CloudPlayer>(result.Players);

        var playerId = AuthenticationService.Instance.PlayerId;

        var listOfOtherPlayers = new List<CloudPlayer>();

        foreach (var player in players) {
            if (player.id == playerId) {
                continue;
            }
            listOfOtherPlayers.Add(player);
        }

        return listOfOtherPlayers;
    }

    public async UniTask<List<Content>> GetContentsListAsync() {
        var contents = await UgcService.Instance.GetContentsAsync(new GetContentsArgs());
        return contents.Results;
    }
}

[Serializable]
public class CloudPlayer {
    public string id;

    public object[] data;
}

public class CloudAllPlayers {
    [JsonProperty("players")]
    public string Players;
}
