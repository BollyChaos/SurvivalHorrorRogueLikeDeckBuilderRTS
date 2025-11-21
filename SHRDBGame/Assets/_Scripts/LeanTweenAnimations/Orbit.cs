using UnityEngine;

public class Orbit : MonoBehaviour
{
    [SerializeField] Transform target;   // objeto alrededor del que rotas
    [SerializeField] float radius = 4f;
    [SerializeField] float speed = 1f;   // vueltas por segundo
  Vector3 initialOffset;
    float angle;
public void InitOrbit(Transform trg,float rd,float spd)
    {
        target=trg;
        radius=rd;
        speed=spd;
         // Guardamos el offset desde la posición actual al centro
        initialOffset = transform.position - target.position;

        // Calculamos el ángulo inicial según ese offset
        angle = Mathf.Atan2(initialOffset.z, initialOffset.x);
    }

    void Update()
    {
        angle += speed * Time.deltaTime;

        float radius = initialOffset.magnitude;

        Vector3 newPos = new Vector3(
            Mathf.Cos(angle) * radius,
            initialOffset.y,
            Mathf.Sin(angle) * radius
        );

        transform.position = target.position + newPos;
    }
}
