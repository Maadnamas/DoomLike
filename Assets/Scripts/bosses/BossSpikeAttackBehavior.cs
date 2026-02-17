using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BossSpikeAttackBehavior : IBossBehavior
{
    private bool attackStarted = false;
    private Coroutine attackCoroutine;

    public void OnEnter(BossAI boss)
    {
        boss.GetAnimator().SetWalking(false);
        boss.GetAnimator().SetIdle(true);
        boss.StopMovement();

        if (!attackStarted)
        {
            attackStarted = true;
            attackCoroutine = boss.StartCoroutine(ExecuteSpikeAttack(boss));
        }
    }

    public void Execute(BossAI boss)
    {
        boss.StopMovement();

        if (boss.player != null)
        {
            boss.RotateTowards(boss.player.position, 3f);
        }
    }

    public void OnExit(BossAI boss)
    {
        boss.GetAnimator().SetIdle(false);

        if (attackCoroutine != null)
        {
            boss.StopCoroutine(attackCoroutine);
        }
    }

    private IEnumerator ExecuteSpikeAttack(BossAI boss)
    {
        Debug.Log("Boss: Initiating spike attack");

        // Activate spike attack animation
        boss.GetAnimator().PlaySpikeAttack();
        Debug.Log("Boss: Trigger 'attack2' activated");

        yield return new WaitForSeconds(0.5f);

        // Calculate RANDOM spike positions + ONE at player position
        Vector3[] spikePositions = CalculateRandomSpikePositionsWithPlayerTarget(boss);

        // Show warnings
        GameObject[] warnings = new GameObject[spikePositions.Length];
        for (int i = 0; i < spikePositions.Length; i++)
        {
            if (boss.bossData.spikeWarningPrefab != null)
            {
                warnings[i] = Object.Instantiate(
                    boss.bossData.spikeWarningPrefab,
                    spikePositions[i],
                    Quaternion.identity
                );
            }
        }

        yield return new WaitForSeconds(boss.bossData.spikeWarningTime);

        // Destroy warnings and spawn spikes
        for (int i = 0; i < spikePositions.Length; i++)
        {
            if (warnings[i] != null)
                Object.Destroy(warnings[i]);

            if (boss.bossData.spikePrefab != null)
            {
                GameObject spike = Object.Instantiate(
                    boss.bossData.spikePrefab,
                    spikePositions[i],
                    Quaternion.identity
                );

                BossSpike spikeScript = spike.GetComponent<BossSpike>();
                if (spikeScript != null)
                {
                    bool isPlayerSpike = (i == 0);
                    float extraDownOffset = isPlayerSpike ? 3f : 0f;

                    spikeScript.Initialize(boss.bossData.spikeDamage, isPlayerSpike, extraDownOffset);
                }
            }
        }

        Debug.Log("Boss: Spikes spawned (including one at player position)");

        boss.lastSpikeAttackTime = Time.time;

        yield return new WaitForSeconds(1f);

        boss.SetBehavior(new BossChaseBehavior());
    }

    private Vector3[] CalculateRandomSpikePositionsWithPlayerTarget(BossAI boss)
    {
        List<Vector3> positions = new List<Vector3>();
        Vector3 bossCenter = boss.transform.position;
        float radius = boss.bossData.spikeRadius;
        int numberOfSpikes = boss.bossData.numberOfSpikes;

        // IMPORTANT: Add player position first
        if (boss.player != null)
        {
            Vector3 playerPosition = GetGroundPosition(boss.player.position);
            positions.Add(playerPosition);
            Debug.Log("Target Spike: Spawning at player position");
        }

        // Generate random spikes
        float minDistanceBetweenSpikes = radius * 0.3f;
        int remainingSpikes = numberOfSpikes - 1;
        int attempts = 0;
        int maxAttempts = remainingSpikes * 10;

        while (positions.Count < numberOfSpikes && attempts < maxAttempts)
        {
            attempts++;

            Vector3 randomPosition = GetRandomPositionInCircle(bossCenter, radius);

            bool tooClose = false;
            foreach (Vector3 existingPos in positions)
            {
                float distance = Vector3.Distance(randomPosition, existingPos);
                if (distance < minDistanceBetweenSpikes)
                {
                    tooClose = true;
                    break;
                }
            }

            if (!tooClose)
            {
                Vector3 groundPosition = GetGroundPosition(randomPosition);
                positions.Add(groundPosition);
            }
        }

        while (positions.Count < numberOfSpikes)
        {
            Vector3 randomPosition = GetRandomPositionInCircle(bossCenter, radius);
            Vector3 groundPosition = GetGroundPosition(randomPosition);
            positions.Add(groundPosition);
        }

        Debug.Log($"Generated {positions.Count} spike positions (1 at player + {positions.Count - 1} random)");
        return positions.ToArray();
    }

    // Find ground position using raycast
    private Vector3 GetGroundPosition(Vector3 position)
    {
        RaycastHit hit;
        Vector3 rayOrigin = position + Vector3.up * 5f;

        if (Physics.Raycast(rayOrigin, Vector3.down, out hit, 10f))
        {
            return hit.point;
        }
        else
        {
            return position;
        }
    }

    // Generate random position inside a circle
    private Vector3 GetRandomPositionInCircle(Vector3 center, float radius)
    {
        float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        float distance = Mathf.Sqrt(Random.Range(0f, 1f)) * radius;

        float x = center.x + distance * Mathf.Cos(angle);
        float z = center.z + distance * Mathf.Sin(angle);

        return new Vector3(x, center.y, z);
    }
}