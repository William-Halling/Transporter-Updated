using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Transporter.Events
{
    public static class EventKeys
    {
            // "WorldEvent.NewHour", "GameEventType.SceneLoaded", etc.
        public static string Key<TEnum>(TEnum value) where TEnum : Enum
            => $"{typeof(TEnum).Name}.{value}";
    }
}
