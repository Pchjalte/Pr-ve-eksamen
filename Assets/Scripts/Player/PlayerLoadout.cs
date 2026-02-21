using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;

public static class PlayerLoadout {

    const string GUN = "g";
    const string ABILITY = "a";

    public static void SetLoadout(GunID gun, AbilityID ability) {

        Hashtable table = new Hashtable();

        table[GUN] = (byte)gun;
        table[ABILITY] = (byte)ability;

        PhotonNetwork.LocalPlayer.SetCustomProperties(table);
    }

    public static GunID GetGun(Player player) {

        if (player.CustomProperties.TryGetValue(GUN, out object g))
            return (GunID)(byte)g;

        return GunID.Pistol;
    }

    public static AbilityID GetAbility(Player player) {

        if (player.CustomProperties.TryGetValue(ABILITY, out object a))
            return (AbilityID)(byte)a;

        return AbilityID.Dash;
    }
}