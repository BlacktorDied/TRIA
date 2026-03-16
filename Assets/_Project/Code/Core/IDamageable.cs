namespace TRIA.Core
{
    // This is the "Bridge". Both Player and Enemies will use this
    // so they don't have to talk to each other directly.
    public interface IDamageable
    {
        void TakeDamage(float amount);
    }
}
