using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public static class UiBuilderUtils
{
    public static void RegisterMouseEnterExitEvent(this Button button, EventCallback<MouseEnterEvent> enter, EventCallback<MouseOutEvent> exit)
    {
        button.RegisterCallback<MouseEnterEvent>(enter);
        button.RegisterCallback<MouseOutEvent>(exit);
    }

    public static void UnRegisterMouseEnterExitEvent(this Button button, EventCallback<MouseEnterEvent> enter, EventCallback<MouseOutEvent> exit)
    {
        button.UnregisterCallback<MouseEnterEvent>(enter);
        button.UnregisterCallback<MouseOutEvent>(exit);
    }
}
