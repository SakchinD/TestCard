using Cysharp.Threading.Tasks;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class ImageLoader : MonoBehaviour
{
    public async UniTask<Texture2D> LoadImage(string url)
    {
        UnityWebRequest request = await UnityWebRequestTexture.GetTexture(url)
            .SendWebRequest()
            .WithCancellation(this.GetCancellationTokenOnDestroy());

        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log($"ERROR {request.error}");
            return null;
        }

        Texture2D avatar = DownloadHandlerTexture.GetContent(request);

        request.Dispose();

        return avatar;
    }
}
