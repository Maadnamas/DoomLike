using UnityEngine;
using System.Collections;

public class EnemyKamikazeBehavior : IEnemyBehavior
{
    private bool isExploding = false;

    // Configuración para que no se encimen (Steering Separation)
    private float separationRadius = 2.0f;
    private float separationForce = 3.0f;

    public void OnEnter(EnemyAI enemy)
    {
        enemy.GetAnimator().SetRunning(true);
        enemy.GetAnimator().SetWalking(false);
        enemy.GetAnimator().SetIdle(false);
        isExploding = false;
    }

    public void Execute(EnemyAI enemy)
    {
        if (isExploding || enemy.player == null) return;

        float distanceToPlayer = Vector3.Distance(enemy.transform.position, enemy.player.position);

        // 1. DETONACIÓN: Si llegamos al rango de ataque
        if (distanceToPlayer <= enemy.enemyData.attackRange)
        {
            enemy.StartCoroutine(ExplodeSequence(enemy));
            return;
        }

        // 2. MOVIMIENTO INTELIGENTE (Persiguir + Separarse de amigos)
        Vector3 directionToPlayer = (enemy.player.position - enemy.transform.position).normalized;

        // Calculamos vector para no chocar con otros enemigos
        Vector3 separation = CalculateSeparation(enemy);

        // Sumamos vectores: (Ir al Player) + (Alejarme de Vecinos)
        Vector3 finalDirection = (directionToPlayer + separation).normalized;

        // Moverse
        enemy.RotateTowards(enemy.player.position, true, 15f);
        enemy.MoveTo(enemy.transform.position + finalDirection, enemy.enemyData.runSpeed);
    }

    public void OnExit(EnemyAI enemy) { }

    // Algoritmo para evitar superposición (Boids simplificado)
    private Vector3 CalculateSeparation(EnemyAI me)
    {
        Vector3 separationVector = Vector3.zero;
        // Buscamos colisionadores en la capa Enemy
        Collider[] neighbors = Physics.OverlapSphere(me.transform.position, separationRadius, LayerMask.GetMask("Enemy"));

        foreach (var neighbor in neighbors)
        {
            if (neighbor.gameObject != me.gameObject)
            {
                // Vector que apunta LEJOS del vecino
                Vector3 pushDir = me.transform.position - neighbor.transform.position;
                // Cuanto más cerca, más fuerte empuja
                separationVector += pushDir.normalized / (pushDir.magnitude + 0.1f);
            }
        }
        return separationVector * separationForce;
    }

    private IEnumerator ExplodeSequence(EnemyAI enemy)  
    {
        isExploding = true;
        enemy.StopMovement();
        enemy.GetAnimator().SetRunning(false);
        enemy.GetAnimator().PlayAttack(); // Animación de "hincharse" o gritar

        // Sonido de advertencia
        if (enemy.enemyData.fuseSound)
            AudioManager.PlaySFX3D(enemy.enemyData.fuseSound, enemy.transform.position);

        // Esperar la mecha
        yield return new WaitForSeconds(enemy.enemyData.fuseTime);

        // --- EXPLOSIÓN ---
        Explode(enemy);
    }

    private void Explode(EnemyAI enemy)
    {
        // 1. VFX
        if (enemy.enemyData.explosionVFX)
        {
            GameObject vfx = Object.Instantiate(enemy.enemyData.explosionVFX, enemy.transform.position, Quaternion.identity);
            Object.Destroy(vfx, 3f); // Limpieza automática
        }

        // 2. Sonido
        if (enemy.enemyData.explosionSound)
            AudioManager.PlaySFX3D(enemy.enemyData.explosionSound, enemy.transform.position);

        // 3. Lógica de Daño y Empuje (Igual que el Cohete + Player)
        Collider[] hits = Physics.OverlapSphere(enemy.transform.position, enemy.enemyData.explosionRadius);
        foreach (var hit in hits)
        {
            // A. Daño Genérico (Player, Cajas, Otros Enemigos)
            IDamageable damageable = hit.GetComponent<IDamageable>();
            if (damageable != null)
            {
                // Daño basado en distancia (igual que tu cohete)
                float dist = Vector3.Distance(enemy.transform.position, hit.transform.position);
                float damageFactor = 1f - Mathf.Clamp01(dist / enemy.enemyData.explosionRadius);
                damageable.TakeDamage(enemy.enemyData.explosionDamage * damageFactor, hit.transform.position, Vector3.up);
            }

            // B. Empuje FÍSICO (Para cajas y barriles - Como el cohete)
            if (hit.attachedRigidbody != null)
            {
                hit.attachedRigidbody.AddExplosionForce(500f, enemy.transform.position, enemy.enemyData.explosionRadius);
            }

            // C. Empuje al PLAYER (Usando tu nuevo PlayerMovement)
            if (hit.CompareTag("Player"))
            {
                PlayerMovement pm = hit.GetComponent<PlayerMovement>();
                if (pm != null)
                {
                    Vector3 dir = hit.transform.position - enemy.transform.position;
                    // Empujamos hacia atrás y un poco hacia arriba para que vuele
                    dir += Vector3.up * 0.5f;
                    pm.AddImpact(dir, enemy.enemyData.explosionForce);
                }
            }
        }

        // 4. Morir (Autodestrucción instantánea)
        // Usamos TakeDamage excesivo para asegurar que dispare eventos de muerte, score, etc.
        var myHealth = enemy.GetComponent<IDamageable>();
        if (myHealth != null)
            myHealth.TakeDamage(9999, enemy.transform.position, Vector3.up);
        else
            Object.Destroy(enemy.gameObject);
    }
}