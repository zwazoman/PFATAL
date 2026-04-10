using UnityEngine;

public class Discord_Controller : MonoBehaviour
{
    public static Discord_Controller Instance;

    private long applicationID = 1489601105536094298;
    public string details;
    public string state;
    [Space]
    public string largeImageKey;
    public string largeText;

    private long _time;


    public Discord.Discord discord;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        discord = new Discord.Discord(applicationID, (System.UInt64)Discord.CreateFlags.NoRequireDiscord);
        _time = System.DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        UpdateStatus();
    }

    private void Update()
    {
        try
        {
            discord.RunCallbacks();

        }
        catch
        {
            Debug.LogError("Error running Discord callbacks: ");
        }
    }

    void LateUpdate()
    {
        UpdateStatus();
    }

    void UpdateStatus()
    {
        try
        {
            var activityManager = discord.GetActivityManager();
            var activity = new Discord.Activity
            {
                Details = details,
                State = state,
/*                Assets =
                {
                    LargeImage = largeImageKey,
                    LargeText = largeText
                },*/
                Timestamps =
                {
                    Start = _time
                }
            };

            activityManager.UpdateActivity(activity, (res) =>
            {
                if (res != Discord.Result.Ok)
                {
                    Debug.LogWarning("Failed to update Discord activity: " + res);
                }
            });
        }
        catch
        {
            Destroy(gameObject);
        }
    }
}
