using Assets.CodeBase.Teams;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.CodeBase.Combat.Health.UI
{
    public class HealthBarColor : MonoBehaviour
    {
        [SerializeField] private Image _healthBorder;
        [SerializeField] private Image _healthFill;

        private static Dictionary<TeamType, Color> _colors =
            new Dictionary<TeamType, Color> {
                [TeamType.None] = Color.white,
                [TeamType.Blue] = Color.cyan,
                [TeamType.Orange] = Color.yellow
            };

        public void ResetColor() =>
            SetColor(_colors[TeamType.None]);

        public void SetColorByTeam(TeamType team) =>
            SetColor(_colors[team]);

        private void SetColor(Color color) {
            _healthBorder.color = color;
            _healthFill.color = color;
        }
    }
}
