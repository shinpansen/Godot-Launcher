using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace GodotLauncher.Scripts.UiBinding;

public interface IUiControlBinding
{
    void RegisterPropertyChangedEvent(string propertyName, Action<object> action);
    bool HasProperty(string propertyName);
    object GetPropertyValue(string propertyName);
    T GetPropertyValue<T>(string propertyName);
    void SetPropertyValue(string propertyName, object propertyValue);
}
