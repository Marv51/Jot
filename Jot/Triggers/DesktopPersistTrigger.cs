﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jot.Triggers
{
    public class DesktopPersistTrigger : ITriggerPersist
    {
        public DesktopPersistTrigger()
        {
            if (System.Windows.Application.Current != null)//wpf
                System.Windows.Application.Current.Exit += (s, e) => { OnApplicationClosing(); };
            else //winforms
                System.Windows.Forms.Application.ApplicationExit += (s, e) => { OnApplicationClosing(); };
        }

        private void OnApplicationClosing()
        {
            PersistRequired?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler PersistRequired;
    }
}
