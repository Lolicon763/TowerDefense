using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "MonstersStats", menuName = "Tower Defense/MonstersStats")]
public class MonstersStats : ScriptableObject
{
    public int Health;
    public int PhysicResistance;
    public int MagicResistance;
    public int Speed;
    public int Damage;
    public float AttackSpeed;
    public string MonsterName;
    public int Aggressiveness;
    public int MinusCastleHealth;

    // �s�W��k�Ӯھڤ�ҥͦ��@�ӷs��MonstersStats���
    public MonstersStats CloneWithRatio(float ratio)
    {
        // �ϥ�CreateInstance�ӳЫؤ@�ӷs��MonstersStats���
        MonstersStats clone = ScriptableObject.CreateInstance<MonstersStats>();

        clone.Health = Mathf.RoundToInt(this.Health * ratio);
        clone.PhysicResistance = Mathf.RoundToInt(this.PhysicResistance * ratio);
        clone.MagicResistance = Mathf.RoundToInt(this.MagicResistance * ratio);
        clone.Speed = Mathf.RoundToInt(this.Speed * ratio);
        clone.Damage = Mathf.RoundToInt(this.Damage * ratio);
        clone.AttackSpeed = this.AttackSpeed;
        clone.MonsterName = this.MonsterName; // �W�٥i�ण�ݭn�ܤ�
        clone.Aggressiveness = Mathf.RoundToInt(this.Aggressiveness * ratio);
        clone.MinusCastleHealth = Mathf.RoundToInt(this.MinusCastleHealth * ratio);

        return clone;
    }
}

