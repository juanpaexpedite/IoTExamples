using IoT.Common;
using IoT.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;

namespace IoT.ViewHardwares.Triggers
{
    public class DeviceTrigger : StateTriggerBase
    {
        #region Familiy
        public Families Family
        {
            get { return (Families)GetValue(FamilyProperty); }
            set { SetValue(FamilyProperty, value); }
        }
        public static readonly DependencyProperty FamilyProperty =
            DependencyProperty.Register(nameof(Family), typeof(Families), typeof(DeviceTrigger),
                new PropertyMetadata(Families.None, FamilyChanged));

        private static void FamilyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var trigger = d as DeviceTrigger;
            
            trigger.SetTrigger();
        }
        #endregion


        public DeviceTrigger()
        {
            Initialize();
        }

        private void Initialize()
        {
            if (SystemService.IsRuntime)
            {
                //Initial Trigger
                //NavigatedEventHandler framenavigated = null;
                //framenavigated = (s, e) =>
                //{
                //    SystemService.DisplayFrame.Navigated -= framenavigated;
                //    SetTrigger();
                //};
                //SystemService.DisplayFrame.Navigated += framenavigated;

            }
        }

        public void SetTrigger()
        {
            SetActive(Family == SystemService.Family);
        }


    }
}
