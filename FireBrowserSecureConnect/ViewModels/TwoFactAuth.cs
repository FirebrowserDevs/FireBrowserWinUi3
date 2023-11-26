using FireBrowserBusinessCore.Models;
using SecureConnectOtp;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace FireBrowserSecureConnect.ViewModels
{
    public class TwoFactAuth : INotifyPropertyChanged
    {
        private string code = "000000";
        private int progressValue = 100;

        private int remainingSeconds;
        public string Name { get; set; }

        public TwoFactorAuthItem Data { get; set; }

        public TwoFactAuth(TwoFactorAuthItem data)
        {
            Data = data;
            Name = data.Name;
        }

        public string Code
        {
            get { return code; }
            set
            {
                if (value != code)
                {
                    code = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public int ProgressValue
        {
            get { return progressValue; }
            set
            {
                if (value != progressValue)
                {
                    progressValue = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        private Totp totp;
        public void Start()
        {
            totp = new(Data.Secret, Data.Step, (OtpHashMode)Data.OtpHashMode, Data.Size);
            Refresh();

        }

        private async void Refresh()
        {
            // Calibrate the timer
            int sec = totp.RemainingSeconds();
            while (totp.RemainingSeconds() == sec)
            {
                await Task.Delay(10);
            }

            Code = totp.ComputeTotp();

            while (true)
            {
                remainingSeconds = totp.RemainingSeconds();
                ProgressValue = 100 * remainingSeconds / 30;

                if (remainingSeconds == 30)
                {
                    // Generate new code
                    Code = totp.ComputeTotp();
                }

                await Task.Delay(1000);
            }
        }
    }
}
