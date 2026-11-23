# Dfinery Unity SDK

Unity용 Dfinery SDK입니다. Android 및 iOS 네이티브 SDK의 C# 인터페이스와 Unity 기능을 제공합니다.

- [Dfinery User Guide](https://docs.dfinery.ai/user-guide)
- [Dfinery Developer Guide](https://docs.dfinery.ai/developer-guide/platform/unity)

## 요구 사항

- iOS UIScene lifecycle 지원 Unity 버전 필수 (Xcode 27 대응)
- External Dependency Manager (EDM4U 또는 UDM)

### 지원 Unity 버전 (iOS UIScene lifecycle)

Xcode 27부터 UISceneDelegate 기반 lifecycle이 필수화됨에 따라, 아래 버전 이상의 Unity가 필요합니다.

| Unity 버전 | 최소 요구 버전 |
|------------|---------------|
| Unity 6.5 | 6000.5.0a3+ |
| Unity 6.4 | 6000.4.0b8+ |
| Unity 6.3 LTS | 6000.3.8f1+ |
| Unity 6.0 LTS | 6000.0.68f1+ |
| Unity 2022 xLTS | 2022.3.72f1+ |

## 설치

### 1. 패키지 설치

Unity Package Manager를 통해 설치:

**Git URL 사용:**
```
Window > Package Manager > + > Add package from git URL...
```
```
https://github.com/IGAWorksDev/dfinery-unity-sdk.git
```

**로컬 경로 사용:**
```
Window > Package Manager > + > Add package from disk...
```
`package.json` 파일 선택

### 2. External Dependency Manager 설치

네이티브 의존성 해결을 위해 아래 중 하나를 설치해야 합니다:

#### 옵션 A: Unity External Dependency Manager (UDM) - 권장

Unity 2022.3 이상에서 사용 가능한 공식 패키지입니다.

```
Window > Package Manager > Unity Registry > External Dependency Manager
```

또는 `Packages/manifest.json`에 추가:
```json
{
  "dependencies": {
    "com.unity.external-dependency-manager": "2.1.0"
  }
}
```

자세한 내용: [Unity EDM 문서](https://docs.unity3d.com/Packages/com.unity.external-dependency-manager@2.1/manual/index.html)

#### 옵션 B: Google EDM4U

UDM 미지원 환경에서 사용합니다. Swift Package Manager 사용 시 **1.2.187 이상** 버전이 필요합니다.

[EDM4U 릴리스 페이지](https://github.com/googlesamples/unity-jar-resolver/releases)에서 1.2.187 이상 버전의 `.unitypackage` 다운로드 후 임포트합니다.

또는 OpenUPM을 통해 설치:
```
Window > Package Manager > + > Add package by name...
```
```
com.google.external-dependency-manager
```

자세한 내용: [EDM4U 문서](https://github.com/googlesamples/unity-jar-resolver)

### 3. 의존성 해결

패키지 설치 후 자동으로 네이티브 의존성이 해결됩니다. 수동 해결이 필요한 경우:

**Android:**
```
Assets > External Dependency Manager > Android Resolver > Force Resolve
```

**iOS:**
빌드 시 자동으로 CocoaPods/Swift Package Manager를 통해 해결됩니다.

## 네이티브 의존성

### Android
| 패키지 | 버전 |
|--------|------|
| com.igaworks.dfinery:android-sdk | 2.7.4 |
| com.igaworks.dfinery:unity-extension | 1.0.0 |
| androidx.core:core | 1.5.0 |
| com.google.android.gms:play-services-appset | 16.0.2 |
| com.google.android.gms:play-services-ads-identifier | 18.0.0 |

### iOS

> **주의:** [CocoaPods Specs 저장소가 Read-only 모드로 전환](https://blog.cocoapods.org/CocoaPods-Specs-Repo/)됨에 따라 CocoaPods을 통한 신규 버전 배포가 중단됩니다. Swift Package Manager 사용을 권장합니다.

**Swift Package Manager (권장):**
| 패키지 | 버전 |
|--------|------|
| [DfinerySDK](https://github.com/IGAWorksDev/dfinery-ios-sdk) | 2.6.1 |

**CocoaPods (지원 중단 예정):**
| Pod | 버전 |
|-----|------|
| DfinerySDK | 2.5.1 |
| DfinerySDKServiceExtension | 2.5.1 |

iOS Resolver 설정에서 Swift Package Manager 사용을 권장합니다.
