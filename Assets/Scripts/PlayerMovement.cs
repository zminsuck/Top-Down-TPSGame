using UnityEngine;

// 플레이어 캐릭터를 사용자 입력에 따라 움직이는 스크립트
public class PlayerMovement : MonoBehaviour {
    public float moveSpeed = 5f; // 앞뒤 움직임의 속도
    public float rotateSpeed = 180f; // 좌우 회전 속도


    private PlayerInput playerInput; // 플레이어 입력을 알려주는 컴포넌트
    private Rigidbody playerRigidbody; // 플레이어 캐릭터의 리지드바디
    private Animator playerAnimator; // 플레이어 캐릭터의 애니메이터

    private void Start()
    {
        // 사용할 컴포넌트들의 참조를 가져오기
        playerInput = GetComponent<PlayerInput>();
        playerRigidbody = GetComponent<Rigidbody>();
        playerAnimator = GetComponent<Animator>();
    }

    // FixedUpdate는 물리 갱신 주기에 맞춰 실행됨
    private void FixedUpdate()
    {
        // 물리 갱신 주기마다 움직임, 회전, 애니메이션 처리 실행
        Rotate(); // Rotate 메서드를 가져옴
        Move(); // Movement 상태의 블렌드 트리에서 사용 & Move 메서드를 가져옴

        // 입력값에 따라 애니메이터의 Move 피라미터 값 변경
        playerAnimator.SetFloat("Move", playerInput.move);
    }

    // 입력값에 따라 캐릭터를 앞뒤로 움직임
    private void Move()
    {
        // 상대적으로 이동할 거리 계산
        // 감지된 사용자 입력 값 -> 이동할 거리와 방향은 앞쪽 반향 * 속력 * 시간
        Vector3 moveDistance = playerInput.move * transform.forward * moveSpeed * Time.deltaTime;
        // 리지드 바디로 게임 오브젝트 위치 변경
        // 현재 위치 + 상대적으로 더 이상할 거리
        playerRigidbody.MovePosition(playerRigidbody.position + moveDistance);
    }

    // 입력값에 따라 캐릭터를 좌우로 회전
    private void Rotate()
    {
        // 상대적으로 회전할 수치 계산
        // 사용자 입력(playerInput.rotate) * 회전 속도(rotateSpeed) * 시간(Time.deltaTime)
        float turn = playerInput.rotate * rotateSpeed * Time.deltaTime;
        // 리지드바디를 이용해 게임 오브젝트 회전 변경
        playerRigidbody.rotation = playerRigidbody.rotation * Quaternion.Euler(0, turn, 0f);
    }
}