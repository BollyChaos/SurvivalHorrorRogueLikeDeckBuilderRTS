using System.Collections;
using System.Collections.Generic;
using State.Interfaces;
using UnityEngine;

public class HijaMisions : AEnemyState
{
    //atributos
    private HijaController hija;
    private Coroutine cryRoutine = null;
    private EnemyManager enemyManager;
    //metodos
    public HijaMisions(IEnemy enemy) : base(enemy)
    {
        hija = (HijaController)enemy;
    }

    public override void Enter()
    {
        enemyManager = GameObject.FindObjectOfType<EnemyManager>();
        hija.timeSinceGift = 0;
        hija.timeSinceSeen = 0;
        Debug.Log("Enter State HijaMisions");
    }

    public override void Exit()
    {
        if (cryRoutine != null)
        {
            enemy.GetGameObject().GetComponent<MonoBehaviour>().StopCoroutine(cryRoutine);
            cryRoutine = null;
        }
    }

    public override void FixedUpdate()
    {

    }

    public override void Update()
    {
        if(enemy.GetGameObject().GetComponent<EnemyCombat>().stats.CurrentHealth < enemy.GetGameObject().GetComponent<EnemyCombat>().stats.MaxHealth)
        {
            enemy.SetState(new HijaBattling(enemy));
            return;
        }
        if (enemy.AreMisionsCompleted())
        {
            enemy.SetState(new HijaWaiting(enemy));
            return;
        }

        hija.timeSinceGift += Time.deltaTime;

        if (!enemy.IsPlayerInRoom())
        {
            hija.timeSinceSeen += Time.deltaTime;
        }
        else
        {
            hija.timeSinceSeen = 0f;
        }

        // Calcular enfado (0 a 1)
        hija.enfado = Mathf.Clamp01(hija.timeSinceGift / hija.enfadoMax);

        // Calcular aburrimiento (0 a 1)
        hija.aburrimiento = Mathf.Log(1 + hija.timeSinceSeen) / Mathf.Log(1 + hija.aburrimientoMax);


        // Calcular ganas de llorar
        hija.ganasDeLlorar = 0.3f * hija.enfado + 0.7f * hija.aburrimiento;

        // 6) Si enfado llega al máximo → atacar
        if (hija.enfado >= 1f)
        {
            enemy.SetState(new HijaBattling(enemy));
            return;
        }

        // 7) Si ganas de llorar supera umbral → llorar
        if (hija.ganasDeLlorar >= hija.llorarUmbral && !hija.IsCrying())
        {
            //Llorar
            cryRoutine = enemy.GetGameObject().GetComponent<MonoBehaviour>().StartCoroutine(CryCoroutine());
        }
        //parar de llorar si el jugador vuelve a entrar en la sala o da un regalo 
        if (hija.IsCrying() && hija.ganasDeLlorar < hija.llorarUmbral)
        {
            // parar llanto inmediatamente
            enemy.SetCrying(false);

            if (cryRoutine != null)
            {
                enemy.GetGameObject().GetComponent<MonoBehaviour>().StopCoroutine(cryRoutine);
                cryRoutine = null;
            }

        }
    }
    public IEnumerator CryCoroutine()
    {
        enemy.SetCrying(true);

        // Reproducir sonido
        ASoundPlayer audioSource = enemy.GetGameObject().GetComponent<ASoundPlayer>();
        if (audioSource != null)
        {
            audioSource.PlayRandomSound();
        }
        Debug.Log("La niña está llorando...");

        // Aquí puedes llamar AudioSource.Play();
        // Y también alertar enemigos cercanos

        //llama a los otros enemigos para que vayan
        enemyManager.OnSoundHeard(enemy.GetGameObject().transform.position);
        yield return new WaitForSeconds(3f);  // DURACIÓN DEL LLANTO

        audioSource.StopSound();
        enemy.SetCrying(false);
    }
}
