using TMPro;
using UnityEngine;

namespace Assets.CodeBase.Combat.Health.UI
{
    public class HealthBarCounter : MonoBehaviour
    {
        [SerializeField] private TMP_Text _healthCountText;

        public void SetHealthCount(float healthCount) =>
            _healthCountText.text = ((int)healthCount).ToString();
    }
}
