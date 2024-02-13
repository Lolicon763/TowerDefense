using System;
public class DamageCalculator
{
    public static float CalculateActualDamage(float originalDamage, float defenseValue)
    {
        if (defenseValue == 0)
        {
            return originalDamage;
        }
        float actualDamage = originalDamage / (float)Math.Log(defenseValue + 1);
        return actualDamage;
    }
}
