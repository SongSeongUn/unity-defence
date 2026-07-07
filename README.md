# unity-defence Unity 2D Auto Battle Defense

Unity 기반 2D 자동 전투 / 디펜스 스타일 포트폴리오 프로젝트입니다.  
웨이브 단위로 등장하는 몬스터를 자동 타겟팅/스킬 발사로 처치하고, 경험치 획득과 레벨업 선택을 통해 스킬을 강화하는 전투 루프를 구현했습니다.

> 팀 프로젝트로 시작했으나 중도 드랍 이후 1인 개발로 전환하여, 전투 시스템 구조 정리와 핵심 기능 구현을 담당했습니다.

## 프로젝트 정보

- Engine: Unity 6000.3.6f1
- Genre: 2D Auto Battle / Defense
- Platform: PC / Mobile 대응 구조
- 개발 형태: 팀 프로젝트 중단 이후 1인 개발 전환
- 주요 구현: 전투 시스템, 웨이브, 스킬, 투사체, 풀링, HP UI, 테이블 기반 데이터

## 시연
 - Scene/IntroScene 로드 후 실행
 
> 시연 영상 또는 GIF 추가 예정

- 웨이브 몬스터 스폰
- 자동 타겟팅 및 스킬 발사
- 투사체 / 폭발 / 독 / 슬로우 / 체인 라이트닝
- 몬스터 HP UI
- 플레이어 HP UI
- 경험치 획득 및 레벨업 스킬 선택

## 핵심 구현

### 1. 웨이브 기반 전투 흐름

스테이지 진입 후 테이블 데이터를 기반으로 웨이브 정보를 구성하고, 웨이브별 몬스터를 순차적으로 스폰합니다.

- `StageController`: 스테이지 시작 및 전체 흐름 조율
- `StageWaveHandler`: 테이블 기반 웨이브 데이터 구성
- `WaveSpawnHandler`: 웨이브별 몬스터 스폰 루프 처리
- `MonsterSpawnController`: 몬스터 생성 및 반환 관리

### 2. 자동 타겟팅 및 스킬 실행 구조

플레이어는 현재 장착된 스킬의 쿨타임을 확인하고, 가장 가까운 몬스터를 대상으로 자동 공격합니다.

스킬 실행 책임을 분리하여 `PlayerAttackHandler`가 커지지 않도록 구성했습니다.

- `PlayerAttackHandler`: 타겟 탐색, 쿨타임 확인, 스킬 실행 요청
- `SkillExecutionHandler`: 스킬 타입에 따라 실행기 분기
- `ProjectileSkillExecutor`: 일반 투사체 스킬 실행
- `ChainSkillExecutor`: 체인 라이트닝 스킬 실행

### 3. 조합형 투사체 Hit Feature

투사체의 타격 효과를 `IProjectileHitFeature` 단위로 분리했습니다.  
스킬이 추가될 때마다 `ProjectileActor`에 분기문을 늘리지 않고, 프리팹에 필요한 feature를 조합하는 방식으로 확장할 수 있습니다.

구현된 예시:

- 일반 피해
- 폭발 피해
- 독 / 도트 피해
- 슬로우
- 체인 라이트닝

관련 구조:

- `ProjectileActor`: 투사체 생명주기 조율
- `ProjectileMoveHandler`: 투사체 이동 담당
- `ProjectileHitHandler`: 충돌 감지 및 hit feature 호출
- `IProjectileHitFeature`: 타격 효과 확장 인터페이스

### 4. 테이블 기반 스킬 / 강화 데이터

스킬 기본 정보와 강화 정보를 테이블 데이터로 분리했습니다.

- `PlayerSkillRow`: 스킬 기본 정보
- `PlayerSkillUpgradeRow`: 스킬 강화 정보
- `CurrentSkillData`: 현재 장착된 스킬의 런타임 상태
- `SkillAbilityCalculrator`: 기본 스킬 + 강화 데이터를 기반으로 최종 능력치 계산

이를 통해 코드 수정 없이 테이블 값 변경만으로 데미지, 쿨타임, 투사체 수, 효과 범위, 지속시간 등을 조정할 수 있도록 구성했습니다.

### 5. Object Pool 기반 런타임 객체 관리

전투 중 반복 생성되는 객체는 `ObjectPoolManager`를 통해 재사용합니다.

풀링 대상:

- 몬스터
- 투사체
- FX
- 몬스터 HP UI

최근 보강 사항:

- 풀링 객체 중복 반환 방지
- lifetime coroutine 핸들 관리
- 재사용 시 런타임 상태 초기화
- 비활성화/반환 시 공격 루프 중단

### 6. HP UI와 이벤트 기반 갱신

몬스터와 플레이어 피격 시 이벤트를 발행하고, UI는 해당 이벤트를 구독해 HP 상태를 갱신합니다.

- `MonsterDamagedEvent`
- `PlayerDamagedEvent`
- `UIMonsterHpBar`
- `UIPlayerHpBar`
- `UI_MonsterStatus_World`

게임 로직과 UI 갱신을 직접 연결하지 않고 이벤트 기반으로 분리했습니다.

## 기술 스택

- Unity 6000.3.6f1
- C#
- UniTask
- Addressables
- Object Pool
- TextMeshPro
- Table-driven data

## 설계 포인트

### 책임 분리

각 MonoBehaviour가 하나의 명확한 역할을 갖도록 분리했습니다.

예시:

- 이동: `MonsterMoveHandler`, `ProjectileMoveHandler`
- 공격: `MonsterAttackHandler`, `PlayerAttackHandler`
- 스킬 보유/강화: `SkillInventoryController`
- 스킬 선택: `SkillSelectionController`
- 투사체 충돌: `ProjectileHitHandler`
- 타격 효과: `IProjectileHitFeature`

### 확장 가능한 스킬 구조

스킬별 코드를 하나의 클래스에 몰아넣지 않고, 실행 방식과 타격 효과를 분리했습니다.

```mermaid
flowchart TD
    A[PlayerAttackHandler] --> B[SkillExecutionHandler]
    B --> C[ProjectileSkillExecutor]
    B --> D[ChainSkillExecutor]
    C --> E[ProjectileActor]
    E --> F[ProjectileMoveHandler]
    E --> G[ProjectileHitHandler]
    G --> H[IProjectileHitFeature]