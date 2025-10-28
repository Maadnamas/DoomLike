public interface IDamageable
{
    /// <summary>
    /// Aplica daño. Devuelve true si el objeto murió (o si quieres reaccionar).
    /// </summary>
    /// <param name="amount">Cantidad de daño a aplicar.</param>
    /// <param name="hitPoint">Punto del impacto (opcional).</param>
    /// <param name="hitNormal">Normal del impacto (opcional).</param>
    bool TakeDamage(float amount, UnityEngine.Vector3 hitPoint, UnityEngine.Vector3 hitNormal);
}
