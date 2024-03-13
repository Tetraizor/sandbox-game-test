namespace Tetraizor.Data;

public class ReactiveProperty<T>
{
    private T _value;
    public T Value
    {
        get => _value;
        set
        {
            if (value.Equals(_value)) return;

            T oldValue = _value;
            _value = value;

            OnValueChanged?.Invoke(oldValue, value);
        }
    }

    public delegate void ValueChangedEventHandler(T oldValue, T newValue);
    public ValueChangedEventHandler OnValueChanged;

    public ReactiveProperty(T value)
    {
        _value = value;
    }
}