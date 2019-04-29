using UnityEngine;
using System.Collections;

public struct HurtData {

    public enum Type {
        Normal,
        Physical,
        Magical,
        Real
    }
    public Type type;
    public int skillNum;
    public int damage;
    public int penetration;

}
