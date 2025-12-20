using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prism.Models
{
    public class Login : INotifyPropertyChanged
    {

        private string _userName;
        private string _email;
        private string _userPsw;
        [NotMapped]
        private string _userConfirmPsw;
        private int _userID;

        public int UserID
        {
            get => _userID;
            set
            {
                if (_userID != value)
                {
                    _userID = value;
                    OnPropertyChanged(nameof(UserID));
                }
            }
        }
        public string UserName
        {
            get => _userName;
            set
            {
                if (_userName != value)
                {
                    _userName = value;
                    OnPropertyChanged(nameof(UserName));
                }
            }
        }

        public string UserPsw
        {
            get => _userPsw;
            set
            {
                if (_userPsw != value)
                {
                    _userPsw = value;
                    OnPropertyChanged(nameof(UserPsw));
                }
            }
        }
        public string Email
        {
            get => _email;
            set
            {
                if (_email != value)
                {
                    _email = value;
                    OnPropertyChanged(nameof(Email));
                }
            }
        }
        [NotMapped]
        public string UserConfirmPsw
        {
            get => _userConfirmPsw;
            set
            {
                if (_userConfirmPsw != value)
                {
                    _userConfirmPsw = value;
                    OnPropertyChanged(nameof(UserConfirmPsw));
                }
            }
        }



        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
