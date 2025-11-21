using System.Collections;
using System.Collections.Generic;
using State.Interfaces;
using UnityEngine;

public class HijaMisions : AEnemyState
{
    //atributos
    private HijaController hija;
    private Coroutine cryRoutine = null;
    //metodos
    public HijaMisions(IEnemy enemy) : base(enemy)
    {
        hija = (HijaController)enemy;
    }

    public override void Enter()
    {
        hija.timeSinceGift = 0;
        hija.timeSinceSeen = 0;
    }

    public override void Exit()
    {
        enemy.GetGameObject().GetComponent<MonoBehaviour>().StopCoroutine(cryRoutine);
        cryRoutine = null;
    }

    public override void FixedUpdate()
    {

    }

    public override void Update()
    {
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
        Debug.Log("La niña está llorando...");

        // Aquí puedes llamar AudioSource.Play();
        // Y también alertar enemigos cercanos


        yield return new WaitForSeconds(3f);  // DURACIÓN DEL LLANTO

        enemy.SetCrying(false);
    }
}
