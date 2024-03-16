using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Guizzan.Input.GIM;
public interface IUniversalInputManager<T>
{
    void SetInput(T input, InputValue value);
}