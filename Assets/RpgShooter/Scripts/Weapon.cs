public enum WeaponType
{
    Pistol,
    Revolver,
    AutoRifle,
    Shotgun,
    Rfile
}
[System.Serializable]
public class Weapon 
{
    public WeaponType weaponType;
    public int ammo;
    public int maxAmmo;
}
