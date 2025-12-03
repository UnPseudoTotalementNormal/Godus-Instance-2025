using UnityEngine;
using UnityEngine.Tilemaps;

namespace TileSystemSpace.Tilemap
{
    [CreateAssetMenu(menuName = "2D/Custom/Height Rule Tile")]
    public class CustomRuleTile : RuleTile
    {
        [HideInInspector] public int heightLevel = 0;

        public override bool RuleMatch(int neighbor, TileBase other)
        {
            CustomRuleTile _otherTile = other as CustomRuleTile;
            if (_otherTile&& _otherTile.heightLevel <= heightLevel)
            {
                switch (neighbor)
                {
                    case TilingRuleOutput.Neighbor.This: return other == this;
                    case TilingRuleOutput.Neighbor.NotThis: return other != this;
                    default: return true;
                }
            }

            return true;
        }
    }
}