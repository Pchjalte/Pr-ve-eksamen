using Photon.Pun;
using UnityEngine;

public class LoadoutIDSelect : MonoBehaviour {

    public void SelectGun(int id) {
        var player = PhotonNetwork.LocalPlayer;

        GunID gun = (GunID)id;
        AbilityID ability = PlayerLoadout.GetAbility(player);

        PlayerLoadout.SetLoadout(gun, ability);
    }

    public void SelectAbility(int id) {
        var player = PhotonNetwork.LocalPlayer;

        AbilityID ability = (AbilityID)id;
        GunID gun = PlayerLoadout.GetGun(player);

        PlayerLoadout.SetLoadout(gun, ability);
    }
}