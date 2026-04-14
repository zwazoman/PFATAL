using UnityEngine;
using UnityEngine.UI;
using Steamworks;

public class SteamAvatarUI : MonoBehaviour
{
    public RawImage avatarImage;

    private Callback<AvatarImageLoaded_t> _avatarImageLoaded;

    private void OnEnable()
    {
        SteamAuthenticator.OnAuthSuccess += HandleAuthSuccess;
        SteamAuthenticator.OnAuthFailed  += HandleAuthFailed;
    }

    private void OnDisable()
    {
        SteamAuthenticator.OnAuthSuccess -= HandleAuthSuccess;
        SteamAuthenticator.OnAuthFailed  -= HandleAuthFailed;
    }

    private void HandleAuthSuccess(CSteamID id, string pseudo)
    {
        Debug.Log($"Bienvenue {pseudo} !");
        
        _avatarImageLoaded = Callback<AvatarImageLoaded_t>.Create(OnAvatarLoaded);

        int handle = SteamFriends.GetLargeFriendAvatar(id);
        if (handle > 0) avatarImage.texture = GetSteamTexture(handle);

    }

    private void HandleAuthFailed()
    {
        Debug.Log("Échec de connexion Steam.");
    }

    private void OnAvatarLoaded(AvatarImageLoaded_t callback)
    {
        if (callback.m_steamID != SteamUser.GetSteamID()) return;
        avatarImage.texture = GetSteamTexture(callback.m_iImage);
    }

    private Texture2D GetSteamTexture(int iImage)
    {
        uint width, height;
        SteamUtils.GetImageSize(iImage, out width, out height);

        byte[] data = new byte[width * height * 4];
        SteamUtils.GetImageRGBA(iImage, data, (int)(width * height * 4));

        Texture2D tex = new Texture2D((int)width, (int)height, TextureFormat.RGBA32, false);
        tex.LoadRawTextureData(data);
        FlipTexture(tex);
        tex.Apply();
        return tex;
    }

    private void FlipTexture(Texture2D tex)
    {
        var pixels = tex.GetPixels32();
        int w = tex.width, h = tex.height;
        for (var y = 0; y < h / 2; y++) for (var x = 0; x < w; x++) (pixels[y * w + x], pixels[(h - y - 1) * w + x]) = (pixels[(h - y - 1) * w + x], pixels[y * w + x]);
    
        tex.SetPixels32(pixels);
    }
}