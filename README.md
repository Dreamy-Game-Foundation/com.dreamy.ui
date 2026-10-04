# Dreamy UI

Package thuộc Dreamy Game Studio. Hướng dẫn dưới đây mô tả cấu trúc, cách cài vào project và tích hợp ở root/scene.

## Cài package

Dùng Unity 6000.0 trở lên. Sandbox đã tham chiếu package bằng `file:../LocalPackages/com.dreamy.ui`. Project khác dùng Package Manager > + > Install package from disk và chọn package.json, hoặc Git URL của repository nội bộ. Cài cả dependency Dreamy/Git vào manifest của game; version dependency không tự cấu hình registry riêng.

Dependency trực tiếp theo package.json:

- `com.unity.ugui` (2.0.0)
- `com.cysharp.unitask` (2.5.10)
- `com.demigiant.dotween` (0.0.3)
- `com.dreamy.core` (1.1.2)
- `com.dreamy.assets` (0.1.1)

## Cấu trúc và asmdef

| Assembly | Reference | Phạm vi |
| --- | --- | --- |
| `Dreamy.UI.Editor` | Dreamy.UI.Runtime | Chỉ Editor |
| `Dreamy.UI.Runtime` | Dreamy.Core.Runtime, Dreamy.Assets.Runtime, UniTask, DOTween.Modules, Unity.TextMeshPro | Runtime |

Trong asmdef của game, thêm assembly chứa API trực tiếp sử dụng. Code bootstrap reference thêm Core/DataConfig/Datasave/Economy theo nhu cầu; code async reference UniTask. Code gọi type sample reference assembly sample. Giữ Editor reference trong asmdef Editor-only.

## Cấu trúc và thiết lập scene

Runtime/Panel chứa UIPanel, PanelManager, UILayerRoot; Button/Tab xử lý tương tác; Tween chứa player/effect/preset; Utils chứa safe area, progress và shine; Shaders chứa shader UI. Editor hỗ trợ Inspector. UI không có service installer ở GameInstaller; scene đặt PanelManager trên Canvas và có EventSystem/input module.

```csharp
using Dreamy.UI;
public sealed class HomePanel : UIPanel
{
    public override bool CanBack => false;
}
```

UIPanel chọn Layer Screen/Popup/Overlay; đặt UILayerRoot dưới manager hoặc để manager tạo root thiếu. Override CanCache=true nếu muốn deactivate khi đóng và dùng lại instance. Escape/back đóng panel gần nhất có CanBack. Root phải cài service feature trước khi tạo panel.

## Animation và tiện ích

UITweenPlayer có Auto (thu thập component dưới hierarchy, dừng tại player lồng) và Manual (nhóm target/effect được cấu hình trong Inspector). Tạo preset tại Assets/Create/Dreamy/UI/Tween Preset; cấu hình TweenPresetLibrary hoặc override duration/ease/delay cho show/hide.

TweenDelayControl và marker TweenDelayByIndex dùng cho item xuất hiện tuần tự. Sau spawn/reorder, cập nhật delay theo API của control; giữ một chủ sở hữu timing. UITweenPlayer chỉ chạy effect. UIProgressBar dùng Image Filled; UIShineWave có material runtime riêng, controller tổ chức các đợt shine. UIScalable có thể dùng pulse khi idle.

Trong method async UniTask, dùng Show để mở panel đã tự bind, Create để bind presenter trước animation, Close để đóng, Transition để chuyển từ panel hiện tại. Game giữ localization, art, âm thanh và logic scene.
## Sample

Manifest hiện không khai báo sample để import qua Package Manager.

## Addressables Group và class address

1. Lưu prefab/variant của game tại Assets/_Project/Prefabs/Panel/HomePanel.prefab. Với UIPanel, root phải có subclass tương ứng.
2. Mở Window > Asset Management > Addressables > Groups; tạo settings nếu chưa có.
3. Tạo group UI Panels và kéo prefab vào group.
4. Đặt cột Address thành Panel/HomePanel.prefab.
5. Tạo class dùng chung trong game:

```csharp
public static class PanelAddress
{
    public const string Home = "Panel/HomePanel.prefab";
    public const string Current = "Panel/HomePanel.prefab";
}
```

Đường dẫn asset trên disk và address là hai giá trị riêng. Address do bạn đặt, constant phải khớp chính xác cột Address. Tên group không phải key tải. HomePanel là ví dụ subclass do game tự tạo.

Scene cần Canvas có PanelManager và EventSystem/input module. Chờ root cài service xong. Các lệnh sau nằm trong method async UniTask; asmdef reference Dreamy.UI.Runtime, UniTask và assembly chứa type panel.

```csharp
var panel = await PanelManager.Instance.Create<HomePanel>(PanelAddress.Current);
await panel.Show();
// Đóng từ code game:
await PanelManager.Instance.Close<HomePanel>();
```

Host giữ một presenter cho mỗi panel instance, dispose lúc teardown, bind/render lại khi mở panel cache. Không chạy đồng thời controller sample và presenter khác trên cùng panel. PanelManager không tự cài service feature.

Build Addressables content cho target trước khi thử player. AssetLoader cache prefab; đóng panel không tự unload cache. Chỉ unload sau khi mọi instance/consumer đã kết thúc.
