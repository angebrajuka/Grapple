using System.Collections.Generic;

public static class PlayerInventory
{
    public static Gun[] guns;
    private static int _currentGun;
    public static Gun CurrentGun { get { return guns[_currentGun]; } }

    public static void Init()
    {
        guns = new Gun[3]{null, null, null};

    }

    public static int _CurrentGun
    {
        get { return _currentGun; }
        set
        {
            _currentGun = value;
            PlayerAnimator.instance.SwitchGun();
        }
    }

}