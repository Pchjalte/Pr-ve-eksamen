using Photon.Pun;
using UnityEngine;

public class LoadoutIDSelect : MonoBehaviour {

    public void SelectGun(int id) {

        GunID gun = (GunID)id;

        AbilityID currentAbility = PlayerLoadout.GetAbility(PhotonNetwork.LocalPlayer);

        PlayerLoadout.SetLoadout(gun, currentAbility);
    }

    public void SelectAbility(int id) {

        AbilityID ability = (AbilityID)id;

        GunID currentGun = PlayerLoadout.GetGun(PhotonNetwork.LocalPlayer);

        PlayerLoadout.SetLoadout(currentGun, ability);
    }
}