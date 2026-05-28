using System.Collections.Generic;
using UnityEngine;

public class GameRulesDeathMatch : GameRulesBase
{
    private readonly float _gameDuration;
    
    public GameRulesDeathMatch(List<ulong> clientIDs, float gameDuration) : base(clientIDs)
    {
        _gameDuration = gameDuration;
    }

    static int ComputeScore(int deaths, int kills)
    {
        //(voir excel nathan : https://eartsup-my.sharepoint.com/:x:/g/personal/nathan_tazi_e-artsup_net/IQDlLZONFxj8TLH2poCxFIdAAbTWHX5GnG7fBTHv6jm3ZIw?e=gYGWjQ) 
        const float deathPenaltyWeight = .5f;
        return Mathf.CeilToInt(kills-deaths * ((float)(kills-deaths)/(kills+deaths)*0.5f+0.5f)*deathPenaltyWeight);
    }
    
    protected override void StartGame()
    {
        //setup scoring system
        foreach (PlayerData player in _players.Values)
        {
            player.Character.health.OnDie += () =>
            {
                //quand un joueur meurt, on augmente son nombre de morts et donne un kill au joueur l'ayant tué.
                
                player.Score.Deaths++;
                player.Score.Points = ComputeScore(player.Score.Deaths,player.Score.Kills);
                
                if (player.Character.health.LastDamageSourceClientID != DamageData.NON_PLAYER_DAMAGE_SOURCE_CLIENT_ID)
                {
                    PlayerData killer = _players[player.Character.health.LastDamageSourceClientID];
                    killer.Score.Kills++;
                    killer.Score.Points = ComputeScore(player.Score.Deaths,player.Score.Kills);
                }

                //puis on update le score board
                UpdateScoreBoard();
            };
        }
        
        //StartTimer
        EndGameAfterTimerEnds();
    }

    private async void EndGameAfterTimerEnds()
    {
        await Awaitable.WaitForSecondsAsync(_gameDuration);
        TriggerGameEnd();
    }
    
    protected override GameResult EndGame()
    {
        GameResult gameResult = new GameResult();
        gameResult.LeaderBoard = GetLeaderBoard();
        return gameResult;
    }
}
