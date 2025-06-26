using System.Collections;
using UnityEngine;

// 총을 구현
public class Gun : MonoBehaviour {
    public enum State {
        Ready,     // 발사 준비됨
        Empty,     // 탄알집이 빔
        Reloading  // 재장전 중
    }

    public State state { get; private set; } // 현재 총의 상태

    [Header("Gun Code")]
    public Transform fireTransform; // 탄알이 발사될 위치
    public ParticleSystem muzzleFlashEffect; // 총구 화염 효과
    public ParticleSystem shellEjectEffect; // 탄피 배출 효과
    public GunData gunData; // 총의 현재 데이터
    public int ammoRemain = 100; // 남은 전체 탄알
    public int magAmmo; // 현재 탄알집에 남아 있는 탄알

    private LineRenderer bulletLineRenderer; // 탄알 궤적을 그리기 위한 렌더러
    private AudioSource gunAudioPlayer; // 총 소리 재생기
    private float fireDistance = 50f; // 사정거리
    private float lastFireTime; // 총을 마지막으로 발사한 시점

    private void Awake() {
        gunAudioPlayer = GetComponent<AudioSource>();
        bulletLineRenderer = GetComponent<LineRenderer>();
        bulletLineRenderer.positionCount = 2;
        bulletLineRenderer.enabled = false;
    }

    private void OnEnable() {
        ammoRemain = gunData.startAmmoRemain;
        magAmmo = gunData.magCapacity;
        state = State.Ready;
        lastFireTime = 0;
    }

    public void Fire() {
        if (state == State.Ready && Time.time >= lastFireTime + gunData.timeBetFire) {
            lastFireTime = Time.time;
            Shot();
        }
    }

    private void Shot() {
        RaycastHit hit;
        Vector3 hitPosition = Vector3.zero;

        if (Physics.Raycast(fireTransform.position, fireTransform.forward, out hit, fireDistance)) {
            IDamageable target = hit.collider.GetComponent<IDamageable>();

            if (target != null) {
                target.OnDamage(gunData.damage, hit.point, hit.normal);
            }

            hitPosition = hit.point;
        } else {
            hitPosition = fireTransform.position + fireTransform.forward * fireDistance;
        }

        StartCoroutine(ShotEffect(hitPosition));

        magAmmo--;
        if (magAmmo <= 0) {
            state = State.Empty;
        }
    }

    private IEnumerator ShotEffect(Vector3 hitPosition) {
        muzzleFlashEffect.Play();
        shellEjectEffect.Play();
        gunAudioPlayer.PlayOneShot(gunData.shotClip);

        bulletLineRenderer.SetPosition(0, fireTransform.position);
        bulletLineRenderer.SetPosition(1, hitPosition);
        bulletLineRenderer.enabled = true;

        yield return new WaitForSeconds(0.03f);
        bulletLineRenderer.enabled = false;
    }

    public bool Reload() {
        if (state == State.Reloading || ammoRemain <= 0 || magAmmo >= gunData.magCapacity) {
            return false;
        }

        StartCoroutine(ReloadRoutine());
        return true;
    }

    private IEnumerator ReloadRoutine() {
        state = State.Reloading;
        gunAudioPlayer.PlayOneShot(gunData.reloadClip);
        yield return new WaitForSeconds(gunData.reloadTime);

        int ammoToFill = gunData.magCapacity - magAmmo;
        if (ammoRemain < ammoToFill) {
            ammoToFill = ammoRemain;
        }

        magAmmo += ammoToFill;
        ammoRemain -= ammoToFill;

        state = State.Ready;
    }
}
