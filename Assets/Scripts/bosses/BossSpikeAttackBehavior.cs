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

        // Activar animación de ataque de pinchos
        boss.GetAnimator().PlaySpikeAttack();
        Debug.Log("Boss: Trigger 'ataque2' activado");

        yield return new WaitForSeconds(0.5f);

        // Calcular posiciones RANDOM de los pinchos + UNO en la posición del jugador
        Vector3[] spikePositions = CalculateRandomSpikePositionsWithPlayerTarget(boss);

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
                    bool isPlayerSpike = (i == 0); // El primer pincho es el que apunta al jugador
                    float extraDownOffset = isPlayerSpike ? 3f : 0f; // 3 unidades más abajo para el pincho del jugador

                    spikeScript.Initialize(boss.bossData.spikeDamage, isPlayerSpike, extraDownOffset);
                }
            }
        }

        Debug.Log("Boss: Pinchos spawneados (incluyendo uno en la posición del jugador)");

        // Actualizar timer de ataque
        boss.lastSpikeAttackTime = Time.time;

        // Esperar un momento antes de volver a perseguir
        yield return new WaitForSeconds(1f);

        // Volver a comportamiento de persecución
        boss.SetBehavior(new BossChaseBehavior());
    }

    private Vector3[] CalculateRandomSpikePositionsWithPlayerTarget(BossAI boss)
    {
        List<Vector3> positions = new List<Vector3>();
        Vector3 bossCenter = boss.transform.position;
        float radius = boss.bossData.spikeRadius;
        int numberOfSpikes = boss.bossData.numberOfSpikes;

        // IMPORTANTE: Primero agregar la posición del jugador
        if (boss.player != null)
        {
            Vector3 playerPosition = GetGroundPosition(boss.player.position);
            positions.Add(playerPosition);
            Debug.Log("Pincho objetivo: Spawneando en posición del jugador");
        }

        // Ahora generar el resto de pinchos aleatorios
        float minDistanceBetweenSpikes = radius * 0.3f;
        int remainingSpikes = numberOfSpikes - 1; // -1 porque ya agregamos el del jugador
        int attempts = 0;
        int maxAttempts = remainingSpikes * 10;

        while (positions.Count < numberOfSpikes && attempts < maxAttempts)
        {
            attempts++;

            // Generar posición aleatoria dentro del círculo
            Vector3 randomPosition = GetRandomPositionInCircle(bossCenter, radius);

            // Verificar que no esté muy cerca de otros pinchos
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

            // Si no está muy cerca, agregar la posición
            if (!tooClose)
            {
                Vector3 groundPosition = GetGroundPosition(randomPosition);
                positions.Add(groundPosition);
            }
        }

        // Si no se pudieron generar suficientes posiciones, rellenar las faltantes
        while (positions.Count < numberOfSpikes)
        {
            Vector3 randomPosition = GetRandomPositionInCircle(bossCenter, radius);
            Vector3 groundPosition = GetGroundPosition(randomPosition);
            positions.Add(groundPosition);
        }

        Debug.Log($"Generadas {positions.Count} posiciones de pinchos (1 en jugador + {positions.Count - 1} aleatorias)");
        return positions.ToArray();
    }

    // Encuentra la posición del suelo usando raycast
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
            // Si no hay suelo, usar la posición Y original
            return position;
        }
    }

    // Genera una posición aleatoria dentro de un círculo
    private Vector3 GetRandomPositionInCircle(Vector3 center, float radius)
    {
        // Usar distribución uniforme dentro del círculo
        float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;

        // Para distribución más uniforme, usar raíz cuadrada
        float distance = Mathf.Sqrt(Random.Range(0f, 1f)) * radius;

        float x = center.x + distance * Mathf.Cos(angle);
        float z = center.z + distance * Mathf.Sin(angle);

        return new Vector3(x, center.y, z);
    }
}