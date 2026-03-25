using System;
using BitterECS.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerDeadeningSystem : IEcsAutoImplement
{
    public Priority Priority => Priority.High;


    private EcsEvent _ecsEvent = new EcsEvent().SubscribeWhereEntity<IsDeadEvent>(e => e.Has<TagPlayer>(), added: OnPlayerDead);
    private static void OnPlayerDead(EcsEntity entity)
    {

        Debug.Log("Player is Dead next restart ");
        //TODO: ADD restart 

        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
