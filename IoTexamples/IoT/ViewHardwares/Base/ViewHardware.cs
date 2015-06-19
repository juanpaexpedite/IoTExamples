using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace IoT.ViewHardwares.Base
{
    public class ViewHardware : INotifyPropertyChanged
    {
        public async void NotifyPropertyChanged([CallerMemberName] string caller = "")
        {
            if (PropertyChanged != null)
            {
                if (Window.Current !=null && Window.Current.Dispatcher != null)
                {
                    await Window.Current.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                     {
                         PropertyChanged(this, new PropertyChangedEventArgs(caller));
                     });
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
