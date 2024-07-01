using TMPro;
using UnityEngine;

namespace Assets.CodeBase.Combat.Health.UI
{
    public class HealthBarPlayerName : MonoBehaviour
    {
        [SerializeField] private TMP_Text _playerNameText;

        public void SetPlayerName(string playerName) => 
            _playerNameText.text = playerName;
    }
}
