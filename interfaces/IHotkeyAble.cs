﻿namespace TarkovAssistantWPF.interfaces
{
    interface IHotkeyAble
    {
        void OnCycleSubMap();
        void OnCycleMap();

        void OnZoomIn();
        void OnZoomOut();

        void OnNextMap();
        void OnPrevMap();
        void OnReset();

        void OnSetMap(Map mapToSet);

        void OnClear();

        void OnPan(int x, int y);
    }
}
