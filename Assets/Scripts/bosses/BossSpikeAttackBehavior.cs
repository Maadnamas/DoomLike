using UnityEngine;
using System.Collections;

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
        // Mantener parado durante el ataque
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
        Debug.Log("Boss: Iniciando ataque de pinchos");

        // IMPORTANTE: PlaySpikeAttack() activa el trigger "ataque2" (ataque de pinchos)
        boss.GetAnimator().PlaySpikeAttack();
        Debug.Log("Boss: Trigger 'ataque2' activado");

        yield return new WaitForSeconds(0.5f);

        // Calcular posiciones de los pinchos alrededor del boss
        Vector3[] spikePositions = CalculateSpikePositions(boss);

        // Mostrar avisos (partículas) donde van a salir los pinchos
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

        // Esperar el tiempo de aviso
        yield return new WaitForSeconds(boss.bossData.spikeWarningTime);

        // Destruir avisos y spawnear pinchos
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

                // Configurar el pincho
                BossSpike spikeScript = spike.GetComponent<BossSpike>();
                if (spikeScript != null)
                {
                    spikeScript.Initialize(boss.bossData.spikeDamage);
                }
            }
        }

        Debug.Log("Boss: Pinchos spawneados");

        // Actualizar timer de ataque
        boss.lastSpikeAttackTime = Time.time;

        // Esperar un momento antes de volver a perseguir
        yield return new WaitForSeconds(1f);

        // Volver a comportamiento de persecución
        boss.SetBehavior(new BossChaseBehavior());
    }

    private Vector3[] CalculateSpikePositions(BossAI boss)
    {
        Vector3[] positions = new Vector3[boss.bossData.numberOfSpikes];
        float angleStep = 360f / boss.bossData.numberOfSpikes;

        for (int i = 0; i < boss.bossData.numberOfSpikes; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;
            float x = boss.transform.position.x + boss.bossData.spikeRadius * Mathf.Cos(angle);
            float z = boss.transform.position.z + boss.bossData.spikeRadius * Mathf.Sin(angle);

            // Hacer raycast para encontrar el suelo
            RaycastHit hit;
            Vector3 rayOrigin = new Vector3(x, boss.transform.position.y + 5f, z);

            if (Physics.Raycast(rayOrigin, Vector3.down, out hit, 10f))
            {
                positions[i] = hit.point;
            }
            else
            {
                positions[i] = new Vector3(x, boss.transform.position.y, z);
            }
        }

        return positions;
    }
}