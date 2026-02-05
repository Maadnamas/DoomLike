using UnityEngine;

public class EnemyShooterBehavior : IEnemyBehavior
{
    private float lastShootTime;

    public void OnEnter(EnemyAI enemy)
    {
        Debug.Log($"[SHOOTER] Iniciando comportamiento Shooter en {enemy.name}");

        // Intentar setear animación, con protección por si faltan params
        var anim = enemy.GetAnimator();
        if (anim != null)
        {
            // Solo llamamos si existen para no spammear errores, 
            // pero asumimos que ya creaste los parámeteros
            anim.SetRunning(false);
            anim.SetWalking(false);
            anim.SetIdle(true);
        }

        enemy.StopMovement();
        lastShootTime = Time.time - (enemy.enemyData.attackCooldown * 0.5f);
    }

    public void Execute(EnemyAI enemy)
    {
        if (enemy.player == null)
        {
            Debug.LogWarning("[SHOOTER] No encuentro al Player (variable es null)");
            return;
        }

        float distance = Vector3.Distance(enemy.transform.position, enemy.player.position);

        // 1. ROTACIÓN
        enemy.RotateTowards(enemy.player.position, true, 20f);

        // 2. CHECK DE DISTANCIA
        if (distance > enemy.enemyData.shootRange)
        {
            // Comentar esto si llena mucho la consola
            // Debug.Log($"[SHOOTER] Player muy lejos: {distance}m / Rango: {enemy.enemyData.shootRange}m");
            return;
        }

        // 3. CHECK DE COOLDOWN
        if (Time.time - lastShootTime >= enemy.enemyData.attackCooldown)
        {
            Debug.Log("[SHOOTER] Cooldown listo. Intentando apuntar...");

            // 4. CHECK DE VISIÓN (RAYCAST)
            if (HasLineOfSight(enemy))
            {
                Debug.Log("[SHOOTER] ¡Tengo visión! DISPARANDO.");
                Shoot(enemy);
                lastShootTime = Time.time;
            }
            else
            {
                Debug.Log("[SHOOTER] No tengo línea de visión (Bloqueado).");
            }
        }
    }

    public void OnExit(EnemyAI enemy)
    {
        Debug.Log($"[SHOOTER] Saliendo del estado Shooter");
    }

    private void Shoot(EnemyAI enemy)
    {
        enemy.GetAnimator().PlayAttack();

        if (enemy.enemyData.projectilePrefab != null)
        {
            // 1. PUNTO DE SALIDA:
            // Ajusta el '1.5f' si sale muy de arriba, o el '0.8f' si sale muy adelante.
            Vector3 spawnOrigin = enemy.transform.position + Vector3.up * 1.5f + enemy.transform.forward * 0.8f;

            // 2. PUNTO DE DESTINO (Aim Assist):
            // Apuntamos al PECHO del jugador (Player position suele ser los pies, así que sumamos +1m)
            Vector3 targetPosition = enemy.player.position + Vector3.up * 0.0f;

            // 3. CÁLCULO DE DIRECCIÓN:
            // Vector = Destino - Origen
            Vector3 shootDirection = (targetPosition - spawnOrigin).normalized;

            // 4. ROTACIÓN DE LA BALA:
            // Le decimos a Unity: "Crea una rotación mirando hacia esa dirección"
            Quaternion bulletRotation = Quaternion.LookRotation(shootDirection);

            // 5. INSTANCIAR:
            // Usamos 'bulletRotation' en vez de 'enemy.transform.rotation'
            GameObject bullet = Object.Instantiate(enemy.enemyData.projectilePrefab, spawnOrigin, bulletRotation);

            // Debug para ver la trayectoria
            Debug.DrawLine(spawnOrigin, targetPosition, Color.magenta, 2f);

            // Inicializar datos de la bala
            EnemyProjectile projScript = bullet.GetComponent<EnemyProjectile>();
            if (projScript != null)
            {
                projScript.Initialize(enemy.enemyData.projectileSpeed, enemy.enemyData.damage);
            }
        }
    }

    private bool HasLineOfSight(EnemyAI enemy)
    {
        RaycastHit hit;
        
        // AJUSTE 1: Bajamos un poco la altura de los ojos (de 1.5 a 1.2)
        // para asegurar que salga del pecho/cabeza y no del cielo.
        Vector3 origin = enemy.transform.position + Vector3.up * 1.2f;
        
        // AJUSTE 2: Apuntamos al CENTRO del jugador (o un poco arriba), no a los pies
        Vector3 targetPos = enemy.player.position + Vector3.up * 1.0f;
        
        Vector3 direction = (targetPos - origin).normalized;
        float range = enemy.enemyData.shootRange;

        // Debug Visual (Amarillo = Intento)
        Debug.DrawLine(origin, targetPos, Color.yellow); 

        // AJUSTE 3: Usamos SphereCast (Radio de 0.5f) en vez de Raycast.
        // Es como disparar una pelota de tenis en vez de una aguja.
        // El ~0 es una máscara que significa "Choca con todo".
        if (Physics.SphereCast(origin, 0.5f, direction, out hit, range, ~0, QueryTriggerInteraction.Ignore))
        {
            // Si le pegamos al player...
            if (hit.transform.CompareTag("Player"))
            {
                Debug.DrawLine(origin, hit.point, Color.green);
                return true;
            }
            else
            {
                // Si le pegamos a otra cosa, avisamos qué es
                Debug.DrawLine(origin, hit.point, Color.red);
                Debug.Log($"[SHOOTER] Bloqueado por: {hit.transform.name}");
                return false;
            }
        }
        
        Debug.Log("[SHOOTER] SphereCast no tocó nada (¿El jugador está muy lejos o no tiene Collider?)");
        return false;
    }
}