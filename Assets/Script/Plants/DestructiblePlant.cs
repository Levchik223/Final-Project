using System;
using UnityEngine;

public class DestructiblePlant : MonoBehaviour
{
  
  public event EventHandler OnDestructibleTakeDamage;
  
  private void OnTriggerEnter2D(Collider2D collision)
  {
    if (collision.gameObject.GetComponent<PlayerCombat>() != null)
    {
      Player.Instance.Heal(1);
      OnDestructibleTakeDamage?.Invoke(this, EventArgs.Empty);
      Destroy(gameObject);
      
      NavMeshSurfaceManagement.Instance.RebakeNavMeshSurface();
    }
  }
}
