using AI;
using UnityEngine;

public class Entity : MonoBehaviour, ITeamComponent
{
    [field:SerializeField] public EntityTeam team { get; private set; }
}

