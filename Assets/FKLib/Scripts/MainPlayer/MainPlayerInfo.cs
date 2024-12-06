using UnityEngine;
//============================================================
namespace FKLib
{
    public class MainPlayerInfo
    {
        private string _tag = "MainPlayer";
        public MainPlayerInfo(string tag) { _tag = tag; }

        private GameObject _gameObject;
        public GameObject GameObject 
        {  
            get 
            {
                if(_gameObject == null)
                {
                    GameObject[] players = GameObject.FindGameObjectsWithTag(_tag);
                    for (int i = 0; i < players.Length; i++)
                    {
                        GameObject player = players[i];
                        _gameObject = player;
                    }
                }
                return _gameObject;
            } 
        }
        // this is a virtual player, shouldn't have other attributes.
    }
}
