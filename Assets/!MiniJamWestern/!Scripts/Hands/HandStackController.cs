using System;
using System.Collections.Generic;
using System.Linq;
using BitterECS.Integration.Unity;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class HandStackController<TData, TView> : MonoBehaviour where TView : MonoBehaviour
{
    protected struct Entry { public TData data; public TView prefab; }

    [ReadOnly] public HandController<TData, TView> hand;
    private RectTransform _rect;
    private readonly Stack<Entry> _stack = new();
    private readonly Dictionary<TData, TView> _views = new();

    public event Action OnChanged;
    public int Count => _stack.Count;

    private void Awake() => _rect = GetComponent<RectTransform>();

    public virtual void Initialize(HandController<TData, TView> hand) => this.hand = hand;

    public virtual void Add(TData item, TView prefab)
    {
        _stack.Push(new Entry { data = item, prefab = prefab });
        if (prefab != null) _views[item] = Instantiate(prefab, _rect);
        OnChanged?.Invoke();
    }

    public TView GetView(TData data) => _views.TryGetValue(data, out var v) ? v : null;

    public virtual bool DrawToHand()
    {
        if (_stack.Count == 0 || hand == null) return false;

        var entry = _stack.Pop();
        var isAdded = hand.Add(entry.data, entry.prefab);
        if (!isAdded)
        {
            _stack.Push(entry);
            return false;
        }

        if (_views.Remove(entry.data, out var view)) Destroy(view.gameObject);

        OnChanged?.Invoke();
        return true;
    }

    public void Shuffle()
    {
        var list = _stack.ToList();
        _stack.Clear();
        foreach (var e in list.OrderBy(_ => UnityEngine.Random.value)) _stack.Push(e);
        OnChanged?.Invoke();
    }

    public void SetContainer(Transform container)
    {
        _rect = container.GetComponent<RectTransform>();
        foreach (var v in _views.Values) v.transform.SetParent(_rect, false);
        OnChanged?.Invoke();
    }

    public IEnumerable<TView> GetOrderedViews()
    {
        var items = _stack.ToArray();
        Array.Reverse(items);
        foreach (var item in items)
        {
            if (_views.TryGetValue(item.data, out var view)) yield return view;
        }
    }
}
