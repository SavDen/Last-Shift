using UnityEngine;

public class ProceduralZombieRun : MonoBehaviour
{
    // Ссылки на кости (назначаются вручную через Inspector)
    public Transform Hips;
    public Transform UpperLeg_L, LowerLeg_L, Ankle_L, Ball_L;
    public Transform UpperLeg_R, LowerLeg_R, Ankle_R, Ball_R;
    public Transform Spine_01; // Для качания корпуса

    // Параметры бега
    public float runSpeed = 3f;
    public float legSwingAngle = 45f;
    public float ankleAngle = 20f;
    public float stepFrequency = 8f;

    private float timer;

    void Update()
    {
        // Движение вперёд
        transform.position += transform.forward * runSpeed * Time.deltaTime;

        // Обновляем таймер
        timer += Time.deltaTime * stepFrequency;

        // Анимация ног (используем синус для плавности)
        float legPhase = Mathf.Sin(timer);
        UpperLeg_L.localRotation = Quaternion.Euler(legPhase * legSwingAngle, 0, 0);
        UpperLeg_R.localRotation = Quaternion.Euler(-legPhase * legSwingAngle, 0, 0);

        // Дополнение для голеней/ступней
        LowerLeg_L.localRotation = Quaternion.Euler(Mathf.Abs(legPhase) * ankleAngle, 0, 0);
        LowerLeg_R.localRotation = Quaternion.Euler(Mathf.Abs(legPhase) * ankleAngle, 0, 0);

        // Качание бёдер и корпуса
        Hips.localRotation = Quaternion.Euler(Mathf.Sin(timer * 0.5f) * 10f, 0, 0);
        Spine_01.localRotation = Quaternion.Euler(Mathf.Sin(timer * 0.3f) * 5f, 0, 0);
    }
}