using UnityEngine;
using GamePush;
using TMPro;
public class Info : MonoBehaviour
{
    [SerializeField] private TMP_Text _playerId;

    private void Start() => _playerId.text = GP_Player.GetID().ToString();
}