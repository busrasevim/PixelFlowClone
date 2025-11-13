using DG.Tweening;
using UnityEngine;

namespace _Project.Scripts.Game
{
    public class Bullet : MonoBehaviour, IPoolObject
    {
        public void Fire(ColorCube targetCube, float bulletSpeed, Ease bulletFireEase, ShooterManager shooterManager)
        {
            transform.DOMove(
                targetCube.getBulletTr.position - transform.forward * (0.15f * targetCube.transform.localScale.x),
                bulletSpeed).SetSpeedBased().SetEase(bulletFireEase).OnComplete(() =>
            {
                targetCube.Blast(shooterManager);
                transform.DOScale(Vector3.zero, 0.1f);
            });
        }

        public void Reset()
        {
            transform.localScale = Vector3.one;
        }
    }
}