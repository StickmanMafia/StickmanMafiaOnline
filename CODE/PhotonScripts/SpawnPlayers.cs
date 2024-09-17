using System.Collections;
using System.Linq;
using Photon.Pun;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class SpawnPlayers : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private GameObject _prefab;

    // Start is called before the first frame update
    private void Start()
    {
        Spawn();
    }

    private void Spawn()
    {
        var targetIndex = 0;

        GetSpawnIndex(ref targetIndex);
        InstantiatePlayer(Resources.LoadAll<SpawnPosition>($"ScriptableObjects/Coordinates/PlayerPositions{PhotonNetwork.CurrentRoom.MaxPlayers}"), targetIndex);
    }
    
    private void InstantiatePlayer(SpawnPosition[] collection, int index)
    {
        var name = _prefab.name;
        var position = collection.ElementAt(index).Position;
        var rotation = collection.ElementAt(index).Rotation;
        var scale = collection.ElementAt(index).Scale;
            
        var newInstance = PhotonNetwork.Instantiate(name, position, rotation);
        newInstance.transform.Rotate(0, rotation.y - 180, 0);
        newInstance.transform.localScale = scale;
        
        var newCustomProps = new Hashtable { { Strings.PositionPropName, index } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(newCustomProps);
    }
    
    private void GetSpawnIndex(ref int index)
    {
        if (!PhotonNetwork.CurrentRoom.Players.All(n => Equals(n.Value, PhotonNetwork.LocalPlayer)))
        {
            for (var i = 1; i < PhotonNetwork.CurrentRoom.MaxPlayers; i++)
            {
                if (PhotonNetwork.CurrentRoom.Players
                    .Where(n => n.Value.CustomProperties.ContainsKey(Strings.PositionPropName) && (int)n.Value.CustomProperties[Strings.PositionPropName] != Numerics.NoId)
                    .All(n => (int)n.Value.CustomProperties[Strings.PositionPropName] != i))
                {
                    index = i;
                    break;
                }
            }    
        }
    }
    
    
}