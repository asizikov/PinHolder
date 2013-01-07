using PinHolder.Annotations;

namespace PinHolder.Model
{
    public struct ApplicationSettings
    {
        private string _masterPassword;

        public bool UseMasterPassword { get; set; }

        [NotNull]
        public string MasterPassword
        {
            get
            {
                return _masterPassword ?? (_masterPassword = string.Empty);
            }
            set {
                _masterPassword = value ?? string.Empty;
            }
        }


        public bool Equals(ApplicationSettings other)
        {
            return UseMasterPassword.Equals(other.UseMasterPassword) && string.Equals(MasterPassword, other.MasterPassword);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is ApplicationSettings && Equals((ApplicationSettings) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (UseMasterPassword.GetHashCode()*397) ^ MasterPassword.GetHashCode();
            }
        }

        public static bool operator ==(ApplicationSettings left, ApplicationSettings right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ApplicationSettings left, ApplicationSettings right)
        {
            return !left.Equals(right);
        }
    }
}
