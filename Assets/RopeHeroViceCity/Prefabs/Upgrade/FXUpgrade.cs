using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;

public class FXUpgrade : MonoBehaviour
{
    private ParticleSystem particleSystem;
    [SerializeField] private Image cell_1;
    [SerializeField] private Image cell_2;
    [SerializeField] private Image cell_3;
    [SerializeField] private Image cell_4;
    private void Start()
    {
        Anim();
    }
    private void Anim()
    {
        cell_1.transform.DOMoveY(cell_1.transform.position.y + 100f, 1f).SetUpdate(true);
        cell_2.transform.DOMoveY(cell_2.transform.position.y + 95f, 1f).SetDelay(0.1f).SetUpdate(true);
        cell_3.transform.DOMoveY(cell_3.transform.position.y + 95f, 1f).SetDelay(0.1f).SetUpdate(true);
        cell_4.transform.DOMoveY(cell_4.transform.position.y + 90, 1f).SetDelay(0.2f).SetUpdate(true);
        //
        cell_1.DOFade(1, 0.3f).SetDelay(0f).SetUpdate(true).OnComplete(() =>
        {
            cell_1.DOFade(0, 0.3f).SetUpdate(true);
        });
        cell_2.DOFade(1, 0.3f).SetDelay(0.1f).SetUpdate(true).OnComplete(() =>
        {
            cell_2.DOFade(0, 0.3f).SetUpdate(true);
        });
        cell_3.DOFade(1, 0.3f).SetDelay(0.1f).SetUpdate(true).OnComplete(() =>
        {
            cell_3.DOFade(0, 0.3f).SetUpdate(true);
        });
        cell_4.DOFade(1, 0.3f).SetDelay(0.2f).SetUpdate(true).OnComplete(() =>
        {
            cell_4.DOFade(0, 0.3f).SetUpdate(true).OnComplete(() =>
            {
                Destroy(this.gameObject);
            });
        });
    }
}