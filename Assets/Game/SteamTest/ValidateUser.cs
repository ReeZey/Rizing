using UnityEngine;
using Steamworks;
using Steamworks.Data;
using Rizing.Abstract;

public class ValidateUser : SingletonMono<ValidateUser>
{
    [SerializeField]
    private uint _steamAppId = 2269150;

    void Start()
    {
        SteamClient.Init(_steamAppId);

        SteamServerInit aa = new SteamServerInit {
            GamePort = 27017,
            DedicatedServer = true,
        };

        SteamServer.Init(_steamAppId, aa, true);
        SteamServer.LogOnAnonymous();

        SteamServer.OnValidateAuthTicketResponse += ( steamid, ownerid, rsponse ) =>
        {
            Debug.Log( $"SteamServer.OnValidateAuthTicketResponse: {steamid} {ownerid} {rsponse}" );
        };

        SteamServer.OnSteamServersConnected += () =>
        {
            Debug.Log("OnSteamServersConnected");
        };
        SteamServer.OnSteamServersDisconnected += (reason) =>
        {
            Debug.Log("OnSteamServersDisconnected");
        };
        SteamServer.OnSteamServerConnectFailure += ( result, aa ) =>
        {
            Debug.Log( $"OnSteamServerConnectFailure: {result}" );
        };
    }
    
    [ContextMenu("Authenticate")]
    public void Authenticate() {        
        Debug.Log("Authenticate");
        var ticket = SteamUser.GetAuthSessionTicket( NetIdentity.LocalHost );

        if ( !SteamServer.BeginAuthSession( ticket.Data, SteamClient.SteamId ) )
        {
            Debug.Log("redjected");
        }
    }
}
