using System;
using Unity.Behavior;
using UnityEngine;
using Unity.Properties;

#if UNITY_EDITOR
[CreateAssetMenu(menuName = "Behavior/Event Channels/OnAttacked")]
#endif
[Serializable, GeneratePropertyBag]
[EventChannelDescription(name: "OnAttacked", message: "[Self] was Attacked by [Enemy]", category: "Events", id: "620beb3c240652003c2d6844d27f5987")]
public sealed partial class OnAttacked : EventChannel<GameObject, GameObject> { }

