using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace TileSystemSpace.Tilemap
{
    [CreateAssetMenu(menuName = "2D/Custom/Height Rule Tile")]
    public class CustomRuleTile : RuleTile
    {
        [HideInInspector] public int heightLevel = 0;
        [SerializeField] private List<TileType> excludeNeighbourTiles = new();

        [SerializeField] private TileType tileType;
        
        private Vector2Int offsetPosition;
        
        public override bool RuleMatch(Vector2Int _position, Vector2Int _otherPos, int _neighbor, TileBase _other)
        {
            CustomRuleTile _otherTile = _other as CustomRuleTile;

            if (!_otherTile)
            {
                return true;
            }

            if (!TileSystem.instance)
            {
                return false;
            }
            
            int _currentHeightLevel = TileSystem.instance.GetTile(_position).level;
            int _otherHeightLevel = TileSystem.instance.GetTile(_position + _otherPos).level;
            
            if (_currentHeightLevel == _otherHeightLevel)
            {
                if (excludeNeighbourTiles.Contains(_otherTile.tileType))
                {
                    switch (_neighbor)
                    {
                        case TilingRuleOutput.Neighbor.This: return _other == this;
                        case TilingRuleOutput.Neighbor.NotThis: return _other != this;
                        default: return true;
                    }
                }
            }
            
            if (_currentHeightLevel > _otherHeightLevel)
            {
                switch (_neighbor)
                {
                    case TilingRuleOutput.Neighbor.This: return false;
                    case TilingRuleOutput.Neighbor.NotThis: return true;
                    default: return true;
                }
            }
            
            if (_currentHeightLevel < _otherHeightLevel)
            {
                switch (_neighbor)
                {
                    case TilingRuleOutput.Neighbor.This: return true;
                    case TilingRuleOutput.Neighbor.NotThis: return false;
                    default: return true;
                }
            }

            return true; //WILL NEVER REACH THIS
        }
    }
}