using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TarkovAssistantWPF.interfaces
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
    }
}
