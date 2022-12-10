using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.RemoteConfig;
using Unity.Services.Core.Environments;
using Unity.Services.Authentication;


public struct userAttributes
{
    public int characterLevel;
}

public struct appAttributes{
    
}

public class RemoteConfigFetcher : MonoBehaviour
{
    [SerializeField] string environmentName;
    [SerializeField] int characterLevel;
    [SerializeField] bool fetch;
    // Start is called before the first frame update
    async void Awake()
    {
        var options = new InitializationOptions();
        options.SetEnvironmentName(environmentName);
        await UnityServices.InitializeAsync(options);

        if(AuthenticationService.Instance.IsSignedIn==false)
            await AuthenticationService.Instance.SignInAnonymouslyAsync();

        RemoteConfigService.Instance.FetchCompleted += OnFetchConfig;
        
    }

    private void OnFetchConfig(ConfigResponse response)
    {
        Debug.Log(response.requestOrigin);
        Debug.Log(response.body);
    }

    // Update is called once per frame
    void Update()
    {
        if(fetch)
        {
            fetch = false;

            RemoteConfigService.Instance.FetchConfigs
            (
                new userAttributes() {characterLevel = this.characterLevel},
                new appAttributes()
        
            );
                
        }
    }
}
