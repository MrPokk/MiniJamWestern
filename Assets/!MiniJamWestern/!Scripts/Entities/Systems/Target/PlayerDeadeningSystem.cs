using System;
using BitterECS.Core;
using UnityEngine.SceneManagement;

public class PlayerDeadeningSystem : IEcsAutoImplement
{
    public Priority Priority => Priority.High;


    private EcsEvent _ecsEvent = new EcsEvent().SubscribeWhereEntity<IsDeadEvent>(e => e.Has<TagPlayer>(), added: OnPlayerDead);
    private static void OnPlayerDead(EcsEntity entity)
    {
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
