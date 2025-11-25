using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using DG.Tweening;
using UnityEngine.Rendering.Universal;

public class FlashVolumeEffect : MonoBehaviour
{
    [SerializeField] private GameObject flashEffectObject, flashEffectUI;
    private Coroutine _activeCorutine;

    public void StartEffect(float duration)
    {
        if(_activeCorutine == null)
        _activeCorutine = StartCoroutine(FlashEffect(duration));
    }

    public IEnumerator FlashEffect(float duration)
    {
        flashEffectUI.SetActive(true);
        yield return new WaitForSeconds(1);

        AnimFlashEffectVolume(duration, flashEffectObject.GetComponent<Volume>());
        yield return new WaitForSeconds(duration);
        //flashEffectObject.SetActive(false);
        flashEffectUI.SetActive(false);
        //ResetVolumeIntensity();
        _activeCorutine = null;

    }

    private void AnimFlashEffectVolume(float duration, Volume volumeFlash)
    {
       
        volumeFlash.weight = 1;

        DOTween.To(
                () => volumeFlash.weight,
                x => volumeFlash.weight = x,
                0f,                    // до значения 0
                duration         // за 3 секунды
            ).SetEase(Ease.OutQuad);
    }
}
