public interface IDamageable
{
    /// <summary>
    /// Aplica da�o. Devuelve true si el objeto muri� (o si quieres reaccionar).
    /// </summary>
    /// <param name="amount">Cantidad de da�o a aplicar.</param>
    /// <param name="hitPoint">Punto del impacto (opcional).</param>
    /// <param name="hitNormal">Normal del impacto (opcional).</param>
    bool TakeDamage(float amount, UnityEngine.Vector3 hitPoint, UnityEngine.Vector3 hitNormal);
}
