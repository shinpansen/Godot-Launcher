using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodotLauncher.Scripts.UiBinding;

public interface IUiControlBinding
{
    bool HasChanged(ulong controlId);
    void RegisterControl(ulong controlId);
    object GetPropertyValue(string propertyName);
    T GetPropertyValue<T>(string propertyName);
}
