<details open>
<summary>EN</summary>


## 2.0.0 (2026-11-11)

##### Added
- Added `AttAuthorized(bool authorized)` API to notify ATT (App Tracking Transparency) authorization status to the SDK. [iOS]
- Added `DFConfig.IOS_LOG_LEVEL` configuration key and `DFIOSLogLevel` constants for iOS log level control. [iOS]
- Added Swift Package Manager support for iOS native dependencies.

##### Changed
- Deprecated `DFConfig.IOS_LOG_ENABLE` in favor of `DFConfig.IOS_LOG_LEVEL`. [iOS]

##### Compatibility
- Verified compatibility with Xcode 27 and iOS 27 UIScene lifecycle requirements.
- Dfinery SDK does not use deeplink/Universal Link APIs, so no SceneDelegate migration is required.
- Push notification and SDK initialization APIs remain unaffected by the SceneDelegate transition.

##### Notes
- For iOS 27 / Xcode 27 builds, Unity 2022.3.72f1 or later is recommended for UIScene lifecycle support.

##### Updated
- Updated README documentation with EDM and EDM4U installation guides.

##### Versions

- **Android**
  - Dfinery Android SDK 2.7.4
  - Dfinery Android Unity Extension 1.0.0
- **iOS**
  - Dfinery iOS SDK 2.5.1 (CocoaPods) / 2.6.1 (Swift Package Manager)

---

## 1.0.1 (2026-01-15)

##### Fixed
- Improved logic for precise matching between events, identifiers, and device information has been applied, enabling accurate contextual data collection at the time of occurrence in any situation. [Android]
- Improved internal logic within setIdentity() to handle edge cases more reliably. [iOS]

##### Versions

- **Android**
  - Dfinery Android SDK 2.4.6
  - Dfinery Android Unity Extension 1.0.0
- **iOS**
  - Dfinery iOS SDK 2.3.4
## 1.0.0 (2025-10-10)

##### Notes

- Initial release

##### Versions

- **Android**
  - Dfinery Android SDK 2.3.5
  - Dfinery Android Unity Extension 1.0.0
- **iOS**
  - Dfinery iOS SDK 2.2.1

</details>

<details open>
<summary>KO</summary>


## 2.0.0 (2026-11-11)

##### 추가됨
- ATT (App Tracking Transparency) 권한 부여 상태를 SDK에 전달하는 `AttAuthorized(bool authorized)` API 추가. [iOS]
- iOS 로그 레벨 제어를 위한 `DFConfig.IOS_LOG_LEVEL` 설정 키 및 `DFIOSLogLevel` 상수 추가. [iOS]
- iOS 네이티브 의존성에 Swift Package Manager 지원 추가.

##### 변경됨
- `DFConfig.IOS_LOG_ENABLE`이 deprecated 처리되었으며 `DFConfig.IOS_LOG_LEVEL` 사용 권장. [iOS]

##### 호환성
- Xcode 27 및 iOS 27 UIScene lifecycle 요구사항 호환성 검증 완료.
- Dfinery SDK는 딥링크/Universal Link API를 사용하지 않으므로 SceneDelegate 마이그레이션 불필요.
- Push notification 및 SDK 초기화 API는 SceneDelegate 전환의 영향을 받지 않음.

##### 참고
- iOS 27 / Xcode 27 빌드 시 UIScene lifecycle 지원을 위해 Unity 2022.3.72f1 이상 권장.

##### 업데이트
- README 문서에 EDM 및 EDM4U 설치 가이드 추가.

##### 버전 정보

- **Android**
  - Dfinery Android SDK 2.7.4
  - Dfinery Android Unity Extension 1.0.0
- **iOS**
  - Dfinery iOS SDK 2.5.1 (CocoaPods) / 2.6.1 (Swift Package Manager)

---

## 1.0.1 (2026-01-15)

##### 고쳐짐
- 이벤트와 식별자·기기 정보 간의 정밀 매칭 로직을 적용하여, 어떤 상황에서도 발생 시점의 정확한 맥락 데이터를 수집할 수 있도록 개선했습니다. [Android]
- setIdentity() 내부 로직을 개선하여 에지 케이스를 보다 안정적으로 처리하도록 수정되었습니다. [iOS]

##### 버전 정보

- **Android**
  - Dfinery Android SDK 2.4.6
  - Dfinery Android Unity Extension 1.0.0
- **iOS**
  - Dfinery iOS SDK 2.3.4
## 1.0.0 (2025-10-10)

##### 주요 사항

- 최초 배포

##### 버전 정보

- **Android**
  - Dfinery Android SDK 2.3.5
  - Dfinery Android Unity Extension 1.0.0
- **iOS**
  - Dfinery iOS SDK 2.2.1

</details>
