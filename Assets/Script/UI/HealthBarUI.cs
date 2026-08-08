using System;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
  [SerializeField] private Image healthFill;

  private void Update()
  {
    healthFill.fillAmount = (float) Player.Instance.CurrentHealth / Player.Instance.MaxHealth;
  }
}
