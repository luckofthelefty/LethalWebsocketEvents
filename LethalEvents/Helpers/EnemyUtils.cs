using GameNetcodeStuff;
using UnityEngine;

namespace com.github.luckofthelefty.LethalEvents.Helpers;

internal static class EnemyUtils
{
    /// <summary>
    /// Scans all active enemies to find which one is attacking/targeting a player.
    /// Works for vanilla and modded enemies since it uses base EnemyAI fields.
    /// Returns the enemy name, or null if no attacking enemy is found.
    /// </summary>
    public static string FindAttackingEnemy(PlayerControllerB playerScript)
    {
        if (playerScript == null) return null;

        // First check: is the player locked in a kill animation with an enemy?
        // inAnimationWithEnemy is set on the player when grabbed/killed
        if (playerScript.inAnimationWithEnemy != null)
        {
            return GetEnemyName(playerScript.inAnimationWithEnemy);
        }

        // Second check: scan all enemies for one targeting this player
        var enemies = Object.FindObjectsOfType<EnemyAI>();
        if (enemies == null) return null;

        foreach (var enemy in enemies)
        {
            if (enemy == null || enemy.enemyType == null) continue;

            // inSpecialAnimationWithPlayer means the enemy is actively killing/grabbing
            if (enemy.inSpecialAnimationWithPlayer == playerScript)
            {
                return GetEnemyName(enemy);
            }

            // targetPlayer means the enemy is chasing/attacking this player
            if (enemy.targetPlayer == playerScript && enemy.movingTowardsTargetPlayer)
            {
                return GetEnemyName(enemy);
            }
        }

        return null;
    }

    private static string GetEnemyName(EnemyAI enemy)
    {
        if (enemy?.enemyType == null) return null;
        return enemy.enemyType.enemyName ?? enemy.GetType().Name;
    }
}
