using Singleton.Singleton;
using UnityEngine;

namespace Sliders
{
    public class GooColorIndexController : Singleton<GooColorIndexController>
    {
        private const string TeamIndexKey = "TeamIndex";

        public void SetIndex(int index)
        {
            PlayerPrefs.SetInt(TeamIndexKey, index);
            PlayerPrefs.Save();
        }

        public int GetIndex()
        {
            return PlayerPrefs.GetInt(TeamIndexKey, 0);
        }
    }
}