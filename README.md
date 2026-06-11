# Assets/Danmaku 簡述

玩家在格子上放置「發射器塔」，塔以 ECS 大量生成子彈；
子彈、敵人、傷害結算全部跑在 DOTS（資料導向）架構上以支撐高數量同屏實體，
而 UI、棋盤格子等互動物件維持 GameObject——因此核心挑戰之一是**兩種範式之間的乾淨橋接**。

---

## 架構總覽

[Movement 階段 — 各 MoveSystem 平行，互不依賴]
   LinearMoveSystem, OrbitMoveSystem  ->  寫入 NextPosition（「這一幀想去哪」）

[Hit 階段 — 唯一的碰撞邏輯]
   HitMoveSystem  讀 currentPos -> NextPosition，沿線段 raycast
        命中：產生 DamageEvent + 銷毀子彈
        沒命中：才真正位移 (commit)

[Combat 階段]
   DamageEventSystem  集中套用傷害
   HealthSystem  血量歸零 -> 平行回收

[Bridge 階段 — ECS 通知 GameObject]
   GameObjectLinkBrokenDetectionSystem  偵測帶 GameObjectLink 的 entity 死亡 -> 發事件
   LinkBrokenDispatcher (MonoBehaviour)  依 LinkType 分派給對應 handler
   
---

## 資料夾結構

```
Assets/Danmaku/Scripts/

├── Authoring/          掛載到物件上的腳本 主要執行 Bake
│
├── Mono/               放著繼承 MonoBehaviour 的腳本
│
├── Others/
│   ├── IAsyncLoad.cs          載入物件
│   ├── ShooterSpawner.cs      生成發射器的腳本，可繼承ShooterSpawnerBase擴增
│   └── SpawnRegistry.cs       紀錄Entity物件
│
├── Systems/            繼承Isystem的腳本
│
└── Utility/ 
   └── BulletSpawnHelper.cs          生成子彈的共用方法
```
