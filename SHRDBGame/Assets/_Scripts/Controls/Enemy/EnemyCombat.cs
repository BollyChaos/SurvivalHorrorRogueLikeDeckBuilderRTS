using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCombat : MonoBehaviour
{
    public Stats stats;

    public float damageCooldown = 1f;
    private float lastAttackTime = 0f;

    private EnemyController enemyController;
    private List<Renderer> renderers = new List<Renderer>();
    private MaterialPropertyBlock block;
    private Coroutine flashRoutine;
    private Coroutine hitStopRoutine;

    [Header("Hit Stop")]
    public float hitStopDuration = 0.1f;   // cuanto dura el impacto
    public float slowTimeScale = 0.1f;      // 10% de la velocidad normal
    // Duración del efecto de color rojo (en segundos)
    public float damageColorDuration = 0.25f;

    protected void OnEnable()
    {
        stats.ResetStats();
        enemyController = GetComponent<EnemyController>();
        renderers.Clear();
        GetComponentsInChildren(renderers);

        block = new MaterialPropertyBlock();
        foreach (Renderer r in renderers)
        {
            r.GetPropertyBlock(block);
            block.SetFloat("_ColorValue", 0.0f);
            r.SetPropertyBlock(block);
        }
    }
    protected void OnDisable()
    {
        // Detener cualquier corrutina activa al desactivar el objeto
        if (flashRoutine != null)
        {
            StopCoroutine(flashRoutine);
            flashRoutine = null;
        }
        
        Time.timeScale = 1f;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("EnemyVision")) return;

        PlayerCombat player = collision.gameObject.GetComponent<PlayerCombat>();

        if (player != null && Time.time >= lastAttackTime + damageCooldown)
        {
            Attack(player);
            lastAttackTime = Time.time;
        }
    }

    public void Attack(PlayerCombat player)
    {
        if (player != null && player.stats.IsAlive())
        {
            player.TakeDamage(stats.Attack);

            if (enemyController != null)
            {
                enemyController.AttackPlayer();
            }
        }
    }

    public void TakeDamage(float amount)
    {
        stats.TakeDamage(amount);

        //  Iniciar el efecto de color al recibir daño
        if (renderers != null)
        {
            flashRoutine = StartCoroutine(FlashRed());
        }
        //  Iniciar el efecto de hit stop
        hitStopRoutine = StartCoroutine(HitStop());

        if (!stats.IsAlive())
        {
            if (enemyController != null)
                gameObject.GetComponent<EnemyController>().ShootDrops();

            LevelManager.Instance.AddEnemyKill();
            gameObject.SetActive(false);
        }
    }
    private IEnumerator FlashRed()
    {
        foreach (Renderer r in renderers)
        {
            r.GetPropertyBlock(block);
            block.SetFloat("_ColorValue", 0.6f);
            r.SetPropertyBlock(block);
        }

        yield return new WaitForSeconds(damageColorDuration);

        foreach (Renderer r in renderers)
        {
            r.GetPropertyBlock(block);
            block.SetFloat("_ColorValue", 0f);
            r.SetPropertyBlock(block);
        }
    }

    private IEnumerator HitStop()
    {
        // Guardar valores originales
        float originalTimeScale = Time.timeScale;
        //float originalFixedDeltaTime = Time.fixedDeltaTime;

        // Ralentizar tiempo
        Time.timeScale =0.02f;
        //Time.fixedDeltaTime = 0.02f * Time.timeScale;

        // Esperar en tiempo REAL
        yield return new WaitForSecondsRealtime(hitStopDuration);

        // Restaurar tiempo
        Time.timeScale = originalTimeScale;
        //Time.fixedDeltaTime = originalFixedDeltaTime;
    }
}