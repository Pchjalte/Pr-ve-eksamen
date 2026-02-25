using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public static class PlayerLoadout {

    const string GUN = "gun";
    const string ABILITY = "ability";

    public static void SetLoadout(GunID gun, AbilityID ability) {

        Hashtable hash = new Hashtable();
        hash[GUN] = (int)gun;
        hash[ABILITY] = (int)ability;

        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
    }

    public static GunID GetGun(Player player) {

        if (player.CustomProperties.TryGetValue(GUN, out object value))
            return (GunID)(int)value;

        return GunID.Revolver;
    }

    public static AbilityID GetAbility(Player player) {

        if (player.CustomProperties.TryGetValue(ABILITY, out object value))
            return (AbilityID)(int)value;

        return AbilityID.Dash;
    }
}