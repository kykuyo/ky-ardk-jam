using Singleton.Singleton;
using UnityEngine;

namespace Sliders
{
    public class GooColorIndexController : Singleton<GooColorIndexController>
    {
        private const string PLAYER_TEAM_KEY = "PlayerTeam";

        public void SetIndex(int index)
        {
            PlayerPrefs.SetInt(PLAYER_TEAM_KEY, index);
            PlayerPrefs.Save();
        }

        public int GetIndex()
        {
            return PlayerPrefs.GetInt(PLAYER_TEAM_KEY, 0);
        }
    }
}
