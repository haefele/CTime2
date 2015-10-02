using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;

namespace CTime2.Views.StampTime
{
    public class CheckedInViewModel : Screen
    {
        public StampTimeViewModel Container => this.Parent as StampTimeViewModel;
    }
}
