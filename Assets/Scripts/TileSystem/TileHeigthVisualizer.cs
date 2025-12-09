using UnityEngine;
using TMPro;
namespace TileSystemSpace
{
    public class TileHeigthVisualizer : MonoBehaviour
    {
        void ShowTileHeigth()
        {
            TileSystem.instance.GetSize();
            for (int x = 0; x < TileSystem.instance.GetSize().x; x++)
            {
                for (int y = 0; y < TileSystem.instance.GetSize().y; y++)
                {

                }
            }
        }
    }
}