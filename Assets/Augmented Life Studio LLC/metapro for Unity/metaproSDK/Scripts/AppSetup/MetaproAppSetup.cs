using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "MetaproAppSetup", menuName = "Metapro/AppSetup")]
public class MetaproAppSetup : ScriptableObject
{
    public string SelectedChain;
    public string GameKey;
    public string AppId;

    public string GameImageURL;
    public string GameName;
    public string TeamName;

    public List<AvailableAsset> GameAssets;

}
