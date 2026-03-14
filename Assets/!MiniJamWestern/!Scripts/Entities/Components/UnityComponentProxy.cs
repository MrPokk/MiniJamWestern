using BitterECS.Integration.Unity;
using UnityEngine;
using UnityEngine.UI;

// ===== Базовые компоненты =====
public class TransformProvider : ProviderUnity<Transform> { }
public class RectTransformProvider : ProviderUnity<RectTransform> { }

// ===== Физика 3D =====
public class RigidbodyProvider : ProviderUnity<Rigidbody> { }
public class ColliderProvider : ProviderUnity<Collider> { }
public class BoxColliderProvider : ProviderUnity<BoxCollider> { }
public class SphereColliderProvider : ProviderUnity<SphereCollider> { }
public class CapsuleColliderProvider : ProviderUnity<CapsuleCollider> { }
public class MeshColliderProvider : ProviderUnity<MeshCollider> { }

// ===== Физика 2D =====
public class Rigidbody2DProvider : ProviderUnity<Rigidbody2D> { }
public class Collider2DProvider : ProviderUnity<Collider2D> { }
public class BoxCollider2DProvider : ProviderUnity<BoxCollider2D> { }
public class CircleColliderProvider : ProviderUnity<CircleCollider2D> { }
public class PolygonCollider2DProvider : ProviderUnity<PolygonCollider2D> { }
public class EdgeCollider2DProvider : ProviderUnity<EdgeCollider2D> { }

// ===== Рендеринг =====
public class MeshFilterProvider : ProviderUnity<MeshFilter> { }
public class MeshRendererProvider : ProviderUnity<MeshRenderer> { }
public class SkinnedMeshRendererProvider : ProviderUnity<SkinnedMeshRenderer> { }
public class SpriteRendererProvider : ProviderUnity<SpriteRenderer> { }
public class LineRendererProvider : ProviderUnity<LineRenderer> { }
public class TrailRendererProvider : ProviderUnity<TrailRenderer> { }

// ===== Анимация =====
public class AnimatorProvider : ProviderUnity<Animator> { }
public class AnimationProvider : ProviderUnity<Animation> { }

// ===== Камера и свет =====
public class CameraProvider : ProviderUnity<Camera> { }
public class LightProvider : ProviderUnity<Light> { }

// ===== Аудио =====
public class AudioSourceProvider : ProviderUnity<AudioSource> { }
public class AudioListenerProvider : ProviderUnity<AudioListener> { }

// ===== Эффекты =====
public class ParticleSystemProvider : ProviderUnity<ParticleSystem> { }

// ===== UI =====
public class CanvasProvider : ProviderUnity<Canvas> { }
public class CanvasGroupProvider : ProviderUnity<CanvasGroup> { }
public class GraphicRaycasterProvider : ProviderUnity<GraphicRaycaster> { }
public class ImageProvider : ProviderUnity<Image> { }
public class TextProvider : ProviderUnity<Text> { }
public class ButtonProvider : ProviderUnity<Button> { }
public class SliderProvider : ProviderUnity<Slider> { }
public class ToggleProvider : ProviderUnity<Toggle> { }
public class InputFieldProvider : ProviderUnity<InputField> { }
public class ScrollRectProvider : ProviderUnity<ScrollRect> { }
