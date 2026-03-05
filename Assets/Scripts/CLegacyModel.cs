using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Assertions;

public class CLegacyModel : MonoBehaviour
{
    #region 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓 인스펙터 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓
    [Header("변수")]
    [SerializeField] private Animation _targetAnimation;

    // 애니메이션을 재생하기 위한 핵심 컴포넌트입니다.
    [Header("클립")]
    [SerializeField] private AnimationClip _idleClip;
    [SerializeField] private AnimationClip _runClip;
    [SerializeField] private AnimationClip _actionClip;

    // 레거시 상태 전환 코드를 작성해볼건데 메카님도 딱히 다르지 않습니다.
    [Header("옵션")]
    [SerializeField] private bool _useCrossFade = true; // 부드럽게 섞어서 넘어갈지 여부
    [SerializeField] private float _fadeTime = 0.15f; // 얼마나 오래 섞을지? (블렌딩 시간)
    [SerializeField] private float _actionSpeed = 1f; // 1회 동작 재생 속도

    [Header("루트 모션")]
    [SerializeField] private bool _forceStayInPlace = false; // 부드럽게 섞어서 넘어갈지 여부
    [SerializeField] private Transform _rootTrans; // 부드럽게 섞어서 넘어갈지 여부
    #endregion

    #region 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓 내부 변수 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓
    private Coroutine _actionRoutine; // 1회 재생은 코루틴으로 처리할 에정
    #endregion

    #region 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓 내부 메서드 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓
    private void RegisterClip(AnimationClip clip, WrapMode wrap = WrapMode.Default, float speed = 1f)
    {
        if (clip == null) {
            Debug.LogWarning("클립이 비어있습니다.registerClip");
            return;
        }
        if (_targetAnimation.GetClip(clip.name) != null) {
            print("클립이 이미 등록되어 있습니다.");
            return;
        }
        _targetAnimation.AddClip(clip, clip.name);
    }
    private void PlayLoop(AnimationClip clip)
    {
        if (clip == null) {
            Debug.LogWarning("클립이 비어있습니다. playLoop");
            return;
        }

        if (_actionRoutine != null) {
            StopCoroutine(_actionRoutine);
            _actionRoutine = null;
        }

        AnimationState state = _targetAnimation[clip.name]; // 이건 배열처럼 접근하네
        if (state == null) {
            Debug.LogWarning("스테이트가 비어있음 ... PlayLoop 실패");
            return;
        }

        // 루프 설정
        state.wrapMode = WrapMode.Loop;
        state.speed = 1f;

        if (_useCrossFade) {
            _targetAnimation.CrossFade(clip.name, _fadeTime); // 어떻게 자연스럽게 하나 했는데 그냥 기능이 있네
        } else {
            _targetAnimation.Play(clip.name);
        }
    }

    private void PlayOnce(AnimationClip clip, float speed)
    {
        // 아무것도 안할 때 기본은 idle, 무언가 동작이 끝나면 대부분 idle로 복귀한다.
        if(clip == null) {
            Debug.LogWarning("PlayOnce 실패했습니다. 클립이 비었습니다.");
            return;
        }

        // 기존 모션 제거 (교체할 것)
        if(_actionRoutine != null) {
            StopCoroutine(_actionRoutine);
            _actionRoutine = null;
        }
        _actionRoutine = StartCoroutine(Co_PlayOnce(clip, speed));

    }

    private IEnumerator Co_PlayOnce(AnimationClip clip, float speed)
    {
        AnimationState state = _targetAnimation[clip.name];
        if(state == null) {
            _actionRoutine = null;
            yield break;
        }

        state.wrapMode = WrapMode.Once;
        state.speed = Mathf.Max(0.1f, speed); // 0이면 멈춰버리기 때문에 최솟값 보장

        if (_useCrossFade) {
            _targetAnimation.CrossFade(clip.name, _fadeTime);
            
        } else {
            _targetAnimation.Play(clip.name);
        }
        // 모델러가 작업한 실제 애니메이션 길이를 확인해 보고 테스트하는게 디테일 면에서는 더 좋다는 것
        float wait = clip.length / state.speed;
        yield return new WaitForSeconds(wait);

        if(_idleClip != null) {
            PlayLoop(_idleClip);
        } else {
            StopAll();
        }
        _actionRoutine = null;
    }

    private void StopAll()
    {
        if (_actionRoutine != null) {
            StopCoroutine(_actionRoutine);
            _actionRoutine = null;
        }
        _targetAnimation.Stop();
        print("모델 정지");
    }

    #endregion

    #region 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓 메시지 함수 〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓〓
    private void Start()
    {
        if(_targetAnimation == null) {
            _targetAnimation = GetComponentInChildren<Animation>();
        }
        if(_targetAnimation == null) {
            print("애니메이션 왜 없음");
            enabled = false;
            return;
        }

        RegisterClip(_idleClip);
        RegisterClip(_runClip);
        RegisterClip(_actionClip);
        PlayLoop(_idleClip);
        print("레거시 모델 준비 완료");
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            PlayLoop(_idleClip);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2)) {
            PlayLoop(_runClip);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3)) {
            PlayOnce(_actionClip, _actionSpeed);
        }
        if (Input.GetKeyDown(KeyCode.Space)) {
            StopAll();
        }
    }

    private void LateUpdate()
    {
        // 루트 모션을 건드리겠다는 것은 개발자의 범위를 한참 벗어난 영역입니다.
        // 모든 애니메이션이 영향을 받을 여지가 있습니다.
        // 루트모션 빡세게 넣어놓은 모델은 달리면 루트 모션이 계속 왔다갔다한다.
        // 이런거 보면 '수정할까?' 가 아니라 '바꿔야겠다' 를 지향할 것
        // 실제 업무로 가도 각자 모여서 회의하고 결정하고, PD가 관리해야지. 개발자가 그렇게 신경쓸 일은 아님
        if (!_forceStayInPlace)
            return;

    }
    #endregion
}
