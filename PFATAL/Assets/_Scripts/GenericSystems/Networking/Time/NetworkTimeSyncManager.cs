using System.Collections.Generic;
using _Scripts.Exceptions;
using Unity.Netcode;
using UnityEngine;

namespace NetworkTime
{
    public class NetworkTimeSyncManager : NetworkBehaviour
    {
        Dictionary<ulong, float> TimestampCorrectionsPerClient = new();
        
        public async Awaitable SyncClientTimestamps()
        {
            if (!IsServer) throw new NetworkAuthorityException();
            
            print("Sync client timestamps. this is server : "+IsServer+". this clientID : "+NetworkManager.Singleton.LocalClientId);
            
            TimestampCorrectionsPerClient.Clear();
            
            //demande aux clients d'envoyer leur timestamp au server
            print("about to ping every client.");
            PingClientsRPC(TimeStamp.LocalNow);

            //attend qu'ils aient tous répondu
            print("waiting for everyone to answer.");
            while (TimestampCorrectionsPerClient.Count < NetworkManager.ConnectedClients.Count-1)
                await Awaitable.NextFrameAsync();
            print("everyone answered.");
            
            //envoie la correction à chaque client
            foreach (ulong clientID in TimestampCorrectionsPerClient.Keys)
            {
                ClientRpcParams clientRpcParams = new ClientRpcParams
                {
                    Send = new ClientRpcSendParams { TargetClientIds = new ulong[]{clientID} }
                };
                
                SendTimeStampCorrectionToClientRPC(
                    TimestampCorrectionsPerClient[clientID],
                    clientID);
            }
        }

        [Rpc(SendTo.NotServer)]
        void PingClientsRPC(float serverTimestamp)
        {
            print("timestamp Ping. this is server : "+IsServer+". clientID : "+NetworkManager.Singleton.LocalClientId);
            AnswerServerPingRPC(NetworkManager.Singleton.LocalClientId,serverTimestamp,TimeStamp.LocalNow);
        }
        
        [Rpc(SendTo.Server)]
        void AnswerServerPingRPC(ulong clientID,float serverTimestampAtStart,float clientTimestampAfterFirstRPC )
        {
            float rpcDuration = (TimeStamp.LocalNow - serverTimestampAtStart)*.5f ;
            float clientTimestampAtStart = clientTimestampAfterFirstRPC-rpcDuration;
            float clientTimestampCorrection = serverTimestampAtStart - clientTimestampAtStart;
            
            TimestampCorrectionsPerClient[clientID] = clientTimestampCorrection;
            print("timestamp Ping answer from : "+clientID+". Answer count : "+TimestampCorrectionsPerClient.Count +'/' + (NetworkManager.ConnectedClients.Count-1)+". this is server : "+IsServer+". this clientID : "+NetworkManager.Singleton.LocalClientId);
        }

        [Rpc(SendTo.NotServer)]
        void SendTimeStampCorrectionToClientRPC(float correction, ulong clientID)
        {
            if (NetworkManager.Singleton.LocalClientId != clientID) return;
            
            print("received TimestampCorrection : "+correction + ". this is server : "+IsServer+". this clientID : "+NetworkManager.Singleton.LocalClientId);
            TimeStamp.Correction = correction;
        }
        
        
    }

}
