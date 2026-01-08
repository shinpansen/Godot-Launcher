using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodotLauncher.Scripts.Binding.Interfaces;

public interface IItemBinding
{
    void Init(object model);
    void Init(object model, IDataSourceBinding dataSource);
}
