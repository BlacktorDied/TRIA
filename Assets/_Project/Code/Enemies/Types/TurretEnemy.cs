//using UnityEngine;

//namespace TRIA.Enemies
//{
//    public class TurretEnemy : Enemy
//    {
//        [Header("Shooting")]
//        [SerializeField]
//        private GameObject bulletPrefab;

//        [SerializeField]
//        private Transform shootPoint;

//        [SerializeField]
//        private float fireRate = 1.5f;

//        [SerializeField]
//        private float range = 10f;

//        private float _fireTimer;

//        private void Update()
//        {
//            if (isDead || playerTransform == null)
//                return;

//            float dist = Vector2.Distance(transform.position, playerTransform.position);
//            if (dist > range)
//                return;

//            _fireTimer += Time.deltaTime;
//            if (_fireTimer >= fireRate)
//            {
//                Shoot();
//                _fireTimer = 0;
//            }
//        }

//        private void Shoot()
//        {
//            Vector2 dir = (playerTransform.position - shootPoint.position).normalized;
//            GameObject bullet = Instantiate(bulletPrefab, shootPoint.position, Quaternion.identity);
//            // Assuming your projectile has a SetDirection method
//            bullet.GetComponent<Projectile>()?.SetDirection(dir);
//        }
//    }
//}
