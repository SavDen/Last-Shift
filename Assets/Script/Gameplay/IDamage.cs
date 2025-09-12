using System;

public interface IDamage
{
    void TakeDamage(float damage, TypeDamage typeDamage);
}

public enum TypeDamage
{
    Fire,
    Blood,
    Flash,
    Smoke
}
